using P1_Inventory_SaaS_.Shared.Models;

namespace P1_Inventory_SaaS_.Shared.Services;

public sealed class UiShellService
{
    private readonly List<UiToast> _toasts = [];
    private readonly List<string> _pendingSyncActions = [];
    private Func<UiConfirmRequest, Task<bool>>? _confirmationHandler;
    private Func<string, Task>? _scanHandler;

    public event Action? Changed;

    public IReadOnlyList<UiToast> Toasts => _toasts;
    public IReadOnlyList<string> PendingSyncActions => _pendingSyncActions;
    public int PendingSyncCount => _pendingSyncActions.Count;
    public UiDrawerState? Drawer { get; private set; }
    public UiImageViewerState? ImageViewer { get; private set; }
    public bool CommandPaletteOpen { get; private set; }
    public bool ScannerOpen { get; private set; }
    public bool ScannerHasConsumer => _scanHandler is not null;
    public string ScannerPurpose { get; private set; } = "inventory";
    public bool IsOnline { get; private set; } = true;
    public string ActiveBranch { get; private set; } = "All branches";
    public string ThemeMode { get; private set; } = "system";

    public void ShowToast(
        string title,
        string message = "",
        string tone = "success",
        string? actionLabel = null,
        Action? action = null)
    {
        var toast = new UiToast(Guid.NewGuid(), title, message, tone, actionLabel, action);
        _toasts.Add(toast);
        while (_toasts.Count > 5)
            _toasts.RemoveAt(0);
        Notify();
        _ = AutoDismissAsync(toast.Id);
    }

    public void CompleteTouchAction(
        string title,
        string message,
        string? actionLabel = null,
        Action? action = null)
    {
        if (IsOnline)
        {
            ShowToast(title, message, "success", actionLabel, action);
            return;
        }

        _pendingSyncActions.Add(message);
        ShowToast("Saved to offline queue", $"{message} · {PendingSyncCount} waiting to sync.", "warning");
        Notify();
    }

    public void DismissToast(Guid id)
    {
        var toast = _toasts.FirstOrDefault(x => x.Id == id);
        if (toast is null)
            return;

        _toasts.Remove(toast);
        Notify();
    }

    private async Task AutoDismissAsync(Guid id)
    {
        await Task.Delay(5000);
        DismissToast(id);
    }

    public void SetConfirmationHandler(Func<UiConfirmRequest, Task<bool>>? handler)
        => _confirmationHandler = handler;

    public Task<bool> ConfirmAsync(
        string title,
        string message,
        string confirmText = "Confirm",
        string tone = "danger")
        => _confirmationHandler?.Invoke(new UiConfirmRequest(title, message, confirmText, tone))
           ?? Task.FromResult(true);

    public void OpenProductDrawer(ProductRow product)
    {
        Drawer = new UiDrawerState(
            product.Name,
            $"{product.Sku} · {product.Category}",
            product.ImageUrl,
            [
                new("Branch", ProductBranch(product.Warehouse)),
                new("On hand", product.OnHand.ToString("N0")),
                new("Available", product.Available.ToString("N0")),
                new("Average cost", $"RM {product.AverageCost:N2}"),
                new("Selling price", $"RM {product.SellingPrice:N2}"),
                new("Status", product.Status)
            ],
            $"/products/{product.Id}",
            $"/products/{product.Id}/edit");
        Notify();
    }

    public void CloseDrawer()
    {
        Drawer = null;
        Notify();
    }

    public void OpenImage(string url, string title)
    {
        ImageViewer = new UiImageViewerState(url, title);
        Notify();
    }

    public void CloseImage()
    {
        ImageViewer = null;
        Notify();
    }

    public void SetCommandPalette(bool open)
    {
        CommandPaletteOpen = open;
        Notify();
    }

    public void SetScanner(bool open)
    {
        _scanHandler = null;
        ScannerPurpose = "inventory";
        ScannerOpen = open;
        Notify();
    }

    public void OpenScanner(Func<string, Task> onScanned, string purpose = "field")
    {
        _scanHandler = onScanned;
        ScannerPurpose = purpose;
        ScannerOpen = true;
        Notify();
    }

    public async Task CompleteScanAsync(string code)
    {
        var handler = _scanHandler;
        _scanHandler = null;
        ScannerOpen = false;
        ScannerPurpose = "inventory";
        Notify();
        if (handler is not null)
            await handler(code);
    }

    public void SetThemeMode(string mode, bool notifyUser = true)
    {
        if (mode is not ("light" or "dark" or "system"))
            mode = "system";

        if (ThemeMode == mode)
            return;

        ThemeMode = mode;
        if (notifyUser)
            ShowToast("Appearance updated", $"Theme set to {char.ToUpperInvariant(mode[0]) + mode[1..]}.", "info");
        Notify();
    }

    public void SetBranch(string branch)
    {
        if (ActiveBranch == branch)
            return;

        ActiveBranch = branch;
        ShowToast("Branch changed", branch == "All branches" ? "Showing data from all branches." : $"Now viewing {branch}.", "info");
        Notify();
    }

    public void SetOnline(bool online)
    {
        if (IsOnline == online)
            return;

        IsOnline = online;
        if (online)
        {
            var message = PendingSyncCount > 0
                ? $"Connection restored. {PendingSyncCount} queued action(s) are ready for backend sync."
                : "Connection restored. Your latest changes can sync now.";
            ShowToast("Back online", message, "success");
        }
        Notify();
    }

    public void CloseAll()
    {
        Drawer = null;
        ImageViewer = null;
        CommandPaletteOpen = false;
        ScannerOpen = false;
        _scanHandler = null;
        ScannerPurpose = "inventory";
        Notify();
    }

    private static string ProductBranch(string warehouse) => warehouse switch
    {
        var x when x.Contains("KL", StringComparison.OrdinalIgnoreCase) => "Kuala Lumpur",
        var x when x.Contains("PJ", StringComparison.OrdinalIgnoreCase) => "Petaling Jaya",
        var x when x.Contains("Johor", StringComparison.OrdinalIgnoreCase) => "Johor Bahru",
        _ => warehouse
    };

    private void Notify() => Changed?.Invoke();
}

public sealed record UiToast(
    Guid Id,
    string Title,
    string Message,
    string Tone,
    string? ActionLabel,
    Action? Action);

public sealed record UiConfirmRequest(
    string Title,
    string Message,
    string ConfirmText,
    string Tone);

public sealed record UiDrawerField(string Label, string Value);

public sealed record UiDrawerState(
    string Title,
    string Subtitle,
    string? ImageUrl,
    IReadOnlyList<UiDrawerField> Fields,
    string PrimaryHref,
    string SecondaryHref);

public sealed record UiImageViewerState(string Url, string Title);
