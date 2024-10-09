namespace InterfazMaui.Views;

public partial class PerfilView : ContentPage
{
	public PerfilView()
	{
		InitializeComponent();



	}


    private async void OnEditProfileClicked(object sender, EventArgs e)
    {
        // Lógica para editar el perfil o navegar a otra página de edición
        await DisplayAlert("Editar Perfil", "Funcionalidad no implementada", "OK");
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        // Lógica para cerrar sesión, puedes limpiar la sesión o navegar a la pantalla de login
        bool confirmLogout = await DisplayAlert("Cerrar Sesión", "¿Estás seguro que deseas cerrar sesión?", "Sí", "No");
        if (confirmLogout)
        {
            // Aquí iría la lógica de logout
            await DisplayAlert("Sesión Cerrada", "Has cerrado sesión", "OK");
            // Navegar de regreso a la pantalla de inicio de sesión
        }
    }
}