namespace InterfazMaui;

public partial class MainTabbedPage : TabbedPage
{
	public MainTabbedPage()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Oculta la barra de navegación
    }
}