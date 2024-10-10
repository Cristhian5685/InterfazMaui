using InterfazMaui.Views;

namespace InterfazMaui

{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new LoginView());
        }
    }
}
