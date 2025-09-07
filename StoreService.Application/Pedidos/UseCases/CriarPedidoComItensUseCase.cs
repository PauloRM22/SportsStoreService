using StoreService.Application.Pedidos.DTOs;
using StoreService.Application.Pedidos.Interfaces;
using StoreService.Domain.Entities;

namespace StoreService.Application.Pedidos.UseCases
{
    public class CriarPedidoComItensUseCase
    {
        private readonly IPedidoRepository _repo;

        public CriarPedidoComItensUseCase(IPedidoRepository repo) => _repo = repo;

        public async Task<PedidoResponse> ExecuteAsync(CreatePedidoRequest request, Guid vendedorId, CancellationToken ct)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            var pedido = new Pedido
            {
                Id = Guid.NewGuid(),
                ClienteDocumento = request.ClienteDocumento?.Trim() ?? string.Empty,
                VendedorId = vendedorId,
                VendedorNome = request.VendedorNome?.Trim() ?? string.Empty,
                CriadoEm = DateTime.UtcNow
            };

            var itens = request.Itens.Select(i => new PedidoItem
            {
                Id = Guid.NewGuid(),
                ProdutoId = i.ProdutoId,
                Quantidade = i.Quantidade
            }).ToList();

            var criado = await _repo.CriarComItensAsync(pedido, itens, ct);
            return Map(criado);
        }

        private static PedidoResponse Map(Pedido p) => new()
        {
            Id = p.Id,
            Codigo = p.Codigo,
            ClienteDocumento = p.ClienteDocumento,
            VendedorId = p.VendedorId,          // ✅ vai casar com Guid? no DTO
            VendedorNome = p.VendedorNome,
            ValorTotal = p.ValorTotal,
            Status = p.Status,
            CriadoEm = p.CriadoEm,
            AtualizadoEm = p.AtualizadoEm,
            Itens = p.Itens.Select(i => new PedidoItemResponse
            {
                Id = i.Id,
                ProdutoId = i.ProdutoId,
                ProdutoCodigo = i.Produto?.Codigo ?? 0,    // ✅ vem do Include feito no repo
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario,
                Subtotal = i.Quantidade * i.PrecoUnitario  // ✅
            }).ToList()
        };
    }
}