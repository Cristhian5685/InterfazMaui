namespace InterfazMaui.Views;

public partial class LoginView : ContentPage
{
	public LoginView()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Oculta la barra de navegaci�n
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        // Aqu� puedes agregar validaci�n de credenciales si es necesario
        await Navigation.PushAsync(new MainTabbedPage()); // Navega a la MainTabbedPage
    }
}