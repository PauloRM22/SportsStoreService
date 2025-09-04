using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using SportsStoreService.Data;
using SportsStoreService.Data.repositories;
using SportsStoreService.Data.Repositories;
using SportsStoreService.Validations.Estoque;
using SportsStoreService.Validations.Produtos;
using StoreService.Application.Estoque.Interfaces;
using StoreService.Application.Estoque.UseCases;
using StoreService.Application.Products.Interfaces;
using StoreService.Application.Products.UseCases;

namespace SportsStoreService.Extensions
{
    public static class ServiceExtensions
    {
        // Registra TUDO que a app precisa (DbContext, repos, use cases, controllers, swagger, validators)
        public static IServiceCollection AddSportsStore(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("Default")));

            // Repositórios
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<IEstoqueRepository, EstoqueRepository>();

            // UseCases - Produtos
            services.AddScoped<CriarProdutoUseCase>();
            services.AddScoped<AtualizarProdutoUseCase>();
            services.AddScoped<ListarProdutosUseCase>();
            services.AddScoped<ObterProdutoPorIdUseCase>();
            services.AddScoped<RemoverProdutoUseCase>();

            // UseCases - Estoque
            services.AddScoped<RegistrarEntradaEstoqueUseCase>();
            services.AddScoped<ConsultarSaldoProdutoUseCase>();

            // Controllers + FluentValidation
            services.AddControllers();

            services.AddFluentValidationAutoValidation();

            // Registro dos validators (quantos precisar)
            services.AddValidatorsFromAssemblyContaining<CreateProdutoRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateEntradaEstoqueRequestValidator>();

            // Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }

        // Pipeline da aplicação (Swagger, HTTPS, Auth, Controllers)
        public static WebApplication UseSportsStore(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            return app;
        }
    }
}
