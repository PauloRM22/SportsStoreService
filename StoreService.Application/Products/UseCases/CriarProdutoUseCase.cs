using StoreService.Application.Products.DTOs;
using StoreService.Application.Products.Interfaces;
using StoreService.Domain.Entities;

namespace StoreService.Application.Products.UseCases
{
    public class CriarProdutoUseCase
    {
        private readonly IProdutoRepository _repo;
        public CriarProdutoUseCase(IProdutoRepository repo) => _repo = repo;

        public async Task<ProdutoResponse> ExecuteAsync(CreateProdutoRequest input, CancellationToken ct = default)
        {
            if (await _repo.ExistsByNomeAsync(input.Nome, null, ct))
                throw new ArgumentException($"Já existe um produto com o nome '{input.Nome}'.");

            var entity = new Produto
            {
                Id = Guid.NewGuid(),
                Nome = input.Nome.Trim(),
                Descricao = string.IsNullOrWhiteSpace(input.Descricao) ? null : input.Descricao.Trim(),
                Preco = input.Preco,
                Ativo = true,
                CriadoEm = DateTime.UtcNow
            };

            entity = await _repo.AddAsync(entity, ct);
            return Map(entity);
        }

        private static ProdutoResponse Map(Produto p) => new()
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