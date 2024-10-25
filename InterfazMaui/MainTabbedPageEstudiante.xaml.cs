using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
namespace InterfazMaui;

public partial class MainTabbedPageEstudiante : Microsoft.Maui.Controls.TabbedPage
{
	public MainTabbedPageEstudiante()
	{
		InitializeComponent();
        Microsoft.Maui.Controls.NavigationPage.SetHasNavigationBar(this, false); // Oculta la barra de navegaci�n

        // Deshabilitar el gesto de deslizamiento en Android
        this.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(false);
    }
}