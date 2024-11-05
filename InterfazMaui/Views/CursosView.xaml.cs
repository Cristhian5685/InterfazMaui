using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using InterfazMaui.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InterfazMaui.Views
{
    public partial class CursosView : ContentPage
    {
        FirebaseClient client = new FirebaseClient("https://cursosmovil-2b6d6-default-rtdb.firebaseio.com/");

        public ObservableCollection<Cursos> CursosDisponibles { get; set; } = new ObservableCollection<Cursos>();

        public ICommand ToggleVerMasCommand { get; }
        public ICommand InscribirseCommand { get; }

        public CursosView()
        {
            InitializeComponent();
            BindingContext = this;
            NavigationPage.SetHasNavigationBar(this, false);

            ToggleVerMasCommand = new Command<Cursos>(ToggleVerMas);
            InscribirseCommand = new Command<Cursos>(async (curso) => await InscribirseCurso(curso));

            CargarCursos();
        }

        private void CargarCursos()
        {
            try
            {
                client.Child("cursos")
                      .AsObservable<Cursos>()
                      .Subscribe(curso =>
                      {
                          if (curso.Object != null)
                          {
                              var cursoExistente = CursosDisponibles.FirstOrDefault(c => c.Name == curso.Object.Name);
                              if (cursoExistente == null)
                              {
                                  curso.Object.MostrarDetalles = false;
                                  CursosDisponibles.Add(curso.Object);
                              }
                              else
                              {
                                  int index = CursosDisponibles.IndexOf(cursoExistente);
                                  CursosDisponibles[index] = curso.Object;
                              }
                          }
                      },
                      ex => Console.WriteLine($"Error al cargar cursos en tiempo real: {ex.Message}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al configurar la observación de cursos: {ex.Message}");
            }
        }

        private void ToggleVerMas(Cursos curso)
        {
            if (curso != null)
            {
                curso.MostrarDetalles = !curso.MostrarDetalles;
            }
        }

        private async Task InscribirseCurso(Cursos curso)
        {
            if (curso != null)
            {
                var usuarioId = Preferences.Get("UserId", string.Empty);


                // Verificar si el usuario ya está inscrito
                var inscripcionExistente = (await client.Child("inscripciones")
                    .Child(curso.Name)
                    .OnceAsync<string>())
                    .Any(x => x.Object == usuarioId);

                if (inscripcionExistente)
                {
                    await DisplayAlert("Inscripción duplicada", "Ya estás inscrito en este curso.", "OK");
                }
                else
                {
                    // Agregar la inscripción
                    await client.Child("inscripciones")
                        .Child(curso.Name)
                        .PostAsync(usuarioId);

                    await DisplayAlert("Inscripción exitosa", "Te has inscrito en el curso correctamente.", "OK");

                    
                }
            }
        }

        private async Task<int> ObtenerNumeroDeInscritos(string cursoId)
        {
            var inscripciones = await client.Child("inscripciones").Child(cursoId).OnceAsync<string>();
            return inscripciones.Count;
        }

    }
}
