using StoreService.Application.Users.DTOs;
using StoreService.Domain.Entities;

namespace StoreService.Application.Users
{
    public static class UsuarioMapper
    {
        public static UsuarioResponse ToResponse(this Usuario u) => new UsuarioResponse
        {
            Id = u.Id,
            Codigo = u.Codigo,
            Nome = u.Nome,
            Email = u.Email,
            TipoUsuario = u.TipoUsuario,
            CriadoEm = u.CriadoEm,
            AtualizadoEm = u.AtualizadoEm
        };
    }
}