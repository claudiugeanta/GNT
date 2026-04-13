using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace GNT.Common.Client.Services
{
    public class ServerAuthenticationStateProvider(ILocalStorageService localStorage) : AuthenticationStateProvider
    {
        private static readonly JwtSecurityTokenHandler _tokenHandler = new();

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await localStorage.GetItemAsync<string>("jwt-cookie");

            if(string.IsNullOrEmpty(token))
            {
                return new AuthenticationState(new ClaimsPrincipal());
            }

            try
            {

                var jwtToken = _tokenHandler.ReadJwtToken(token);

                if (jwtToken.ValidTo < DateTime.UtcNow)
                    return new AuthenticationState(new ClaimsPrincipal());

                var claims = jwtToken.Claims.ToList();
                var identity = new ClaimsIdentity(claims, "JWT");
                return new AuthenticationState(new ClaimsPrincipal(identity));

            }
            catch
            {
                return new AuthenticationState(new ClaimsPrincipal());
            }
        }
    }
}
