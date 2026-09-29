using P1_Inventory_SaaS_.Shared.Models;

namespace P1_Inventory_SaaS_.Shared.Services;

public sealed class MockInventoryUiService : IInventoryUiService
{
    private readonly object gate = new();
    private int productSequence = 6;
    private int movementSequence = 4_821;
    private int transferSequence = 1_294;
    private int purchaseOrderSequence = 3_821;
    private int availableUnitDelta;
    private int incomingUnitDelta;
    private decimal inventoryValueDelta;

    private readonly List<StockMovementRow> movements =
    [
        new("GRN-004821", "USB-C Hub 8-in-1", "HUB-8IN1-GR", "KL Central", "Stock In", 240, "Sarah Lim", DateTimeOffset.Now.AddMinutes(-18)),
        new("TR-001294", "Running Tee / Black / M", "TEE-BLK-M", "PJ Warehouse", "Transfer Out", -80, "Daniel Tan", DateTimeOffset.Now.AddMinutes(-42)),
        new("ADJ-000829", "Wireless Scanner S2", "SCN-S2", "KL Central", "Adjustment", -3, "Aina Yusuf", DateTimeOffset.Now.AddHours(-1.4)),
        new("SO-009283", "Thermal Label 100x150", "LBL-100150", "Penang", "Stock Out", -120, "System", DateTimeOffset.Now.AddHours(-2.2)),
        new("GRN-004817", "Packing Tape 48mm", "TAPE-48", "Johor", "Stock In", 600, "Marcus Lee", DateTimeOffset.Now.AddHours(-3.0))
    ];

    private readonly List<ProductRow> products =
    [
        new("p1","USB-C Hub 8-in-1","HUB-8IN1-GR","Electronics","KL Central",420,35,385,68.50m,109.00m,"In Stock","_content/StockPilot.Shared/images/products/hub.svg"),
        new("p2","Wireless Scanner S2","SCN-S2","Hardware","KL Central",18,6,12,145.00m,219.00m,"Low Stock","_content/StockPilot.Shared/images/products/scanner.svg"),
        new("p3","Thermal Label 100x150","LBL-100150","Consumables","Penang",860,140,720,0.19m,0.35m,"In Stock","_content/StockPilot.Shared/images/products/label.svg"),
        new("p4","Packing Tape 48mm","TAPE-48","Packaging","Johor",1200,90,1110,2.20m,4.20m,"In Stock","_content/StockPilot.Shared/images/products/tape.svg"),
        new("p5","Running Tee / Black / M","TEE-BLK-M","Apparel","PJ Warehouse",62,20,42,24.00m,59.00m,"In Stock","_content/StockPilot.Shared/images/products/tee.svg"),
        new("p6","Receipt Roll 80mm","ROLL-80","Consumables","KL Central",0,0,0,1.10m,2.60m,"Out of Stock","_content/StockPilot.Shared/images/products/roll.svg")
    ];

    private readonly List<InventoryRow> inventory =
    [
        new("USB-C Hub 8-in-1","HUB-8IN1-GR","KL Central","A-01-02",420,35,385,240,100,68.50m,28770m,"In Stock"),
        new("Wireless Scanner S2","SCN-S2","KL Central","B-02-01",18,6,12,0,20,145m,2610m,"Low Stock"),
        new("Thermal Label 100x150","LBL-100150","Penang","C-04-08",860,140,720,400,300,0.19m,163.40m,"In Stock"),
        new("Receipt Roll 80mm","ROLL-80","KL Central","C-05-01",0,0,0,500,120,1.10m,0m,"Out of Stock")
    ];

