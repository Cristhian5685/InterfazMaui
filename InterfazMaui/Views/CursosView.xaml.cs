using Firebase.Database;
using InterfazMaui.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace InterfazMaui.Views
{
    public partial class CursosView : ContentPage
    {
        FirebaseClient client = new FirebaseClient("https://cursosmovil-2b6d6-default-rtdb.firebaseio.com/");

        public ObservableCollection<Cursos> CursosDisponibles { get; set; } = new ObservableCollection<Cursos>();

        public ICommand ToggleVerMasCommand { get; }

        public CursosView()
        {
            InitializeComponent();
            BindingContext = this;
            NavigationPage.SetHasNavigationBar(this, false); // Oculta la barra de navegación

            ToggleVerMasCommand = new Command<Cursos>(ToggleVerMas);
            CargarCursos();
        }


        // Método para cargar cursos desde Firebase

        public void CargarCursos()
        {
            try
            {
                client.Child("cursos")
                      .AsObservable<Cursos>()
                      .Subscribe(curso =>
                      {
                          if (curso.Object != null)
                          {
                              // Verificar si el curso ya existe en la lista
                              var cursoExistente = CursosDisponibles.FirstOrDefault(c => c.Name == curso.Object.Name);

                              if (cursoExistente == null)
                              {
                                  // Agregar nuevo curso si no existe
                                  curso.Object.MostrarDetalles = false;
                                  CursosDisponibles.Add(curso.Object);
                              }
                              else
                              {
                                  // Actualizar los detalles del curso si ya existe
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
    }
}
