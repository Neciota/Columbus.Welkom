namespace Columbus.Welkom.Client
{
    public partial class App : Microsoft.Maui.Controls.Application
    {
        public App()
        {
            InitializeComponent();
        }

        // MAUI 9 deprecated setting Application.MainPage in favour of creating the window here.
        protected override Window CreateWindow(IActivationState? activationState) => new(new MainPage());
    }
}