    private readonly List<TransferRow> transfers =
    [
        new("TR-001294","KL Central","PJ Warehouse",4,180,DateOnly.FromDateTime(DateTime.Today),DateOnly.FromDateTime(DateTime.Today.AddDays(1)),"In Transit"),
        new("TR-001293","Penang","Johor",2,75,DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),DateOnly.FromDateTime(DateTime.Today),"Received"),
        new("TR-001292","KL Central","Penang",7,420,DateOnly.FromDateTime(DateTime.Today.AddDays(-2)),DateOnly.FromDateTime(DateTime.Today),"Picking"),
        new("TR-001291","PJ Warehouse","KL Central",3,95,DateOnly.FromDateTime(DateTime.Today.AddDays(-3)),DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),"Completed")
    ];

    private readonly List<PurchaseOrderRow> purchaseOrders =
    [
        new("PO-2026-003821","TechSource Distribution","KL Central",DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),DateOnly.FromDateTime(DateTime.Today.AddDays(4)),8,48650m,"Approved"),
        new("PO-2026-003820","PackPro Supplies","Johor",DateOnly.FromDateTime(DateTime.Today.AddDays(-2)),DateOnly.FromDateTime(DateTime.Today.AddDays(2)),4,12240m,"Sent"),
        new("PO-2026-003819","Northern Labels","Penang",DateOnly.FromDateTime(DateTime.Today.AddDays(-3)),DateOnly.FromDateTime(DateTime.Today.AddDays(1)),3,8210m,"Partially Received"),
        new("PO-2026-003818","Urban Apparel Manufacturing","PJ Warehouse",DateOnly.FromDateTime(DateTime.Today.AddDays(-4)),DateOnly.FromDateTime(DateTime.Today.AddDays(5)),12,67490m,"Pending Approval")
    ];

    public Task<DashboardSummary> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
    {
        lock (gate)
        {
            return Task.FromResult(new DashboardSummary(
                InventoryValue: 1_284_520m + inventoryValueDelta,
                TotalSkus: 8_642 + Math.Max(0, products.Count - 6),
                LowStock: 126 + products.Count(x => x.Status == "Low Stock") - 1,
                AvailableUnits: 48_390 + availableUnitDelta,
                IncomingUnits: 6_240 + incomingUnitDelta));
        }
    }

    public Task<IReadOnlyList<StockMovementRow>> GetRecentMovementsAsync(CancellationToken cancellationToken = default)
    {
        lock (gate)
            return Task.FromResult<IReadOnlyList<StockMovementRow>>(movements.ToList());
    }

    public Task<IReadOnlyList<ProductRow>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        lock (gate)
            return Task.FromResult<IReadOnlyList<ProductRow>>(products.ToList());
    }

    public Task<IReadOnlyList<InventoryRow>> GetInventoryAsync(CancellationToken cancellationToken = default)
    {
        lock (gate)
            return Task.FromResult<IReadOnlyList<InventoryRow>>(inventory.ToList());
    }

    public Task<IReadOnlyList<TransferRow>> GetTransfersAsync(CancellationToken cancellationToken = default)
    {
        lock (gate)
            return Task.FromResult<IReadOnlyList<TransferRow>>(transfers.ToList());
    }

    public Task<IReadOnlyList<PurchaseOrderRow>> GetPurchaseOrdersAsync(CancellationToken cancellationToken = default)
    {
        lock (gate)
            return Task.FromResult<IReadOnlyList<PurchaseOrderRow>>(purchaseOrders.ToList());
    }

    public Task<IReadOnlyList<WarehouseRow>> GetWarehousesAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<WarehouseRow> rows =
        [
            new("KL Central Warehouse","WH-KL01","Kuala Lumpur",4630,22480,642300m,"Sarah Lim","Active"),
            new("PJ Fulfilment Hub","WH-PJ01","Petaling Jaya",2410,13220,328900m,"Daniel Tan","Active"),
            new("Penang Warehouse","WH-PG01","Bayan Lepas",1850,8240,190120m,"Lee Wei Jian","Active"),
            new("Johor Warehouse","WH-JB01","Johor Bahru",1480,4450,123200m,"Marcus Lee","Active")
        ];
        return Task.FromResult(rows);
    }

    public Task<IReadOnlyList<UserRow>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<UserRow> rows =
        [
            new("Huey Ren","huey@example.com","Administrator","All warehouses",DateTimeOffset.Now,true,"Active"),
            new("Sarah Lim","sarah@example.com","Inventory Manager","KL Central",DateTimeOffset.Now.AddMinutes(-7),true,"Active"),
            new("Daniel Tan","daniel@example.com","Warehouse Manager","PJ Warehouse",DateTimeOffset.Now.AddHours(-1),true,"Active"),
            new("Aina Yusuf","aina@example.com","Warehouse Staff","KL Central",DateTimeOffset.Now.AddHours(-3),false,"Active"),
            new("Marcus Lee","marcus@example.com","Warehouse Manager","Johor",DateTimeOffset.Now.AddDays(-1),true,"Active")
        ];
        return Task.FromResult(rows);
    }

    public Task<ProductRow?> GetProductAsync(string id, CancellationToken cancellationToken = default)
    {
        lock (gate)
            return Task.FromResult(products.FirstOrDefault(x => x.Id.Equals(id, StringComparison.OrdinalIgnoreCase)));
    }

    public Task<ProductRow> CreateProductAsync(ProductDraft draft, CancellationToken cancellationToken = default)
    {
        lock (gate)
        {
            var id = $"p{++productSequence}";
            var sku = UniqueSku(draft.Sku);
            var row = new ProductRow(
                id,
                Required(draft.Name, "New product"),
                sku,
                Required(draft.Category, "Uncategorized"),
                NormalizeBranch(draft.Warehouse),
                0,
                0,
                0,
                Math.Max(0, draft.AverageCost),
                Math.Max(0, draft.SellingPrice),
                "Out of Stock",
                string.IsNullOrWhiteSpace(draft.ImageUrl) ? "_content/StockPilot.Shared/images/products/hub.svg" : draft.ImageUrl);
            products.Insert(0, row);
            return Task.FromResult(row);
        }
    }

    public Task<ProductRow?> UpdateProductAsync(string id, ProductDraft draft, CancellationToken cancellationToken = default)
    {
        lock (gate)
        {
            var index = products.FindIndex(x => x.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (index < 0)
                return Task.FromResult<ProductRow?>(null);

            var current = products[index];
            var updated = current with
            {
                Name = Required(draft.Name, current.Name),
                Sku = string.IsNullOrWhiteSpace(draft.Sku) ? current.Sku : draft.Sku.Trim().ToUpperInvariant(),
                Category = Required(draft.Category, current.Category),
                Warehouse = NormalizeBranch(draft.Warehouse),
                AverageCost = Math.Max(0, draft.AverageCost),
                SellingPrice = Math.Max(0, draft.SellingPrice),
                ImageUrl = string.IsNullOrWhiteSpace(draft.ImageUrl) ? current.ImageUrl : draft.ImageUrl
            };
            products[index] = updated;

            for (var i = 0; i < inventory.Count; i++)
            {
                if (!inventory[i].Sku.Equals(current.Sku, StringComparison.OrdinalIgnoreCase)) continue;
                inventory[i] = inventory[i] with
                {
                    Product = updated.Name,
                    Sku = updated.Sku,
                    AverageCost = updated.AverageCost,
                    StockValue = inventory[i].OnHand * updated.AverageCost
                };
            }

            return Task.FromResult<ProductRow?>(updated);
        }
    }

    public Task<ProductRow?> DuplicateProductAsync(string id, CancellationToken cancellationToken = default)
    {
        lock (gate)
        {
            var source = products.FirstOrDefault(x => x.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (source is null)
                return Task.FromResult<ProductRow?>(null);

            var duplicate = source with
            {
                Id = $"p{++productSequence}",
                Name = $"{source.Name} copy",
                Sku = UniqueSku($"{source.Sku}-COPY"),
                OnHand = 0,
                Reserved = 0,
                Available = 0,
                Status = "Out of Stock"
            };
            products.Insert(0, duplicate);
            return Task.FromResult<ProductRow?>(duplicate);
        }
    }

    public Task<bool> DeleteProductAsync(string id, CancellationToken cancellationToken = default)
    {
        lock (gate)
        {
            var product = products.FirstOrDefault(x => x.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (product is null)
                return Task.FromResult(false);
            products.Remove(product);
            inventory.RemoveAll(x => x.Sku.Equals(product.Sku, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(true);
        }
    }

    public Task<StockMovementRow> PostStockTransactionAsync(StockTransactionDraft draft, CancellationToken cancellationToken = default)
    {
        lock (gate)
        {
            var warehouse = NormalizeBranch(draft.Warehouse);
            var productIndex = products.FindIndex(x =>
                x.Sku.Equals(draft.Sku, StringComparison.OrdinalIgnoreCase) ||
                x.Name.Equals(draft.Product, StringComparison.OrdinalIgnoreCase));
            if (productIndex < 0)
            {
                var created = new ProductRow(
                    $"p{++productSequence}", Required(draft.Product, "Scanned product"), UniqueSku(draft.Sku),
                    "Uncategorized", warehouse, 0, 0, 0, Math.Max(0, draft.UnitCost), 0, "Out of Stock",
                    "_content/StockPilot.Shared/images/products/hub.svg");
                products.Insert(0, created);
                productIndex = 0;
            }

            var product = products[productIndex];
            var quantity = draft.Quantity;
            if (quantity < 0)
                quantity = -Math.Min(Math.Abs(quantity), product.Available);

            var nextOnHand = Math.Max(0, product.OnHand + quantity);
            var nextAvailable = Math.Max(0, product.Available + quantity);
            var cost = draft.UnitCost > 0 ? draft.UnitCost : product.AverageCost;
            var updatedProduct = product with
            {
                Warehouse = warehouse,
                OnHand = nextOnHand,
                Available = nextAvailable,
                AverageCost = cost,
                Status = StockStatus(nextAvailable)
            };
            products[productIndex] = updatedProduct;

            var inventoryIndex = inventory.FindIndex(x =>
                x.Sku.Equals(updatedProduct.Sku, StringComparison.OrdinalIgnoreCase) &&
                NormalizeBranch(x.Warehouse).Equals(warehouse, StringComparison.OrdinalIgnoreCase));
            if (inventoryIndex >= 0)
            {
                var item = inventory[inventoryIndex];
                var onHand = Math.Max(0, item.OnHand + quantity);
                var available = Math.Max(0, item.Available + quantity);
                inventory[inventoryIndex] = item with
                {
                    Product = updatedProduct.Name,
                    Warehouse = warehouse,
                    Location = Required(draft.Location, item.Location),
                    OnHand = onHand,
                    Available = available,
                    AverageCost = cost,
                    StockValue = onHand * cost,
                    Status = StockStatus(available)
                };
            }
            else
            {
                inventory.Insert(0, new InventoryRow(
                    updatedProduct.Name, updatedProduct.Sku, warehouse, Required(draft.Location, "Receiving"),
                    nextOnHand, 0, nextAvailable, 0, 20, cost, nextOnHand * cost, StockStatus(nextAvailable)));
            }

            var reference = string.IsNullOrWhiteSpace(draft.Reference)
                ? $"{MovementPrefix(draft.Type)}-{++movementSequence:000000}"
                : draft.Reference.Trim();
            var movement = new StockMovementRow(reference, updatedProduct.Name, updatedProduct.Sku, warehouse,
                Required(draft.Type, "Adjustment"), quantity, Required(draft.User, "Huey Ren"), DateTimeOffset.Now);
            movements.Insert(0, movement);
            availableUnitDelta += quantity;
            inventoryValueDelta += quantity * cost;
            return Task.FromResult(movement);
        }
    }

    public Task<TransferRow> CreateTransferAsync(TransferDraft draft, CancellationToken cancellationToken = default)
    {
        lock (gate)
        {
            var row = new TransferRow(
                $"TR-{++transferSequence:000000}", NormalizeBranch(draft.From), NormalizeBranch(draft.To),
                Math.Max(1, draft.Items), Math.Max(1, draft.Quantity), DateOnly.FromDateTime(DateTime.Today),
                draft.Expected, "Picking");
            transfers.Insert(0, row);
            return Task.FromResult(row);
        }
    }

    public Task<bool> SetTransferStatusAsync(string number, string status, CancellationToken cancellationToken = default)
    {
        lock (gate)
        {
            var index = transfers.FindIndex(x => x.Number.Equals(number, StringComparison.OrdinalIgnoreCase));
            if (index < 0) return Task.FromResult(false);
            transfers[index] = transfers[index] with { Status = status };
            return Task.FromResult(true);
        }
    }

    public Task<PurchaseOrderRow> CreatePurchaseOrderAsync(PurchaseOrderDraft draft, CancellationToken cancellationToken = default)
    {
        lock (gate)
        {
            var row = new PurchaseOrderRow(
                $"PO-{DateTime.Today.Year}-{++purchaseOrderSequence:000000}", Required(draft.Supplier, "Unassigned supplier"),
                NormalizeBranch(draft.Warehouse), DateOnly.FromDateTime(DateTime.Today), draft.ExpectedDate,
                Math.Max(1, draft.Items), Math.Max(1, draft.Quantity) * Math.Max(0, draft.UnitCost), "Pending Approval");
            purchaseOrders.Insert(0, row);
            incomingUnitDelta += Math.Max(1, draft.Quantity);
            return Task.FromResult(row);
        }
    }

    public Task<bool> SetPurchaseOrderStatusAsync(string number, string status, CancellationToken cancellationToken = default)
    {
        lock (gate)
        {
            var index = purchaseOrders.FindIndex(x => x.Number.Equals(number, StringComparison.OrdinalIgnoreCase));
            if (index < 0) return Task.FromResult(false);
            purchaseOrders[index] = purchaseOrders[index] with { Status = status };
            return Task.FromResult(true);
        }
    }

    private string UniqueSku(string requested)
    {
        var baseSku = string.IsNullOrWhiteSpace(requested) ? $"ITEM-{productSequence:0000}" : requested.Trim().ToUpperInvariant();
        var candidate = baseSku;
        var suffix = 2;
        while (products.Any(x => x.Sku.Equals(candidate, StringComparison.OrdinalIgnoreCase)))
            candidate = $"{baseSku}-{suffix++}";
        return candidate;
    }

    private static string Required(string? value, string fallback) => string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();

    private static string NormalizeBranch(string? value) => value?.Trim() switch
    {
        "Kuala Lumpur" or "KL Central Warehouse" => "KL Central",
        "Petaling Jaya" or "PJ Fulfilment Hub" => "PJ Warehouse",
        "Johor Bahru" or "Johor Warehouse" => "Johor",
        "Penang Warehouse" => "Penang",
        { Length: > 0 } branch => branch,
        _ => "KL Central"
    };

    private static string StockStatus(int available) => available switch
    {
        <= 0 => "Out of Stock",
        < 20 => "Low Stock",
        _ => "In Stock"
    };

    private static string MovementPrefix(string type) => type switch
    {
        "Stock In" => "GRN",
        "Stock Out" => "SO",
        "Transfer Out" => "TR",
        _ => "ADJ"
    };
}
