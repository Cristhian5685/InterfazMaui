using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using InterfazMaui.Models;

namespace InterfazMaui.Views
{
    public partial class PerfilView : ContentPage
    {
        private FirebaseClient firebaseClient;
        private FirebaseAuthProvider authProvider;

        public PerfilView()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false); // Oculta la barra de navegación
            firebaseClient = new FirebaseClient("https://cursosmovil-2b6d6-default-rtdb.firebaseio.com/"); // Cambia a la URL base de tu base de datos
            authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyDHQIM8REv5OKqzIMJ-vc_jwyh2viBZqBY")); // Reemplaza con tu API key
            LoadUserData();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadUserData(); // Recarga los datos al entrar al perfil
        }


        // Método para seleccionar y subir una nueva foto de perfil con opción de Guardar o Cancelar
        private async void OnChangePhotoButtonClicked(object sender, EventArgs e)
        {
            var photoResult = await FilePicker.PickAsync(new PickOptions { FileTypes = FilePickerFileType.Images });
            if (photoResult != null)
            {
                var stream = await photoResult.OpenReadAsync();

                // Pregunta si se desea guardar la foto seleccionada
                bool confirmSave = await DisplayAlert("Confirmación", "¿Deseas guardar esta foto de perfil?", "Guardar", "Cancelar");

                if (confirmSave)
                {
                    var storageImageUrl = await UploadPhotoToFirebase(stream);

                    // Actualizar la URL de la foto de perfil en Firebase Database
                    if (!string.IsNullOrEmpty(storageImageUrl))
                    {
                        string userId = Preferences.Get("UserId", string.Empty);
                        await firebaseClient
                            .Child("users")
                            .Child(userId)
                            .Child("FotoPerfilUrl")
                            .PutAsync(storageImageUrl);

                        // Actualiza la imagen en la interfaz
                        perfilImage.Source = storageImageUrl;
                    }
                }
            }
        }

        private async void OnDeletePhotoButtonClicked(object sender, EventArgs e)
        {
            bool confirmDelete = await DisplayAlert("Confirmación", "¿Estás seguro de que deseas borrar tu foto de perfil?", "Sí", "No");

            if (confirmDelete)
            {
                string userId = Preferences.Get("UserId", string.Empty);

                // Eliminar la URL de la foto de perfil en Firebase Database
                await firebaseClient
                    .Child("users")
                    .Child(userId)
                    .Child("FotoPerfilUrl")
                    .DeleteAsync();

                // Establecer una imagen predeterminada en la interfaz
                perfilImage.Source = "foto_perfil.png"; // Cambia a tu imagen predeterminada
            }
        }


        // Método para subir la imagen a Firebase Storage y obtener la URL
        private async Task<string> UploadPhotoToFirebase(Stream imageStream)
        {
            try
            {
                var storage = new FirebaseStorage("cursosmovil-2b6d6.appspot.com");
                var imageUrl = await storage
                    .Child("profile_photos")
                    .Child($"{Guid.NewGuid()}.jpg") // Nombre único para cada imagen
                    .PutAsync(imageStream);
  
                return imageUrl;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo subir la foto: {ex.Message}", "OK");
                return null;
            }
        }










        // Método para cargar los datos del usuario desde Firebase

        private async void LoadUserData()
        {
            try
            {
                // Obtén el UID del usuario almacenado
                string userId = Preferences.Get("UserId", string.Empty);

                if (!string.IsNullOrEmpty(userId))
                {
                    // Usa el UID para recuperar los datos del usuario desde Firebase
                    var userData = await firebaseClient
                        .Child("users") // Asegúrate de que este es el nodo correcto en tu base de datos
                        .Child(userId) // Utiliza el UID del usuario
                        .OnceSingleAsync<UserModel>();

                    // Muestra los datos en la interfaz
                    nombreLabel.Text = userData.NombreCompleto;
                    emailLabel.Text = userData.Email;


                    // Asigna la URL de la foto de perfil si existe
                    if (!string.IsNullOrEmpty(userData.FotoPerfilUrl))
                    {
                        perfilImage.Source = userData.FotoPerfilUrl;
                    }
                    else
                    {
                        perfilImage.Source = "foto_perfil.png"; // Usa una imagen predeterminada si no hay URL

                    }

                    if (!string.IsNullOrEmpty(userData.Especialidad))
                    {
                        especialidadLabel.Text = userData.Especialidad;
                        especialidadLabel.IsVisible = true;
                    }
                }
                else
                {
                    await DisplayAlert("Error", "No se encontró ningún usuario autenticado.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudieron cargar los datos del perfil: {ex.Message}", "OK");
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirmLogout = await DisplayAlert("Cerrar Sesión", "¿Estás seguro de que quieres cerrar sesión?", "Sí", "No");
            if (confirmLogout)
            {
                // Limpia el UID almacenado
                Preferences.Remove("UserId");

                Application.Current.MainPage = new NavigationPage(new LoginView());
            }
        }
    }
}
