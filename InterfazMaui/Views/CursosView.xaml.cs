using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using InterfazMaui.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using InterfazMaui.Services;
namespace InterfazMaui.Views
{
    public partial class CursosView : ContentPage
    {
        FirebaseClient client = new FirebaseClient("https://cursosmovil-2b6d6-default-rtdb.firebaseio.com/");

        public ObservableCollection<Cursos> CursosDisponibles { get; set; } = new ObservableCollection<Cursos>();

        public ICommand ToggleVerMasCommand { get; }
        public ICommand InscribirseCommand { get; }
        public ICommand DesinscribirseCommand { get; }

        public ICommand ToggleInscripcionCommand { get; }

        //private readonly CursoService _cursoService;
        public CursosView()
        {
            InitializeComponent();
            BindingContext = this;
            NavigationPage.SetHasNavigationBar(this, false);

            ToggleVerMasCommand = new Command<Cursos>(ToggleVerMas);
            InscribirseCommand = new Command<Cursos>(async (curso) => await InscribirseCurso(curso));
            DesinscribirseCommand = new Command<Cursos>(async (curso) => await DesinscribirseCurso(curso));

            //_cursoService = new CursoService(); // Crear una instancia del servicio

            ToggleInscripcionCommand = new Command<Cursos>(async (curso) => await ToggleInscripcion(curso));


            // Suscribirse a notificaciones de inscripción y desinscripción enviadas desde MainPage
            MessagingCenter.Subscribe<MainPage, Cursos>(this, "CursoInscrito", (sender, cursoInscrito) =>
            {
                var curso = CursosDisponibles.FirstOrDefault(c => c.Name == cursoInscrito.Name);
                if (curso != null)
                {
                    curso.EstaInscrito = true;
                    curso.EstudiantesInscritos = cursoInscrito.EstudiantesInscritos;
                }
            });

            MessagingCenter.Subscribe<MainPage, string>(this, "CursoDesinscrito", (sender, nombreCurso) =>
            {
                var curso = CursosDisponibles.FirstOrDefault(c => c.Name == nombreCurso);
                if (curso != null)
                {
                    curso.EstaInscrito = false;
                    curso.EstudiantesInscritos--; // Reducir el contador de inscritos
                }
            });


            CargarCursos();
        }

        private async Task ToggleInscripcion(Cursos curso)
        {
            if (curso == null) return;

            if (curso.EstaInscrito)
            {
                await DesinscribirseCurso(curso);
            }
            else
            {
                await InscribirseCurso(curso);
            }
        }

        //private void CargarCursos()
        //{
        //    try
        //    {
        //        client.Child("cursos")
        //              .AsObservable<Cursos>()
        //              .Subscribe(curso =>
        //              {
        //                  if (curso.Object != null)
        //                  {
        //                      Device.BeginInvokeOnMainThread(async () =>
        //                      {
        //                          if (curso.EventType == Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate)
        //                          {
        //                              var cursoExistente = CursosDisponibles.FirstOrDefault(c => c.Name == curso.Object.Name);
        //                              if (cursoExistente == null)
        //                              {
        //                                  curso.Object.MostrarDetalles = false;
        //                                  curso.Object.EstudiantesInscritos = await ObtenerNumeroDeInscritos(curso.Object.Name);
        //                                  CursosDisponibles.Add(curso.Object);

        //                                  // Suscribirse a cambios en las inscripciones del curso
        //                                  SuscribirCambiosEnInscripciones(curso.Object);
        //                              }
        //                              else
        //                              {
        //                                  cursoExistente.EstudiantesInscritos = await ObtenerNumeroDeInscritos(curso.Object.Name);
        //                              }
        //                          }
        //                          else if (curso.EventType == Firebase.Database.Streaming.FirebaseEventType.Delete)
        //                          {
        //                              var cursoAEliminar = CursosDisponibles.FirstOrDefault(c => c.Name == curso.Object.Name);
        //                              if (cursoAEliminar != null)
        //                              {
        //                                  CursosDisponibles.Remove(cursoAEliminar);
        //                              }
        //                          }
        //                      });
        //                  }
        //              },
        //              ex => Console.WriteLine($"Error al cargar cursos en tiempo real: {ex.Message}"));
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error al configurar la observación de cursos: {ex.Message}");
        //    }
        //}

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
                                          // Verifica si el usuario está inscrito en el curso al cargar
                                          curso.Object.EstaInscrito = await VerificarEstadoInscripcion(curso.Object.Name);

                                          curso.Object.MostrarDetalles = false;
                                          curso.Object.EstudiantesInscritos = await ObtenerNumeroDeInscritos(curso.Object.Name);
                                          CursosDisponibles.Add(curso.Object);

                                          // Suscribirse a cambios en las inscripciones del curso
                                          SuscribirCambiosEnInscripciones(curso.Object);
                                      }
                                      else
                                      {
                                          cursoExistente.EstudiantesInscritos = await ObtenerNumeroDeInscritos(curso.Object.Name);
                                          cursoExistente.EstaInscrito = await VerificarEstadoInscripcion(curso.Object.Name); // Actualiza el estado si el curso ya existe
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

        private async Task<bool> VerificarEstadoInscripcion(string cursoId)
        {
            var usuarioId = Preferences.Get("UserId", string.Empty);

            // Verifica en Firebase si el usuario está inscrito en el curso
            var inscripciones = await client.Child("inscripciones")
                .Child(cursoId)
                .OnceAsync<string>();

            return inscripciones.Any(inscripcion => inscripcion.Object == usuarioId);
        }

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

                    curso.EstudiantesInscritos += 1;
                    curso.EstaInscrito = true;

                    // Enviar notificación de inscripción
                    MessagingCenter.Send(this, "CursoInscrito", curso);

                    await DisplayAlert("Inscripción exitosa", "Te has inscrito en el curso correctamente.", "OK");
                }
            }
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
                    curso.EstaInscrito = false;

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        var cursoEnLista = CursosDisponibles.FirstOrDefault(c => c.Name == curso.Name);
                        if (cursoEnLista != null)
                        {
                            cursoEnLista.EstudiantesInscritos = curso.EstudiantesInscritos;
                        }
                    });

                    // Enviar notificación de desinscripción
                    MessagingCenter.Send(this, "CursoDesinscrito", curso.Name);

                    await DisplayAlert("Desinscripción exitosa", "Te has desinscrito del curso correctamente.", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "No estás inscrito en este curso.", "OK");
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
