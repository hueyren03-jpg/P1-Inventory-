using Microsoft.AspNetCore.Components.WebView.Maui;
using P1_Inventory_SaaS_.Shared;

namespace P1_Inventory_SaaS_
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            blazorWebView.RootComponents.Add(new RootComponent
            {
                Selector = "#app",
                ComponentType = typeof(Routes)
            });
        }
    }
}
