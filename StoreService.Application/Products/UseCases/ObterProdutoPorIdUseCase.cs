using StoreService.Application.Products.DTOs;
using StoreService.Application.Products.Interfaces;

namespace StoreService.Application.Products.UseCases
{
    public class ObterProdutoPorIdUseCase
    {
        private readonly IProdutoRepository _repo;
        public ObterProdutoPorIdUseCase(IProdutoRepository repo) => _repo = repo;

        public async Task<ProdutoResponse> ExecuteAsync(Guid id, CancellationToken ct = default)
        {
            var p = await _repo.GetByIdAsync(id, ct)
                ?? throw new ArgumentException("Produto não encontrado.");

            return new ProdutoResponse
            {
                Id = p.Id,
                Codigo = p.Codigo,
                Nome = p.Nome,
                Descricao = p.Descricao,
                Preco = p.Preco,
                Ativo = p.Ativo,
                CriadoEm = p.CriadoEm,
                AtualizadoEm = p.AtualizadoEm
            };
        }
    }
}