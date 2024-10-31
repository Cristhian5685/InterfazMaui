using Firebase.Database;
using Firebase.Database.Query;
using InterfazMaui.Models;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace InterfazMaui.Views
{
    public partial class MatriculaView : ContentPage
    {
        private readonly FirebaseClient _firebaseClient;

        public MatriculaView()
        {
            InitializeComponent();

            // Inicializar FirebaseClient con tu URL de la base de datos
            _firebaseClient = new FirebaseClient("https://cursosmovil-2b6d6-default-rtdb.firebaseio.com/");
        }

        private async void OnConfirmarMatriculaClicked(object sender, EventArgs e)
        {
            // Validar datos ingresados
            if (string.IsNullOrEmpty(NombreEntry.Text) || string.IsNullOrEmpty(ApellidoEntry.Text) ||
                string.IsNullOrEmpty(EdadEntry.Text) || string.IsNullOrEmpty(EmailEntry.Text))
            {
                await DisplayAlert("Error", "Por favor, completa todos los campos.", "OK");
                return;
            }

            if (!int.TryParse(EdadEntry.Text, out int edad))
            {
                await DisplayAlert("Error", "Edad debe ser un número.", "OK");
                return;
            }

            // Crear una instancia de MatriculaModel
            var matricula = new MatriculaModel
            {
                Nombre = NombreEntry.Text,
                Apellido = ApellidoEntry.Text,
                Edad = edad,
                Email = EmailEntry.Text,
                CursoId = "<ID_del_Curso>", // Debes asignar el ID del curso correspondiente aquí
                FechaMatricula = DateTime.Now
            };

            // Guardar los datos en Firebase
            await GuardarMatriculaEnFirebase(matricula);
        }

        private async Task GuardarMatriculaEnFirebase(MatriculaModel matricula)
        {
            try
            {
                await _firebaseClient
                    .Child("matriculas") // Nombre del nodo en Firebase donde se almacenarán las matrículas
                    .PostAsync(matricula);

                await DisplayAlert("Éxito", "La matrícula se ha registrado correctamente.", "OK");

                // Limpiar los campos de entrada
                NombreEntry.Text = string.Empty;
                ApellidoEntry.Text = string.Empty;
                EdadEntry.Text = string.Empty;
                EmailEntry.Text = string.Empty;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error al registrar la matrícula: {ex.Message}", "OK");
            }
        }
    }
}
