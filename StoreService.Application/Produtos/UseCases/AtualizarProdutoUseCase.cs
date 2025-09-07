using StoreService.Application.Produtos.DTOs;
using StoreService.Application.Produtos.Interfaces;

namespace StoreService.Application.Produtos.UseCases
{
    public class AtualizarProdutoUseCase
    {
        private readonly IProdutoRepository _repo;
        public AtualizarProdutoUseCase(IProdutoRepository repo) => _repo = repo;

        public async Task<ProdutoResponse> ExecuteAsync(UpdateProdutoRequest input, CancellationToken ct = default)
        {
            if (input is null) throw new ArgumentNullException(nameof(input));

            var entity = await _repo.GetByIdAsync(input.Id, ct)
                ?? throw new ArgumentException("Produto não encontrado.");

            var novoNome = input.Nome?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(novoNome))
                throw new ArgumentException("Nome é obrigatório.", nameof(input.Nome));

            // Só checa duplicidade se o nome foi alterado
            if (!string.Equals(entity.Nome, novoNome, StringComparison.Ordinal))
            {
                if (await _repo.ExistsByNomeAsync(novoNome, ct))
                    throw new ArgumentException($"Já existe um produto com o nome '{novoNome}'.");
            }

            entity.Nome = novoNome;
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
