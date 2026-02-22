using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace OnlineChatBackend.Settings
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddAuth(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var authSettings = configuration.GetSection(nameof(AuthSettings)).Get<AuthSettings>();

            serviceCollection
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(authSettings.SecretKey))
                    };

                    o.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var path = context.HttpContext.Request.Path;

                            // 1) SignalR: как было — из query ?access_token=
                            if (path.StartsWithSegments("/hubs/chat"))
                            {
                                var accessToken = context.Request.Query["access_token"];
                                if (!string.IsNullOrEmpty(accessToken))
                                {
                                    context.Token = accessToken;
                                }

                                return Task.CompletedTask;
                            }

                            // 2) Обычные HTTP-запросы: читаем токен из cookie "accessToken"
                            var cookieToken = context.Request.Cookies["accessToken"];
                            if (!string.IsNullOrEmpty(cookieToken))
                            {
                                context.Token = cookieToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            return serviceCollection;
        }
    }
}
