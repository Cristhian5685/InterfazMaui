using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using InterfazMaui.Models;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace InterfazMaui.Views
{
    public partial class EditarCursoView : ContentPage
    {
        private readonly FirebaseClient client = new FirebaseClient("https://cursosmovil-2b6d6-default-rtdb.firebaseio.com/");
        private string imageUrl; // Guardará la URL de la imagen subida
        private readonly string cursoId; // ID del curso a editar
        private Cursos cursoActual; // Curso actual a editar

        public EditarCursoView(Cursos curso, string cursoId) // Agregamos el parámetro cursoId
        {
            InitializeComponent();
            this.cursoId = cursoId; // Asignamos el ID del curso de Firebase
            cursoActual = curso;
            BindingContext = this;

            // Cargar datos del curso en los campos de la interfaz
            NameEntry.Text = cursoActual.Name;
            CursoImage.Source = cursoActual.Image;
            DescripcionEditor.Text = cursoActual.Descripcion;
            DuracionEntry.Text = cursoActual.Duracion;
            NivelPicker.SelectedItem = cursoActual.Nivel;
            RequisitosEditor.Text = cursoActual.Requisitos;
            imageUrl = cursoActual.Image; // Asignamos la URL de la imagen actual
        }

        private async Task ActualizarNombreCursoEnInscripciones(string nombreAntiguo, string nombreNuevo)
        {
            try
            {
                // Recupera todas las inscripciones asociadas al nombre antiguo
                var inscripcionesAntiguas = await client
                    .Child("inscripciones")
                    .Child(nombreAntiguo)
                    .OnceAsync<string>();

                // Copia cada inscripción al nuevo nombre del curso
                foreach (var inscripcion in inscripcionesAntiguas)
                {
                    await client
                        .Child("inscripciones")
                        .Child(nombreNuevo)
                        .Child(inscripcion.Key)
                        .PutAsync(inscripcion.Object);
                }

                // Elimina las inscripciones antiguas del nombre viejo
                await client.Child("inscripciones").Child(nombreAntiguo).DeleteAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudieron actualizar las inscripciones: {ex.Message}", "OK");
            }
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

                CursoImage.Source = imageUrl; // Mostrar la nueva imagen seleccionada
            }
        }


        private async void OnGuardarCambiosClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                await DisplayAlert("Error", "Por favor, selecciona una imagen antes de guardar los cambios.", "OK");
                return;
            }

            try
            {
                // Guardar el nombre antiguo del curso antes de actualizarlo
                string nombreAntiguo = cursoActual.Name;

                // Actualizar el curso con los datos ingresados
                cursoActual.Name = NameEntry.Text;
                cursoActual.Descripcion = DescripcionEditor.Text;
                cursoActual.Duracion = DuracionEntry.Text;
                cursoActual.Nivel = NivelPicker.SelectedItem?.ToString();
                cursoActual.Requisitos = RequisitosEditor.Text;
                cursoActual.Image = imageUrl;

                // Actualizar el curso en Firebase
                await client.Child("cursos").Child(cursoId).PutAsync(cursoActual);

                // Actualizar las inscripciones en el nodo del nuevo nombre, si el nombre cambió
                if (nombreAntiguo != cursoActual.Name)
                {
                    await ActualizarNombreCursoEnInscripciones(nombreAntiguo, cursoActual.Name);
                }

                await DisplayAlert("Éxito", "El curso y sus inscripciones han sido actualizados con éxito.", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudieron guardar los cambios: {ex.Message}", "OK");
            }
        }



    }
}
