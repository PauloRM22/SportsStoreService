using FluentValidation;
using System.Net.Mail;
using StoreService.Domain.Enums;
using StoreService.Application.Usuarios.DTOs; // <-- add

namespace StoreService.Application.Validations.Usuarios
{
    public class CreateUsuarioRequestValidator : AbstractValidator<CreateUsuarioRequest>
    {
        public CreateUsuarioRequestValidator()
        {
            RuleFor(x => x.Nome)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .Must(v => !string.IsNullOrWhiteSpace(v?.Trim()))
                    .WithMessage("Nome é obrigatório")
                .Must(v => (v ?? string.Empty).Trim().Length <= 100)
                    .WithMessage("Nome deve ter no máximo 100 caracteres.");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("E-mail é obrigatório")
                .Must(IsValidEmailTrimmed)
                    .WithMessage("E-mail inválido.");

            RuleFor(x => x.Senha)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Senha é obrigatória")
                .Must(v => !string.IsNullOrWhiteSpace(v?.Trim()))
                    .WithMessage("Senha é obrigatória")
                .Must(v => (v ?? string.Empty).Trim().Length >= 6)
                    .WithMessage("Senha deve ter no mínimo 6 caracteres.");

            RuleFor(x => x.TipoUsuario)
                .IsInEnum()
                .WithMessage("Tipo de usuário inválido. Use 'Administrador' ou 'Vendedor'.");
        }

        private static bool IsValidEmailTrimmed(string? email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var e = email.Trim().ToLowerInvariant();
            try
            {
                var addr = new MailAddress(e);
                return addr.Address == e;
            }
            catch { return false; }
        }
    }
}
