using P1_Inventory_SaaS_.Shared.Models;

namespace P1_Inventory_SaaS_.Shared.Services;

public interface IInventoryUiService
{
    Task<DashboardSummary> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StockMovementRow>> GetRecentMovementsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductRow>> GetProductsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InventoryRow>> GetInventoryAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TransferRow>> GetTransfersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PurchaseOrderRow>> GetPurchaseOrdersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WarehouseRow>> GetWarehousesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserRow>> GetUsersAsync(CancellationToken cancellationToken = default);
    Task<ProductRow?> GetProductAsync(string id, CancellationToken cancellationToken = default);
    Task<ProductRow> CreateProductAsync(ProductDraft draft, CancellationToken cancellationToken = default);
    Task<ProductRow?> UpdateProductAsync(string id, ProductDraft draft, CancellationToken cancellationToken = default);
    Task<ProductRow?> DuplicateProductAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> DeleteProductAsync(string id, CancellationToken cancellationToken = default);
    Task<StockMovementRow> PostStockTransactionAsync(StockTransactionDraft draft, CancellationToken cancellationToken = default);
    Task<TransferRow> CreateTransferAsync(TransferDraft draft, CancellationToken cancellationToken = default);
    Task<bool> SetTransferStatusAsync(string number, string status, CancellationToken cancellationToken = default);
    Task<PurchaseOrderRow> CreatePurchaseOrderAsync(PurchaseOrderDraft draft, CancellationToken cancellationToken = default);
    Task<bool> SetPurchaseOrderStatusAsync(string number, string status, CancellationToken cancellationToken = default);
}
