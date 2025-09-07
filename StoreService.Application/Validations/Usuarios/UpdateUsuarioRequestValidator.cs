using FluentValidation;
using System.Net.Mail;
using StoreService.Application.Usuarios.DTOs;

namespace StoreService.Application.Validations.Usuarios
{
    public class UpdateUsuarioRequestValidator : AbstractValidator<UpdateUsuarioRequest>
    {
        public UpdateUsuarioRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id do usuário é obrigatório.");

            RuleFor(x => x.Nome)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Nome é obrigatório.")
                .Must(v => !string.IsNullOrWhiteSpace(v?.Trim()))
                    .WithMessage("Nome é obrigatório.")
                .Must(v => (v ?? string.Empty).Trim().Length <= 100)
                    .WithMessage("Nome deve ter no máximo 100 caracteres.");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("E-mail é obrigatório.")
                .Must(IsValidEmailTrimmed).WithMessage("E-mail inválido.");
        }

        private static bool IsValidEmailTrimmed(string? email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var e = email.Trim().ToLowerInvariant();
            try { return new MailAddress(e).Address == e; }
            catch { return false; }
        }
    }
}