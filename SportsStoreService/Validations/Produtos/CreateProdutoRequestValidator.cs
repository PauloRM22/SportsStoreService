using FluentValidation;
using StoreService.Application.Products.DTOs;
using StoreService.Application.Products.Interfaces;

namespace SportsStoreService.Validations.Produtos
{
    public class CreateProdutoRequestValidator : AbstractValidator<CreateProdutoRequest>
    {
        public CreateProdutoRequestValidator(IProdutoRepository repo)
        {
            RuleFor(x => x.Nome)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Nome é obrigatório.")
                .Must(s => !string.IsNullOrWhiteSpace(s))
                    .WithMessage("Nome é obrigatório.")
                .MaximumLength(120)
                // Checagem de unicidade no repositório (sem Transform; fazemos Trim local)
                .MustAsync(async (model, nome, ct) =>
                {
                    var candidato = nome?.Trim() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(candidato)) return false;
                    // Para criação, ignoreId = null
                    var existe = await repo.ExistsByNomeAsync(candidato, null, ct);
                    return !existe;
                })
                .WithMessage(x => $"Já existe um produto com o nome '{x.Nome?.Trim()}'.");

            // Se houver preço:
            RuleFor(x => x.Preco)
                .NotNull().WithMessage("Preço é obrigatório.")
                .GreaterThan(0).WithMessage("Preço deve ser maior que zero.");
        }
    }
}