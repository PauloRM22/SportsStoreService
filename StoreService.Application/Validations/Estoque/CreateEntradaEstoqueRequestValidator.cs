using FluentValidation;
using StoreService.Application.Estoque.DTOs;

namespace StoreService.Application.Validations.Estoque
{
    public class CreateEntradaEstoqueRequestValidator : AbstractValidator<CreateEntradaEstoqueRequest>
    {
        public CreateEntradaEstoqueRequestValidator()
        {
            RuleFor(x => x.ProdutoId).NotEmpty();
            RuleFor(x => x.Quantidade).GreaterThan(0);
            RuleFor(x => x.NumeroNotaFiscal).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Observacao).MaximumLength(250).When(x => !string.IsNullOrWhiteSpace(x.Observacao));
        }
    }
}