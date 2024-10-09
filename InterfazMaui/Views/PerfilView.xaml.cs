namespace InterfazMaui.Views;

public partial class PerfilView : ContentPage
{
	public PerfilView()
	{
		InitializeComponent();



	}


    private async void OnEditProfileClicked(object sender, EventArgs e)
    {
        // L�gica para editar el perfil o navegar a otra p�gina de edici�n
        await DisplayAlert("Editar Perfil", "Funcionalidad no implementada", "OK");
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        // L�gica para cerrar sesi�n, puedes limpiar la sesi�n o navegar a la pantalla de login
        bool confirmLogout = await DisplayAlert("Cerrar Sesi�n", "�Est�s seguro que deseas cerrar sesi�n?", "S�", "No");
        if (confirmLogout)
        {
            // Aqu� ir�a la l�gica de logout
            await DisplayAlert("Sesi�n Cerrada", "Has cerrado sesi�n", "OK");
            // Navegar de regreso a la pantalla de inicio de sesi�n
        }
    }
}