// StoreService.Infrastructure/DependencyInjection/InfrastructureServiceCollectionExtensions.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using StoreService.Infrastructure.Data;                  // AppDbContext
using StoreService.Infrastructure.Data.Repositories;     // ProdutoRepository, etc.

// Interfaces (Application)
using StoreService.Application.Estoque.Interfaces;
using StoreService.Application.Pedidos.Interfaces;

// JWT (contrato na Application, implementação na Infrastructure)
using StoreService.Application.Security;
using StoreService.Infrastructure.Security;
using StoreService.Application.Produtos.Interfaces;
using StoreService.Application.Usuarios.Interfaces;
using StoreService.Application.Messaging;
using StoreService.Infrastructure.Messaging;

namespace StoreService.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext (SQL Server)
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("Default")));

            services.AddScoped<IEventPublisher, MassTransitEventPublisher>();

            // JWT
            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
            services.AddSingleton<ITokenService, TokenService>();

            // Repositórios (EF)
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<IEstoqueRepository, EstoqueRepository>();
            services.AddScoped<IPedidoRepository, PedidoRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            return services;
        }
    }
}