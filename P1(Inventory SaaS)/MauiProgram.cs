using Microsoft.Extensions.Logging;
using P1_Inventory_SaaS_.Services;
using P1_Inventory_SaaS_.Shared.Services;

namespace P1_Inventory_SaaS_
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>();

            builder.Services.AddSingleton<IFormFactor, FormFactor>();
            builder.Services.AddSingleton<IInventoryUiService, MockInventoryUiService>();
            builder.Services.AddSingleton<UiShellService>();

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
