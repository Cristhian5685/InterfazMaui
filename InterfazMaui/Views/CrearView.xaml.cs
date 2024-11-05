using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using InterfazMaui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace InterfazMaui.Views
{
    public partial class CrearView : ContentPage
    {
        private readonly FirebaseClient client = new FirebaseClient("https://cursosmovil-2b6d6-default-rtdb.firebaseio.com/");
        private string imageUrl; // Guardará la URL de la imagen subida

        public CrearView()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false); // Oculta la barra de navegación

            BindingContext = this;
        }

        private async void OnSeleccionarImagenClicked(object sender, EventArgs e)
        {
            var foto = await MediaPicker.PickPhotoAsync();
            if (foto != null)
            {
                var stream = await foto.OpenReadAsync();
                imageUrl = await new FirebaseStorage("cursosmovil-2b6d6.appspot.com")
                    .Child("images")
                    .Child(DateTime.Now.ToString("ddMMyyyyhhmmss") + foto.FileName)
                    .PutAsync(stream);

                // Mostrar la imagen seleccionada en la interfaz
                CursoImage.Source = imageUrl;
            }
        }


        private async void OnCrearCursoClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                await DisplayAlert("Error", "Por favor, selecciona una imagen antes de crear el curso.", "OK");
                return;
            }

            // Obtener el UserId del docente autenticado
            var docenteId = Preferences.Get("UserId", string.Empty);
            if (string.IsNullOrEmpty(docenteId))
            {
                await DisplayAlert("Error", "No se pudo obtener el ID del docente.", "OK");
                return;
            }

            // Recuperar el nombre del docente desde Firebase
            var docenteData = await client
                .Child("users")
                .Child(docenteId)
                .OnceSingleAsync<UserModel>();

            if (docenteData == null)
            {
                await DisplayAlert("Error", "No se pudo obtener el nombre del docente.", "OK");
                return;
            }

            // Crear una nueva instancia de Cursos con los datos del docente
            var nuevoCurso = new Cursos
            {
                Name = NameEntry.Text,
                Docente = docenteData.NombreCompleto, // Nombre del docente
                DocenteId = docenteId,                // ID del docente
                Image = imageUrl,                     // URL de la imagen subida
                Descripcion = DescripcionEditor.Text,
                Duracion = DuracionEntry.Text,
                Nivel = NivelPicker.SelectedItem?.ToString(),
                Requisitos = RequisitosEditor.Text,
            };

            try
            {
                // Guardar el curso en Firebase
                await client.Child("cursos").PostAsync(nuevoCurso);
                await DisplayAlert("Éxito", "El curso ha sido creado con éxito.", "OK");

                // Limpiar campos después de la creación
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo crear el curso: {ex.Message}", "OK");
            }
        }


        private void LimpiarCampos()
        {
            NameEntry.Text = string.Empty;
            //DocenteEntry.Text = string.Empty;
            CursoImage.Source = null; // Limpia la imagen seleccionada
            DescripcionEditor.Text = string.Empty;
            DuracionEntry.Text = string.Empty;
            NivelPicker.SelectedItem = null;
            RequisitosEditor.Text = string.Empty;
            imageUrl = null; // Resetear la URL de la imagen
        }
    }
}
   