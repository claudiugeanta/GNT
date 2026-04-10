using GNT.Application.Security;
using GNT.Domain.Models;
using GNT.Enums;
using GNT.Shared.Dtos.UserManagement;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GNT.Application.Account.Utils;

public class JwtService(IOptions<TokenProviderOptions> tokenOptions)
{
    private readonly TokenProviderOptions _tokenOptions = tokenOptions.Value;

    public TokenDto GenerateToken(User user, IEnumerable<Permission> permissions)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.AuthorizationDecision, JsonConvert.SerializeObject(permissions)),
            new(ClaimTypes.Role, JsonConvert.SerializeObject(permissions))
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.SecretKey));

        var jwtToken = new JwtSecurityToken(
            claims: claims,
            issuer: _tokenOptions.Issuer,
            audience: _tokenOptions.Audience,
            expires: DateTime.UtcNow.AddSeconds(_tokenOptions.ExpirationSeconds),
            notBefore: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256));

        return new TokenDto
        {
            Value = new JwtSecurityTokenHandler().WriteToken(jwtToken)
        };
    }
}