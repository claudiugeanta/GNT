using GNT.Application.Account.Utils;
using GNT.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;

namespace GNT.Web.Server.Config
{
    public static class ServicesSetup
    {
        public static void AddServerServices(this IServiceCollection services, ConfigurationManager Configuration)
        {
            services.AddControllers();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;

            });

            services.AddCors(o => o.AddPolicy("TEMPLATE_CORS_POLICY", b =>
            {
                var origins = Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
                b.WithOrigins(origins)
                 .AllowAnyHeader()
                 .AllowAnyMethod()
                 .AllowCredentials();
            }));

            var aesKey = Configuration["Security:AesEncryptionKey"];

            services.AddMemoryCache();

            services.AddOpenApi();

            services.AddHealthChecks().AddDbContextCheck<AppDbContext>("database");

            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo
            //    {
            //        Title = "Template API",
            //        Version = "v1"
            //    });
            //    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            //    {
            //        Name = "Authorization",
            //        Type = SecuritySchemeType.ApiKey,
            //        Scheme = "Bearer",
            //        BearerFormat = "JWT",
            //        In = ParameterLocation.Header,
            //        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
            //    });
            //    c.AddSecurityRequirement(new OpenApiSecurityRequirement
            //    {
            //        {
            //            new OpenApiSecurityScheme
            //            {
            //                Reference = new OpenApiReference
            //                {
            //                    Type = ReferenceType.SecurityScheme,
            //                        Id = "Bearer"
            //                }
            //            },
            //            Array.Empty<string>()
            //        }

            //    });
            //});
        }
    }
}
