using FluentValidation;
using FluentValidation.AspNetCore;

// Application (DTOs/UseCases/Validators/Segurança)
using StoreService.Application.Auth.UseCases;
using StoreService.Application.Estoque.UseCases;
using StoreService.Application.Pedidos.UseCases;
using StoreService.Application.Produtos.UseCases;
using StoreService.Application.Security;
using StoreService.Application.Usuarios.UseCases;


// Ponto de ancoragem para varredura de validators (qualquer classe da Application)
using StoreService.Application.Validations.Produtos;
using StoreService.Infrastructure.Security;
using System.Text.Json.Serialization;

namespace SportsStoreService.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddSportsStore(this IServiceCollection services, IConfiguration configuration)
        {
            // MVC + FluentValidation (sem ModelStateFilter custom)
            services
                .AddControllers()
                .AddJsonOptions(o =>
                {
                    // aceitar/enviar enums como string: "Administrador", "Vendedor"
                    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
                });
            services.AddFluentValidationAutoValidation();

            // Varredura de VALIDATORS na ASSEMBLY da Application
            services.AddValidatorsFromAssembly(typeof(CreateProdutoRequestValidator).Assembly);

            // Segurança (hash de senha) – implementação simples pode ficar na Application
            services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();

            // TokenService: se quiser ser 100% purista, mova a implementação p/ Infrastructure.
            // Por ora, mantendo aqui para não quebrar. (Se mover, remova a linha abaixo e registre na Infrastructure.)
            services.AddSingleton<ITokenService, TokenService>();

            // USE CASES (escopo por requisição)
            services.AddScoped<LoginUseCase>();

            // Produtos
            services.AddScoped<ListarProdutosUseCase>();
            services.AddScoped<ObterProdutoPorIdUseCase>();
            services.AddScoped<CriarProdutoUseCase>();
            services.AddScoped<AtualizarProdutoUseCase>();
            services.AddScoped<RemoverProdutoUseCase>();

            // Estoque
            services.AddScoped<RegistrarEntradaEstoqueUseCase>();
            services.AddScoped<ConsultarSaldoProdutoUseCase>();

            // Pedidos
            services.AddScoped<CriarPedidoComItensUseCase>();
            services.AddScoped<ConfirmarPedidoUseCase>();
            services.AddScoped<ObterPedidoPorIdUseCase>();

            // Usuários
            services.AddScoped<ListarUsuariosUseCase>();
            services.AddScoped<ObterUsuarioPorIdUseCase>();
            services.AddScoped<CriarUsuarioUseCase>();
            services.AddScoped<AtualizarUsuarioUseCase>();
            services.AddScoped<ExcluirUsuarioUseCase>();

            return services;
        }
    }
}