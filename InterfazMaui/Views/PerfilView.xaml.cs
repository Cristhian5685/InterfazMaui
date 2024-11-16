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
        public Command EditarNombreCommand { get; }
        //public Command EditarEmailCommand { get; }
        public Command EditarEspecialidadCommand { get; }
        public PerfilView()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false); // Oculta la barra de navegaci�n
            firebaseClient = new FirebaseClient("https://cursosmovil-2b6d6-default-rtdb.firebaseio.com/"); // Cambia a la URL base de tu base de datos
            authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyDHQIM8REv5OKqzIMJ-vc_jwyh2viBZqBY")); // Reemplaza con tu API key
            LoadUserData();
            EditarNombreCommand = new Command(async () => await EditarCampo("Nombre Completo", nombreLabel));
            //EditarEmailCommand = new Command(async () => await EditarCampo("Correo Electr�nico", emailLabel));
            EditarEspecialidadCommand = new Command(async () => await EditarCampo("Especialidad", especialidadLabel));
            BindingContext = this; // Aseg�rate de que los comandos est�n disponibles en el XAML
        }

        private async Task EditarCampo(string titulo, Label campo)
        {
            string nuevoValor = await DisplayPromptAsync(titulo, $"Introduce el nuevo {titulo.ToLower()}", initialValue: campo.Text);
            if (!string.IsNullOrWhiteSpace(nuevoValor))
            {
                campo.Text = nuevoValor; // Actualiza el valor en la UI temporalmente
                await GuardarCampoEnFirebase(titulo, nuevoValor); // Actualiza el valor en Firebase
            }
        }

        private async Task GuardarCampoEnFirebase(string campo, string valor)
        {
            try
            {
                string userId = Preferences.Get("UserId", string.Empty);
                if (!string.IsNullOrEmpty(userId))
                {
                    await firebaseClient
                        .Child("users")
                        .Child(userId)
                        .Child(campo.Replace(" ", "")) // Para que coincida con la estructura de datos en Firebase
                        .PutAsync(valor);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo actualizar el {campo.ToLower()}: {ex.Message}", "OK");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadUserData(); // Recarga los datos al entrar al perfil
        }


        // M�todo para seleccionar y subir una nueva foto de perfil con opci�n de Guardar o Cancelar
        private async void OnChangePhotoButtonClicked(object sender, EventArgs e)
        {
            var photoResult = await FilePicker.PickAsync(new PickOptions { FileTypes = FilePickerFileType.Images });
            if (photoResult != null)
            {
                var stream = await photoResult.OpenReadAsync();

                // Pregunta si se desea guardar la foto seleccionada
                bool confirmSave = await DisplayAlert("Confirmaci�n", "�Deseas guardar esta foto de perfil?", "Guardar", "Cancelar");

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
            bool confirmDelete = await DisplayAlert("Confirmaci�n", "�Est�s seguro de que deseas borrar tu foto de perfil?", "S�", "No");

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
                perfilImage.Source = "avatar.png"; // Cambia a tu imagen predeterminada
            }
        }


        // M�todo para subir la imagen a Firebase Storage y obtener la URL
        private async Task<string> UploadPhotoToFirebase(Stream imageStream)
        {
            try
            {
                var storage = new FirebaseStorage("cursosmovil-2b6d6.appspot.com");
                var imageUrl = await storage
                    .Child("profile_photos")
                    .Child($"{Guid.NewGuid()}.jpg") // Nombre �nico para cada imagen
                    .PutAsync(imageStream);
  
                return imageUrl;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo subir la foto: {ex.Message}", "OK");
                return null;
            }
        }


        // M�todo para cargar los datos del usuario desde Firebase

        //private async void LoadUserData()
        //{
        //    try
        //    {
        //        // Obt�n el UID del usuario almacenado
        //        string userId = Preferences.Get("UserId", string.Empty);

        //        if (!string.IsNullOrEmpty(userId))
        //        {
        //            // Usa el UID para recuperar los datos del usuario desde Firebase
        //            var userData = await firebaseClient
        //                .Child("users") // Aseg�rate de que este es el nodo correcto en tu base de datos
        //                .Child(userId) // Utiliza el UID del usuario
        //                .OnceSingleAsync<UserModel>();

        //            // Muestra los datos en la interfaz
        //            nombreLabel.Text = userData.NombreCompleto;
        //            emailLabel.Text = userData.Email;


        //            // Asigna la URL de la foto de perfil si existe
        //            if (!string.IsNullOrEmpty(userData.FotoPerfilUrl))
        //            {
        //                perfilImage.Source = userData.FotoPerfilUrl;
        //            }
        //            else
        //            {
        //                perfilImage.Source = "avatar.png"; // Usa una imagen predeterminada si no hay URL

        //            }

        //            if (!string.IsNullOrEmpty(userData.Especialidad))
        //            {
        //                especialidadLabel.Text = userData.Especialidad;
        //                especialidadLabel.IsVisible = true;
        //            }
        //        }
        //        else
        //        {
        //            await DisplayAlert("Error", "No se encontr� ning�n usuario autenticado.", "OK");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await DisplayAlert("Error", $"No se pudieron cargar los datos del perfil: {ex.Message}", "OK");
        //    }
        //}

        private async void LoadUserData()
        {
            try
            {
                string userId = Preferences.Get("UserId", string.Empty);

                if (!string.IsNullOrEmpty(userId))
                {
                    var userData = await firebaseClient
                        .Child("users")
                        .Child(userId)
                        .OnceSingleAsync<UserModel>();

                    // Actualiza los datos en la interfaz
                    nombreLabel.Text = userData.NombreCompleto;
                    emailLabel.Text = userData.Email;

                    if (!string.IsNullOrEmpty(userData.FotoPerfilUrl))
                    {
                        perfilImage.Source = userData.FotoPerfilUrl;
                    }
                    else
                    {
                        perfilImage.Source = "avatar.png"; // Imagen predeterminada
                    }

                    // Especialidad
                    if (!string.IsNullOrEmpty(userData.Especialidad))
                    {
                        especialidadLabel.Text = userData.Especialidad;
                    }

                    // Controlar la visibilidad basado en el rol
                    bool isDocente = userData.Role == "Docente";
                    especialidadLabel.IsVisible = isDocente;
                    especialidadFrame.IsVisible = isDocente; // Aseg�rate de nombrar tu Frame en el XAML
                }
                else
                {
                    await DisplayAlert("Error", "No se encontr� ning�n usuario autenticado.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudieron cargar los datos del perfil: {ex.Message}", "OK");
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirmLogout = await DisplayAlert("Cerrar Sesi�n", "�Est�s seguro de que quieres cerrar sesi�n?", "S�", "No");
            if (confirmLogout)
            {
                // Limpia el UID almacenado
                Preferences.Remove("UserId");

                Application.Current.MainPage = new NavigationPage(new LoginView());
            }
        }
    }
}
