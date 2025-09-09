using FluentAssertions;
using Moq;
using StoreService.Application.Security;
using StoreService.Application.Usuarios.DTOs;
using StoreService.Application.Usuarios.Interfaces;
using StoreService.Application.Usuarios.UseCases;
using StoreService.Domain.Entities;
using StoreService.Domain.Enums;

namespace StoreService.Tests.UseCases;

public class UsuariosUseCasesTests
{
    [Fact]
    public async Task CriarUsuario_Email_ja_existe_deve_lancar()
    {
        var repo = new Mock<IUsuarioRepository>();
        var hasher = new Mock<IPasswordHasher>();
        var use = new CriarUsuarioUseCase(repo.Object, hasher.Object);

        var req = new CreateUsuarioRequest
        {
            Nome = "Admin",
            Email = "admin@dominio.com",
            Senha = "segredo",
            TipoUsuario = TipoUsuario.Administrador
        };

        repo.Setup(r => r.GetByEmailAsync(req.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Usuario());

        await Assert.ThrowsAsync<ArgumentException>(() => use.ExecuteAsync(req, default));
    }

    [Fact]
    public async Task CriarUsuario_Deve_hashear_senha_e_persistir()
    {
        var repo = new Mock<IUsuarioRepository>();
        var hasher = new Mock<IPasswordHasher>();
        var use = new CriarUsuarioUseCase(repo.Object, hasher.Object);

        var req = new CreateUsuarioRequest
        {
            Nome = "Admin",
            Email = "admin@dominio.com",
            Senha = "segredo",
            TipoUsuario = TipoUsuario.Administrador
        };

        // Hash retorna byte[]:
        var expectedHash = new byte[] { 1, 2, 3, 4 };
        hasher.Setup(h => h.Hash("segredo")).Returns(expectedHash);

        // CreateAsync retorna Task (sem resultado). Capturamos o usuário salvo no Callback.
        Usuario? salvo = null;
        repo.Setup(r => r.CreateAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
            .Callback<Usuario, CancellationToken>((u, _) => salvo = u)
            .Returns(Task.CompletedTask);

        var resp = await use.ExecuteAsync(req, default);

        hasher.Verify(h => h.Hash("segredo"), Times.Once);
        repo.Verify(r => r.CreateAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()), Times.Once);

        // Verificações sobre o objeto persistido:
        salvo.Should().NotBeNull();
        salvo!.Email.Should().Be("admin@dominio.com");
        salvo.SenhaHash.Should().BeEquivalentTo(expectedHash);

        resp.Email.Should().Be("admin@dominio.com");
    }
}