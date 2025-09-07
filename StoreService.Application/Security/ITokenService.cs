using StoreService.Domain.Entities;

namespace StoreService.Application.Security
{
    public interface ITokenService
    {
        string Generate(Usuario usuario, out DateTime expiresAtUtc);
    }
}