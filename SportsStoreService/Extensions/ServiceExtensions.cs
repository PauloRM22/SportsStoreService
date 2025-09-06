using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using SportsStoreService.Data;
using SportsStoreService.Data.repositories;
using SportsStoreService.Data.Repositories;
using SportsStoreService.Middlewares;
using SportsStoreService.Validations.Estoque;
using SportsStoreService.Validations.Pedidos;
using SportsStoreService.Validations.Produtos;
using SportsStoreService.Validations.Usuarios;
using StoreService.Application.Auth.UseCases;
using StoreService.Application.Estoque.Interfaces;
using StoreService.Application.Estoque.UseCases;
using StoreService.Application.Pedidos.Interfaces;
using StoreService.Application.Pedidos.UseCases;
using StoreService.Application.Products.Interfaces;
using StoreService.Application.Products.UseCases;
using StoreService.Application.Security;
using StoreService.Application.Users.UseCases;
using StoreService.Application.Users.Interfaces;

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
            services.AddScoped<IPedidoRepository, PedidoRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();

            // UseCases - Produtos
            services.AddScoped<CriarProdutoUseCase>();
            services.AddScoped<AtualizarProdutoUseCase>();
            services.AddScoped<ListarProdutosUseCase>();
            services.AddScoped<ObterProdutoPorIdUseCase>();
            services.AddScoped<RemoverProdutoUseCase>();

            // UseCases - Estoque
            services.AddScoped<RegistrarEntradaEstoqueUseCase>();
            services.AddScoped<ConsultarSaldoProdutoUseCase>();

            // UseCases - Pedidos
            services.AddScoped<CriarPedidoComItensUseCase>();
            services.AddScoped<ObterPedidoPorIdUseCase>();
            services.AddScoped<ConfirmarPedidoUseCase>();

            // UseCases - Usuarios
            services.AddScoped<CriarUsuarioUseCase>();
            services.AddScoped<AtualizarUsuarioUseCase>();
            services.AddScoped<ExcluirUsuarioUseCase>();
            services.AddScoped<ObterUsuarioPorIdUseCase>();
            services.AddScoped<ListarUsuariosUseCase>();

            // Controllers + FluentValidation
            services.AddControllers();

            services.AddFluentValidationAutoValidation();

            // Registro dos validators (quantos precisar)
            services.AddValidatorsFromAssemblyContaining<CreateProdutoRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateEntradaEstoqueRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateUsuarioRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CreatePedidoRequestValidator>();

            // Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Token + Login
            services.AddSingleton<ITokenService, TokenService>();
            services.AddScoped<LoginUseCase>();

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
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseAuthorization();
            app.MapControllers();

            return app;
        }
    }
}
