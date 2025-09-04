using StoreService.Domain.Entities;

namespace StoreService.Application.Estoque.Interfaces
{
    public interface IEstoqueRepository
    {
        Task<(MovimentacaoEstoque mov, int saldoAtual, int produtoCodigo)> RegistrarEntradaAsync(
            Guid produtoId, int quantidade, string numeroNotaFiscal, string? observacao, CancellationToken ct);

        Task<(int saldo, int produtoCodigo, DateTime atualizadoEm)> ObterSaldoAsync(
            Guid produtoId, CancellationToken ct);
    }
}