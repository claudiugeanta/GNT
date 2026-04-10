using GNT.Application.Interfaces;
using GNT.Enums;
using Microsoft.EntityFrameworkCore;

namespace GNT.Application.Account.Queries
{
    public class CheckSecurityCodeQuery : IRequest<bool>
    {
        public CheckSecurityCodeQuery(string securityCode, SecurityCodeTypes type)
        {
            SecurityCode = securityCode;
            Type = type;
        }

        public string SecurityCode { get; set; }
        public SecurityCodeTypes Type { get; set; }
    }

    internal class CheckSecurityCodeQueryHandler(IAppDbContext appDbContext) : IRequestHandler<CheckSecurityCodeQuery, bool>
    {
        public async Task<bool> Handle(CheckSecurityCodeQuery request, CancellationToken cancellationToken)
        {
            var dbSecurityCode = await appDbContext.UserSecurityCode.Where(d=>d.Code == request.SecurityCode).FirstOrDefaultAsync(cancellationToken);

            if(dbSecurityCode == null 
                || dbSecurityCode.ExpiresAt < DateTime.Now
                || appDbContext.UserSecurityCode.Any(d => d.UserId == dbSecurityCode.UserId && d.Id != dbSecurityCode.Id && d.Type == dbSecurityCode.Type && d.CreatedAt > dbSecurityCode.CreatedAt)
                || dbSecurityCode.SuccessfullyUsed)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
