using StoreService.Application.Produtos.DTOs;
using StoreService.Application.Produtos.Interfaces;
using StoreService.Domain.Entities;

namespace StoreService.Application.Produtos.UseCases
{
    public class CriarProdutoUseCase
    {
        private readonly IProdutoRepository _repo;
        public CriarProdutoUseCase(IProdutoRepository repo) => _repo = repo;

        public async Task<ProdutoResponse> ExecuteAsync(CreateProdutoRequest input, CancellationToken ct = default)
        {
            if (input is null) throw new ArgumentNullException(nameof(input));

            var nome = input.Nome?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome é obrigatório.", nameof(input.Nome));

            if (await _repo.ExistsByNomeAsync(nome, ct))
                throw new ArgumentException($"Já existe um produto com o nome '{nome}'.");

            var entity = new Produto
            {
                Id = Guid.NewGuid(),
                Nome = nome,
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