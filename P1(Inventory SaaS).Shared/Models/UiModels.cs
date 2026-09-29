namespace P1_Inventory_SaaS_.Shared.Models;

public sealed record DashboardSummary(
    decimal InventoryValue,
    int TotalSkus,
    int LowStock,
    int AvailableUnits,
    int IncomingUnits);

public sealed record StockMovementRow(
    string Reference,
    string Product,
    string Sku,
    string Warehouse,
    string Type,
    int Quantity,
    string User,
    DateTimeOffset OccurredAt);

public sealed record ProductRow(
    string Id,
    string Name,
    string Sku,
    string Category,
    string Warehouse,
    int OnHand,
    int Reserved,
    int Available,
    decimal AverageCost,
    decimal SellingPrice,
    string Status,
    string ImageUrl);

public sealed record InventoryRow(
    string Product,
    string Sku,
    string Warehouse,
    string Location,
    int OnHand,
    int Reserved,
    int Available,
    int Incoming,
    int ReorderPoint,
    decimal AverageCost,
    decimal StockValue,
    string Status);

public sealed record TransferRow(
    string Number,
    string From,
    string To,
    int Items,
    int Quantity,
    DateOnly Created,
    DateOnly Expected,
    string Status);

public sealed record PurchaseOrderRow(
    string Number,
    string Supplier,
    string Warehouse,
    DateOnly OrderDate,
    DateOnly ExpectedDate,
    int Items,
    decimal Total,
    string Status);

public sealed record WarehouseRow(
    string Name,
    string Code,
    string City,
    int ProductCount,
    int Units,
    decimal Value,
    string Manager,
    string Status);

public sealed record UserRow(
    string Name,
    string Email,
    string Role,
    string Warehouse,
    DateTimeOffset LastActive,
    bool Mfa,
    string Status);

public sealed record ProductDraft(
    string Name,
    string Sku,
    string Category,
    string Warehouse,
    decimal AverageCost,
    decimal SellingPrice,
    string? ImageUrl = null);

public sealed record StockTransactionDraft(
    string Type,
    string Product,
    string Sku,
    string Warehouse,
    string Location,
    int Quantity,
    decimal UnitCost,
    string? Reference = null,
    string User = "Huey Ren");

public sealed record TransferDraft(
    string From,
    string To,
    int Quantity,
    DateOnly Expected,
    int Items = 1);

public sealed record PurchaseOrderDraft(
    string Supplier,
    string Warehouse,
    int Quantity,
    decimal UnitCost,
    DateOnly ExpectedDate,
    int Items = 1);
