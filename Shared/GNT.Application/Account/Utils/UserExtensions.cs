using GNT.Domain.Models;
using GNT.Enums;
using GNT.Common.Extensions;
using GNT.Shared.Errors;
using Microsoft.AspNetCore.Identity;

namespace GNT.Application.Account.Utils;

public static class UserValidationExtension
{
    public static void ValidatePassword(this User user, string inputPassword, IPasswordHasher<User> passwordHasher)
    {
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, inputPassword);
        if (result == PasswordVerificationResult.Failed)
            throw new BusinessException(FailureCode.InvalidCredentials);
    }

    public static void ValidateStatus(this User user)
    {
        if (user.IsBlocked && user.UnblockDate > DateTime.Now)
        {
            if (user.UnblockDate > DateTime.Now.AddYears(10))
                throw new BusinessException(FailureCode.UserPermanentlyBlocked);
            else
                throw new BusinessException(FailureCode.UserTemporaryBlocked);
        }
        else if (user.IsBlocked && user.UnblockDate < DateTime.Now)
        {
            user.IsBlocked = false;
            user.UnblockDate = null;
        }
    }

    public static UserSecurityCode GenerateUserSecurityCode(this User user, SecurityCodeTypes type)
    {
        var expiresAt = DateTime.Now;
        var code = type switch
        {
            SecurityCodeTypes.ResetPassword => (
                expires: expiresAt.AddMinutes(30),
                code: StringExtensions.GenerateRandomString(100)),
            SecurityCodeTypes.TwoFactorAuthentication => (
                expires: expiresAt.AddMinutes(5),
                code: StringExtensions.GenerateRandomString(6, onlyDigits: true)),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };

        var securityCode = new UserSecurityCode
        {
            Code = code.code,
            CreatedAt = DateTime.Now,
            ExpiresAt = code.expires,
            FailedAttempts = 0,
            Type = type,
            UserId = user.Id
        };

        user.UserSecurityCodes.Add(securityCode);
        return securityCode;
    }

    public static FailureCode? ValidateUserSecurityCode(this User user, string requestSecurityCode)
    {
        var securityCode = user.UserSecurityCodes.SingleOrDefault();

        if (securityCode != null && securityCode.Code == requestSecurityCode
            && !securityCode.SuccessfullyUsed && securityCode.ExpiresAt > DateTime.Now)
            return null;

        if (securityCode == null)
            return FailureCode.InternalError;

        if (securityCode.ExpiresAt < DateTime.Now && securityCode.Code == requestSecurityCode)
            return FailureCode.SecurityCodeExpired;

        if (securityCode.SuccessfullyUsed && securityCode.Code == requestSecurityCode)
            return FailureCode.SecurityCodeAlreadyUsed;

        if (securityCode.Code != requestSecurityCode && securityCode.ExpiresAt > DateTime.Now)
        {
            securityCode.FailedAttempts++;
            if (securityCode.FailedAttempts >= 5)
            {
                user.IsBlocked = true;
                user.UnblockDate = DateTime.Now.AddHours(1);
            }
            return FailureCode.InvalidSecurityCode;
        }

        return FailureCode.InternalError;
    }
}