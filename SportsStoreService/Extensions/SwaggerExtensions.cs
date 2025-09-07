using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace SportsStoreService.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Sports Store API",
                    Version = "v1",
                    Description = "API da Sports Store — usuários, produtos, estoque e pedidos."
                });

                // habilita [SwaggerOperation] etc.
                c.EnableAnnotations();

                // 🆕 ordena ações pelo OperationId (que vamos setar nos métodos)
                c.OrderActionsBy(apiDesc =>
                {
                    apiDesc.TryGetMethodInfo(out var mi);
                    var attr = mi?.GetCustomAttribute<SwaggerOperationAttribute>();
                    return attr?.OperationId ?? $"{apiDesc.GroupName}_{apiDesc.HttpMethod}_{apiDesc.RelativePath}";
                });

                // coloca tags “amigáveis” nos grupos (Auth → Autenticação, etc.)
                c.OperationFilter<FriendlyTagsOperationFilter>();

                // define a ordem e descrição dos grupos (tags) — Autenticação em 1º
                c.DocumentFilter<TagDescriptionsDocumentFilter>();

                // JWT Bearer
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Envie: **Bearer {token}**",
                    Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme }
                };

                c.AddSecurityDefinition("Bearer", jwtSecurityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });
            });

            return services;
        }
    }

    // Mapeia nomes de controllers para tags “bonitas”
    internal sealed class FriendlyTagsOperationFilter : IOperationFilter
    {
        private static readonly Dictionary<string, string> Map = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Auth"] = "Autenticação",
            ["Usuario"] = "Usuários",
            ["Usuarios"] = "Usuários",
            ["Produto"] = "Produtos",
            ["Produtos"] = "Produtos",
            ["Estoque"] = "Estoque",
            ["Pedido"] = "Pedidos",
            ["Pedidos"] = "Pedidos",
        };

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.ActionDescriptor.RouteValues.TryGetValue("controller", out var ctrl)
                && ctrl is not null
                && Map.TryGetValue(ctrl, out var friendly))
            {
                operation.Tags = new List<OpenApiTag> { new() { Name = friendly } };
            }
        }
    }

    // Define a ORDEM e a DESCRIÇÃO dos grupos (tags) no topo do Swagger
    internal sealed class TagDescriptionsDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Tags = new List<OpenApiTag>
            {
                new() { Name = "Autenticação", Description = "Login para obter o token JWT e acessar rotas protegidas." },
                new() { Name = "Usuários",     Description = "Gestão de contas. Criação/edição apenas por **Admin**." },
                new() { Name = "Produtos",     Description = "Catálogo de produtos. Operações administrativas por **Admin**." },
                new() { Name = "Estoque",      Description = "Entradas e consulta de saldo (NF, quantidade). **Admin**." },
                new() { Name = "Pedidos",      Description = "Criação e confirmação de pedidos com baixa de estoque. **Vendedor**." },
            };
        }
    }
}