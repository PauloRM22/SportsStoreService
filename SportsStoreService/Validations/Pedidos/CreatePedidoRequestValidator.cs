using FluentValidation;
using StoreService.Application.Pedidos.DTOs;
using System.Linq;

namespace SportsStoreService.Validations.Pedidos
{ 
    public class CreatePedidoRequestValidator : AbstractValidator<CreatePedidoRequest>
    {
        public CreatePedidoRequestValidator()
        {
            RuleFor(x => x.ClienteDocumento)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Documento do cliente é obrigatório.")
                .Must(v => IsCpfOrCnpj(v?.Trim()))
                    .WithMessage("Documento do cliente deve ser CPF (11 dígitos) ou CNPJ (14 dígitos).");

            RuleFor(x => x.VendedorNome)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Nome do vendedor é obrigatório.")
                // aplica limite considerando Trim
                .Must(v => (v ?? string.Empty).Trim().Length <= 120)
                    .WithMessage("Nome do vendedor deve ter no máximo 120 caracteres.");

            RuleFor(x => x.Itens)
                .NotEmpty().WithMessage("O pedido deve conter ao menos um item.");

            RuleForEach(x => x.Itens).ChildRules(it =>
            {
                it.RuleFor(i => i.ProdutoId)
                    .NotEmpty().WithMessage("ProdutoId é obrigatório.");

                it.RuleFor(i => i.Quantidade)
                    .GreaterThan(0).WithMessage("Quantidade do item deve ser maior que zero.");
            });
        }

        private static bool IsCpfOrCnpj(string? doc)
        {
            if (string.IsNullOrWhiteSpace(doc)) return false;
            var digits = new string(doc.Trim().Where(char.IsDigit).ToArray());
            return digits.Length == 11 || digits.Length == 14;
        }
    }
}