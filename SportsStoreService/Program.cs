using SportsStoreService.Extensions;

namespace SportsStoreService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Infra do projeto (DB, EF, repos, etc.)
            builder.Services.AddSportsStore(builder.Configuration);

            // Controllers
            builder.Services.AddControllers();

            // Swagger com esquema Bearer + Autenticação/Autorização via JWT (policies Admin/Vendedor)
            builder.Services.AddSwaggerWithJwt();
            builder.Services.AddJwtAuth(builder.Configuration);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}