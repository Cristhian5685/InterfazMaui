using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using InterfazMaui.Models;
using Firebase.Database;
using Firebase.Database.Query;

namespace InterfazMaui.Views
{
    public partial class CursosView : ContentPage
    {
        public ICommand ToggleVerMasCommand => new Command<Cursos>(ToggleDetalles);
        public ObservableCollection<Cursos> CursosDisponibles { get; set; }

        public CursosView()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false); // Oculta la barra de navegación
            CursosDisponibles = new ObservableCollection<Cursos>(); // Inicializa la colección
            BindingContext = this;

            // Cargar cursos desde Firebase
            CargarCursosDesdeFirebase();
        }

        private async void OnMatricularButtonClicked(object sender, EventArgs e)
        {
            // Navegar a la vista de matrícula
            await Navigation.PushAsync(new MatriculaView());
        }

        private void ToggleDetalles(Cursos curso)
        {
            curso.MostrarDetalles = !curso.MostrarDetalles;

            
        }

        private async void CargarCursosDesdeFirebase()
        {
            try
            {
                var firebaseClient = new FirebaseClient("https://cursosmovil-2b6d6-default-rtdb.firebaseio.com/");
                var cursos = await firebaseClient
                    .Child("cursos")
                    .OnceAsync<Cursos>();

                foreach (var curso in cursos)
                {
                    // Añadir cada curso a la colección
                    CursosDisponibles.Add(new Cursos
                    {
                        Name = curso.Object.Name,
                        Docente = curso.Object.Docente,
                        Image = curso.Object.Image,
                        Descripcion = curso.Object.Descripcion,
                        Duracion = curso.Object.Duracion,
                        Nivel = curso.Object.Nivel,
                        Requisitos = curso.Object.Requisitos
                    });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudieron cargar los cursos: {ex.Message}", "OK");
            }
        }
    }
}
