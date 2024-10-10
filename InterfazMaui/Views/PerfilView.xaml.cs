namespace InterfazMaui.Views;

public partial class PerfilView : ContentPage
{
	public PerfilView()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Oculta la barra de navegaci�n



    }


    private async void OnEditProfileClicked(object sender, EventArgs e)
    {
        // L�gica para editar el perfil o navegar a otra p�gina de edici�n
        await DisplayAlert("Editar Perfil", "Funcionalidad no implementada", "OK");
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirmLogout = await DisplayAlert("Cerrar Sesi�n", "�Est�s seguro de que quieres cerrar sesi�n?", "S�", "No");

        if (confirmLogout)
        {
            // Reinicia la MainPage a la pantalla de inicio de sesi�n
            Application.Current.MainPage = new NavigationPage(new LoginView());
        }
    }
}