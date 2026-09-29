using P1_Inventory_SaaS_.Shared.Services;
using P1_Inventory_SaaS_.Web.Components;
using P1_Inventory_SaaS_.Web.Services;

namespace P1_Inventory_SaaS_
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddSingleton<IFormFactor, FormFactor>();
            builder.Services.AddSingleton<IInventoryUiService, MockInventoryUiService>();
            builder.Services.AddScoped<UiShellService>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
            app.UseHttpsRedirection();
            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddAdditionalAssemblies(
                    typeof(P1_Inventory_SaaS_.Shared._Imports).Assembly);

            app.Run();
        }
    }
}
