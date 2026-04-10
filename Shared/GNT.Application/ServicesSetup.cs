using GNT.Application.Account.Commands;
using GNT.Application.Account.Utils;
using GNT.Application.Infrastructure;
using GNT.Application.Interfaces;
using GNT.Application.Security;
using GNT.Application.Services.Infrastructure;
using GNT.Domain.Models;
using GNT.Enums;
using GNT.Shared;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace GNT.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSharedServices();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

        services.AddTransient<IPaginationService, PaginationService>();
        services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

        services.AddScoped<ICurrentSession, CurrentSession>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        services.AddValidatorsFromAssemblyContaining<CreateUserCommandValidator>();
        services.AddMediatR(typeof(LoginCommand).Assembly);

        services.AddJwt(configuration);

        return services;
    }

    public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var tokenOptions = new TokenProviderOptions();
        configuration.GetSection("TokenProvider").Bind(tokenOptions);

        services.Configure<TokenProviderOptions>(configuration.GetSection("TokenProvider"));
        services.AddScoped<JwtService>();

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecretKey));

        services.AddAuthorizationCore(options =>
        {
            var permissions = Enum.GetValues(typeof(Permission)).Cast<Permission>();
            foreach (var permission in permissions)
            {
                options.AddPolicy(permission.ToString(), policy => policy.RequireAssertion(context =>
                {
                    if (!context.User.HasClaim(c => c.Type == ClaimTypes.AuthorizationDecision))
                        return false;

                    var userPermissions = JsonConvert.DeserializeObject<IEnumerable<Permission>>(
                        context.User.FindFirst(x => x.Type == ClaimTypes.AuthorizationDecision)!.Value);

                    return userPermissions!.Any(x => x == permission);
                }));
            }
        })
        .AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(o =>
        {
            o.RequireHttpsMetadata = !configuration.GetValue<bool>("IsDevelopment");
            o.SaveToken = true;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = ClaimTypes.Email,
                RoleClaimType = ClaimTypes.Role,

                ValidateIssuer = true,
                ValidIssuer = tokenOptions.Issuer,

                ValidateAudience = true,
                ValidAudience = tokenOptions.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = secretKey,

                ValidateLifetime = true,
                LifetimeValidator = (notBefore, expires, _, _) =>
                    notBefore <= DateTime.UtcNow && expires >= DateTime.UtcNow,

                AuthenticationType = JwtBearerDefaults.AuthenticationScheme
            };

            o.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies[tokenOptions.CookieName] ??
                        (string?)context.Request.HttpContext.Items[tokenOptions.CookieName];

                    if (string.IsNullOrEmpty(context.Token))
                    {
                        context.Request.Headers.TryGetValue("Authorization", out var bearerToken);
                        context.Token = bearerToken.ToString().Replace("bearer ", "");
                    }

                    return Task.CompletedTask;
                }
            };
        });
    }
}