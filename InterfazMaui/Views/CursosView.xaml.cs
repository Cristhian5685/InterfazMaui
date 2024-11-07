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
        public ICommand DesinscribirseCommand { get; }

        public CursosView()
        {
            InitializeComponent();
            BindingContext = this;
            NavigationPage.SetHasNavigationBar(this, false);

            ToggleVerMasCommand = new Command<Cursos>(ToggleVerMas);
            InscribirseCommand = new Command<Cursos>(async (curso) => await InscribirseCurso(curso));
            DesinscribirseCommand = new Command<Cursos>(async (curso) => await DesinscribirseCurso(curso));
        



        CargarCursos();
        }

        private async Task DesinscribirseCurso(Cursos curso)
        {
            if (curso != null)
            {
                var usuarioId = Preferences.Get("UserId", string.Empty);

                // Buscar y eliminar la inscripción del usuario en Firebase
                var inscripcion = (await client.Child("inscripciones")
                    .Child(curso.Name)
                    .OnceAsync<string>())
                    .FirstOrDefault(x => x.Object == usuarioId);

                if (inscripcion != null)
                {
                    await client.Child("inscripciones")
                        .Child(curso.Name)
                        .Child(inscripcion.Key)
                        .DeleteAsync();

                    curso.EstudiantesInscritos -= 1; // Actualiza el contador

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        var cursoEnLista = CursosDisponibles.FirstOrDefault(c => c.Name == curso.Name);
                        if (cursoEnLista != null)
                        {
                            cursoEnLista.EstudiantesInscritos = curso.EstudiantesInscritos;
                        }
                    });

                    await DisplayAlert("Desinscripción exitosa", "Te has desinscrito del curso correctamente.", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "No estás inscrito en este curso.", "OK");
                }
            }
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
                              Device.BeginInvokeOnMainThread(async () =>
                              {
                                  if (curso.EventType == Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate)
                                  {
                                      var cursoExistente = CursosDisponibles.FirstOrDefault(c => c.Name == curso.Object.Name);
                                      if (cursoExistente == null)
                                      {
                                          curso.Object.MostrarDetalles = false;
                                          curso.Object.EstudiantesInscritos = await ObtenerNumeroDeInscritos(curso.Object.Name);
                                          CursosDisponibles.Add(curso.Object);

                                          // Suscribirse a cambios en las inscripciones del curso
                                          SuscribirCambiosEnInscripciones(curso.Object);
                                      }
                                      else
                                      {
                                          cursoExistente.EstudiantesInscritos = await ObtenerNumeroDeInscritos(curso.Object.Name);
                                      }
                                  }
                                  else if (curso.EventType == Firebase.Database.Streaming.FirebaseEventType.Delete)
                                  {
                                      var cursoAEliminar = CursosDisponibles.FirstOrDefault(c => c.Name == curso.Object.Name);
                                      if (cursoAEliminar != null)
                                      {
                                          CursosDisponibles.Remove(cursoAEliminar);
                                      }
                                  }
                              });
                          }
                      },
                      ex => Console.WriteLine($"Error al cargar cursos en tiempo real: {ex.Message}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al configurar la observación de cursos: {ex.Message}");
            }
        }

        // Método para suscribirse a cambios en inscripciones de un curso
        private void SuscribirCambiosEnInscripciones(Cursos curso)
        {
            client.Child("inscripciones")
                  .Child(curso.Name)
                  .AsObservable<object>()
                  .Subscribe(async _ =>
                  {
                      curso.EstudiantesInscritos = await ObtenerNumeroDeInscritos(curso.Name);
                  });
        }

        private void ToggleVerMas(Cursos curso)
        {
            if (curso != null)
            {
                curso.MostrarDetalles = !curso.MostrarDetalles;
            }
        }

        //private async Task InscribirseCurso(Cursos curso)
        //{
        //    if (curso != null)
        //    {
        //        var usuarioId = Preferences.Get("UserId", string.Empty);


        //        // Verificar si el usuario ya está inscrito
        //        var inscripcionExistente = (await client.Child("inscripciones")
        //            .Child(curso.Name)
        //            .OnceAsync<string>())
        //            .Any(x => x.Object == usuarioId);

        //        if (inscripcionExistente)
        //        {
        //            await DisplayAlert("Inscripción duplicada", "Ya estás inscrito en este curso.", "OK");
        //        }
        //        else
        //        {
        //            // Agregar la inscripción
        //            await client.Child("inscripciones")
        //                .Child(curso.Name)
        //                .PostAsync(usuarioId);

        //            await DisplayAlert("Inscripción exitosa", "Te has inscrito en el curso correctamente.", "OK");


        //        }
        //    }
        //}

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

                    // Incrementar y notificar el cambio en el contador de estudiantes inscritos
                    curso.EstudiantesInscritos += 1;

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
