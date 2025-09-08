using MassTransit;
using SportsStoreService.Extensions;                   // AddSportsStore, AddSwaggerWithJwt, AddJwtAuth
using SportsStoreService.Middlewares;
using StoreService.Infrastructure.DependencyInjection;                  // ExceptionMiddleware  (ajuste se o namespace for outro)

namespace SportsStoreService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1) Serviços da aplicação (use cases, etc.)
            builder.Services.AddSportsStore(builder.Configuration);

            // 2) Infraestrutura (DbContext + Repositórios)
            builder.Services.AddInfrastructure(builder.Configuration);            

            // 4) Swagger + JWT (esquema de segurança e auth)
            builder.Services.AddSwaggerWithJwt();
            builder.Services.AddJwtAuth(builder.Configuration);

            var app = builder.Build();

            // 5) Swagger só em Development (mantenho seu comportamento atual)
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // 6) Middleware global de exceções (ProblemDetails)
            //    Coloque ANTES de Authentication/Authorization para capturar tudo
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            // 7) Auth
            app.UseAuthentication();
            app.UseAuthorization();

            // 8) Endpoints
            app.MapControllers();

            app.UseDatabaseMigrations();

            app.Run();
        }
    }
}