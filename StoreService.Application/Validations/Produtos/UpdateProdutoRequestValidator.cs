using FluentValidation;
using StoreService.Application.Produtos.DTOs;

namespace StoreService.Application.Validations.Produtos
{
    public class UpdateProdutoRequestValidator : AbstractValidator<UpdateProdutoRequest>
    {
        public UpdateProdutoRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id do produto é obrigatório.");

            RuleFor(x => x.Nome)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Nome é obrigatório.")
                .Must(s => !string.IsNullOrWhiteSpace(s))
                    .WithMessage("Nome é obrigatório.")
                .MaximumLength(150);

            RuleFor(x => x.Descricao)
                .MaximumLength(1000);

            RuleFor(x => x.Preco)
                .NotNull().WithMessage("Preço é obrigatório.")
                .GreaterThan(0).WithMessage("Preço deve ser maior que zero.");
        }
    }
}