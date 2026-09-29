namespace P1_Inventory_SaaS_
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "P1(Inventory SaaS)" };
        }
    }
}
