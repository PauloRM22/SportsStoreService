using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using StoreService.Application.Security;
using System.Security.Claims;
using System.Text;
using System.Text.Json; // <- para serializar o JSON de erro

namespace SportsStoreService.Extensions
{
    public static class JwtAuthExtensions
    {
        public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration cfg)
        {
            var jwtOptions = cfg.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
            if (string.IsNullOrWhiteSpace(jwtOptions.Key) ||
                string.IsNullOrWhiteSpace(jwtOptions.Issuer) ||
                string.IsNullOrWhiteSpace(jwtOptions.Audience))
            {
                throw new InvalidOperationException("Configuração JWT inválida. Verifique appsettings.json (Jwt:Key, Jwt:Issuer, Jwt:Audience).");
            }

            services.AddSingleton(jwtOptions);
            services.AddSingleton<ITokenService, TokenService>();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromSeconds(30),
                        RoleClaimType = ClaimTypes.Role
                    };

                    // 👇 respostas JSON padronizadas
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            if (!context.Response.HasStarted)
                            {
                                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                context.Response.ContentType = "application/json";
                                var payload = JsonSerializer.Serialize(new
                                {
                                    error = "UNAUTHORIZED",
                                    message = "Token ausente ou inválido. Faça login e envie Authorization: Bearer {token}."
                                });
                                return context.Response.WriteAsync(payload);
                            }
                            return Task.CompletedTask;
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";
                            var payload = JsonSerializer.Serialize(new
                            {
                                error = "FORBIDDEN",
                                message = "Você não tem permissão para acessar este recurso."
                            });
                            return context.Response.WriteAsync(payload);
                        }
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Administrador"));
                options.AddPolicy("VendedorOnly", policy => policy.RequireRole("Vendedor"));

                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            return services;
        }
    }
}