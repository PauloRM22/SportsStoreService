using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace StoreService.IntegrationTests.Helpers;

public static class HttpClientAuthExtensions
{
    public static void SetBearer(this HttpClient client, string token) =>
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    public static async Task<string> BootstrapAdminAndLoginAsync(this HttpClient client)
    {
        var bootstrap = new
        {
            Nome = "Admin Test",
            Email = "admin@test.local",
            Senha = "P@ssw0rd!",
            TipoUsuario = "Administrador"
        };

        await client.PostAsJsonAsync("/api/usuarios/bootstrap", bootstrap);

        var loginResp = await client.PostAsJsonAsync("/api/Auth/login",
            new { Email = bootstrap.Email, Senha = bootstrap.Senha });
        loginResp.EnsureSuccessStatusCode();

        var loginJson = await loginResp.Content.ReadAsStringAsync();
        return JsonDocument.Parse(loginJson).RootElement.GetProperty("token").GetString()!;
    }

    public static async Task<(string token, string id)> CreateVendorAndLoginAsync(this HttpClient client, string adminToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var vendorEmail = "vendedor1@test.local";
        var vendorSenha = "Vendor#123";

        // cria vendedor
        var createVendor = new
        {
            Nome = "Vendedor 1",
            Email = vendorEmail,
            Senha = vendorSenha,
            TipoUsuario = "Vendedor"
        };

        var createResp = await client.PostAsJsonAsync("/api/usuarios", createVendor);
        createResp.EnsureSuccessStatusCode();

        var json = await createResp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var id = doc.RootElement.GetProperty("id").GetString()!;

        // login vendedor
        var loginResp = await client.PostAsJsonAsync("/api/Auth/login", new { Email = vendorEmail, Senha = vendorSenha });
        loginResp.EnsureSuccessStatusCode();

        var loginJson = await loginResp.Content.ReadAsStringAsync();
        var token = JsonDocument.Parse(loginJson).RootElement.GetProperty("token").GetString()!;

        return (token, id);
    }
}