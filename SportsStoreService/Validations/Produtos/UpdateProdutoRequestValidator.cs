using FluentValidation;
using StoreService.Application.Products.DTOs;
using StoreService.Application.Products.Interfaces;

namespace SportsStoreService.Validations.Produtos
{
    public class UpdateProdutoRequestValidator : AbstractValidator<UpdateProdutoRequest>
    {
        public UpdateProdutoRequestValidator(IProdutoRepository repo)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id do produto é obrigatório.")
                .MustAsync(async (id, ct) =>
                {
                    var existente = await repo.GetByIdAsync(id, ct);
                    return existente is not null;
                })
                .WithMessage("Produto não encontrado.");

            RuleFor(x => x.Nome)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Nome é obrigatório.")
                .Must(s => !string.IsNullOrWhiteSpace(s))
                    .WithMessage("Nome é obrigatório.")
                .MaximumLength(120)
                .MustAsync(async (model, nome, ct) =>
                {
                    var candidato = nome?.Trim() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(candidato)) return false;

                    // para update: passa o Id atual para ser ignorado na checagem
                    var existe = await repo.ExistsByNomeAsync(candidato, model.Id, ct);
                    return !existe;
                })
                .WithMessage(m => $"Já existe um produto com o nome '{m.Nome?.Trim()}'.");
        }
    }
}