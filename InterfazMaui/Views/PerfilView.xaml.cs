namespace InterfazMaui.Views;

public partial class PerfilView : ContentPage
{
	public PerfilView()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Oculta la barra de navegación



    }


    private async void OnEditProfileClicked(object sender, EventArgs e)
    {
        // Lógica para editar el perfil o navegar a otra página de edición
        await DisplayAlert("Editar Perfil", "Funcionalidad no implementada", "OK");
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirmLogout = await DisplayAlert("Cerrar Sesión", "¿Estás seguro de que quieres cerrar sesión?", "Sí", "No");

        if (confirmLogout)
        {
            // Reinicia la MainPage a la pantalla de inicio de sesión
            Application.Current.MainPage = new NavigationPage(new LoginView());
        }
    }
}