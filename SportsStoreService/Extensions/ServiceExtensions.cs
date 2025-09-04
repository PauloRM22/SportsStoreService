using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SportsStoreService.Data;
using SportsStoreService.Data.Repositories;
using StoreService.Application.Products.Interfaces;
using StoreService.Application.Products.UseCases;

namespace SportsStoreService.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("Default")));
        }

        public static void ConfigureProducts(this IServiceCollection services)
        {
            // Repositório
            services.AddScoped<IProdutoRepository, ProdutoRepository>();

            // UseCases
            services.AddScoped<CriarProdutoUseCase>();
            services.AddScoped<AtualizarProdutoUseCase>();
            services.AddScoped<ListarProdutosUseCase>();
            services.AddScoped<ObterProdutoPorIdUseCase>();
            services.AddScoped<RemoverProdutoUseCase>();
        }
    }
}
