using GNT.Common.Client;
using GNT.Dtos.Enums;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Globalization;
using System.Security.Claims;

namespace GNT.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

            builder.Services.AddAuthorizationCore(options =>
            {
                var permissions = Enum.GetValues(typeof(Permission)).Cast<Permission>();

                foreach (var permission in permissions)
                {
                    options.AddPolicy(permission.ToString(), policy => policy.RequireAssertion(context =>
                    {
                        if (!context.User.HasClaim(c => c.Type == ClaimTypes.AuthorizationDecision))
                        {
                            return false;
                        }

                        var userPermissions = JsonConvert.DeserializeObject<IEnumerable<Permission>>(context.User.FindFirst(x => x.Type == ClaimTypes.AuthorizationDecision).Value);

                        var hasPermission = userPermissions.Any(x => x == permission);

                        return hasPermission;
                    }));
                }
            });

            builder.Services.AddCommonClientServices("");

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build(); ;
        }
    }
}
