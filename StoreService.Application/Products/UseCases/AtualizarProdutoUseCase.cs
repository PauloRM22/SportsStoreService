using StoreService.Application.Products.DTOs;
using StoreService.Application.Products.Interfaces;

namespace StoreService.Application.Products.UseCases
{
    public class AtualizarProdutoUseCase
    {
        private readonly IProdutoRepository _repo;
        public AtualizarProdutoUseCase(IProdutoRepository repo) => _repo = repo;

        public async Task<ProdutoResponse> ExecuteAsync(UpdateProdutoRequest input, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(input.Id, ct)
                ?? throw new ArgumentException("Produto não encontrado.");

            if (await _repo.ExistsByNomeAsync(input.Nome, input.Id, ct))
                throw new ArgumentException($"Já existe um produto com o nome '{input.Nome}'.");

            entity.Nome = input.Nome.Trim();
            entity.Descricao = string.IsNullOrWhiteSpace(input.Descricao) ? null : input.Descricao.Trim();
            entity.Preco = input.Preco;
            entity.Ativo = input.Ativo;
            entity.AtualizadoEm = DateTime.UtcNow;

            await _repo.UpdateAsync(entity, ct);
            return new ProdutoResponse
            {
                Id = entity.Id,
                Codigo = entity.Codigo,
                Nome = entity.Nome,
                Descricao = entity.Descricao,
                Preco = entity.Preco,
                Ativo = entity.Ativo,
                CriadoEm = entity.CriadoEm,
                AtualizadoEm = entity.AtualizadoEm
            };
        }
    }
}