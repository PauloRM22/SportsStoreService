using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StoreService.Domain.Entities;

namespace StoreService.Application.Security
{
    public interface ITokenService
    {
        string Generate(Usuario usuario, out DateTime expiresAtUtc);
    }

    public class TokenService : ITokenService
    {
        private readonly JwtOptions _opts;

        public TokenService(JwtOptions opts)
        {
            _opts = opts;
        }

        public string Generate(Usuario usuario, out DateTime expiresAtUtc)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, usuario.Email),
                new(ClaimTypes.Name, usuario.Nome),
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(ClaimTypes.Role, usuario.TipoUsuario.ToString())
            };

            var creds = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opts.Key)),
                SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;
            expiresAtUtc = now.AddMinutes(_opts.ExpiresMinutes);

            var token = new JwtSecurityToken(
                issuer: _opts.Issuer,
                audience: _opts.Audience,
                claims: claims,
                notBefore: now,
                expires: expiresAtUtc,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}