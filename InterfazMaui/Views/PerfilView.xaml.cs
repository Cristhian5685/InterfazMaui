using Firebase.Auth;
using Firebase.Database;
using System.Text.RegularExpressions;
using Firebase.Database.Query;
using InterfazMaui.Models;

namespace InterfazMaui.Views
{
    public partial class PerfilView : ContentPage
    {
        private string userId; // ID del usuario autenticado
        private FirebaseClient firebaseClient;

        public PerfilView()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false); // Oculta la barra de navegaci�n
           

            // Aqu� podr�as manejar casos donde el userId no est� disponible a�n
        }
        public PerfilView(string userId) : this() // Constructor que recibe el userId
        {
            this.userId = userId;
            firebaseClient = new FirebaseClient("https://cursosmovil-2b6d6-default-rtdb.firebaseio.com/");
           
        }




        private async void OnEditProfileClicked(object sender, EventArgs e)
        {
            // Aqu� la l�gica para editar el perfil
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
}
