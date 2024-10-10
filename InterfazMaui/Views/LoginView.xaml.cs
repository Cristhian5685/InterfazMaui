namespace InterfazMaui.Views;

public partial class LoginView : ContentPage
{
	public LoginView()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Oculta la barra de navegación
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        // Aquí puedes agregar validación de credenciales si es necesario
        await Navigation.PushAsync(new MainTabbedPage()); // Navega a la MainTabbedPage
    }
}