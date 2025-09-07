using StoreService.Application.Produtos.DTOs;
using StoreService.Application.Produtos.Interfaces;

namespace StoreService.Application.Produtos.UseCases
{
    public class ListarProdutosUseCase
    {
        private readonly IProdutoRepository _repo;
        public ListarProdutosUseCase(IProdutoRepository repo) => _repo = repo;

        public async Task<List<ProdutoResponse>> ExecuteAsync(CancellationToken ct = default)
        {
            var list = await _repo.GetAllAsync(ct);
            return list
                .OrderBy(p => p.Nome)
                .Select(p => new ProdutoResponse
                {
                    Id = p.Id,
                    Codigo = p.Codigo,
                    Nome = p.Nome,
                    Descricao = p.Descricao,
                    Preco = p.Preco,
                    Ativo = p.Ativo,
                    CriadoEm = p.CriadoEm,
                    AtualizadoEm = p.AtualizadoEm
                })
                .ToList();
        }
    }
}