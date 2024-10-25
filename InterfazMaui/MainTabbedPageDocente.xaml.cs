using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace InterfazMaui;

public partial class MainTabbedPageDocente : Microsoft.Maui.Controls.TabbedPage
{
    public MainTabbedPageDocente()
    {
        InitializeComponent();
        Microsoft.Maui.Controls.NavigationPage.SetHasNavigationBar(this, false); // Oculta la barra de navegación

        // Deshabilitar el gesto de deslizamiento en Android
        this.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(false);
    }
}

