
using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Input;

namespace InterfazMaui.Views
{
    public partial class CursosDocenteView : ContentPage
    {
        private FirebaseClient client = new FirebaseClient("https://cursosmovil-2b6d6-default-rtdb.firebaseio.com/");

        public ObservableCollection<Cursos> CursosDocente { get; set; } = new ObservableCollection<Cursos>();
        public ICommand VerEstudiantesCommand { get; }
        public ICommand EditarCursoCommand { get; }
        public ICommand EliminarCursoCommand { get; }

        public CursosDocenteView()
        {
            InitializeComponent();
            BindingContext = this;
            NavigationPage.SetHasNavigationBar(this, false); // Oculta la barra de navegación
            VerEstudiantesCommand = new Command<Cursos>(async (curso) => await VerEstudiantes(curso));
            EditarCursoCommand = new Command<Cursos>(async (curso) => await EditarCurso(curso)); // Comando de edición
            EliminarCursoCommand = new Command<Cursos>(async (curso) => await EliminarCurso(curso));

            CargarCursosDocente();
        }

        private async Task<int> ContarEstudiantesEnCurso(string cursoId)
        {
            var inscripciones = await client
                .Child("inscripciones")
                .Child(cursoId)
                .OnceAsync<object>();

            return inscripciones.Count;
        }




        private void CargarCursosDocente()
        {
            try
            {
                var docenteId = Preferences.Get("UserId", string.Empty);

                if (!string.IsNullOrEmpty(docenteId))
                {
                    // Escuchar en tiempo real los cambios en el nodo "inscripciones"
                    client.Child("inscripciones")
                        .AsObservable<object>()
                        .Subscribe(async inscripcionChange =>
                        {
                            // Actualizar el contador en cada cambio
                            await ActualizarConteoEstudiantes();
                        });

                    // Suscripción a los cursos del docente
                    client.Child("cursos")
                        .AsObservable<Cursos>()
                        .Where(c => c.Object != null && c.Object.DocenteId == docenteId)
                        .Subscribe(async curso =>
                        {
                            if (curso.Object == null) return;

                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                var existingCurso = CursosDocente.FirstOrDefault(c => c.Name == curso.Object.Name);
                                if (curso.EventType == Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate)
                                {
                                    if (existingCurso == null)
                                    {
                                        // Agregar el curso y configurar el conteo de estudiantes
                                        curso.Object.EstudiantesInscritos = await ContarEstudiantesEnCurso(curso.Object.Name);
                                        CursosDocente.Add(curso.Object);
                                    }
                                    else
                                    {
                                        existingCurso.EstudiantesInscritos = await ContarEstudiantesEnCurso(curso.Object.Name);
                                        int index = CursosDocente.IndexOf(existingCurso);
                                        CursosDocente[index] = curso.Object;
                                    }
                                }
                                else if (curso.EventType == Firebase.Database.Streaming.FirebaseEventType.Delete && existingCurso != null)
                                {
                                    CursosDocente.Remove(existingCurso);
                                }
                            });
                        });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar los cursos del docente: {ex.Message}");
            }
        }

        private async Task ActualizarConteoEstudiantes()
        {
            var tareas = CursosDocente.Select(async curso =>
            {
                curso.EstudiantesInscritos = await ContarEstudiantesEnCurso(curso.Name);
            });

            await Task.WhenAll(tareas);

            Device.BeginInvokeOnMainThread(() =>
            {
                for (int i = 0; i < CursosDocente.Count; i++)
                {
                    var curso = CursosDocente[i];
                    CursosDocente[i] = curso;
                }
            });
        }

        private async Task EditarCurso(Cursos curso)
        {
            var cursoToEdit = await client
                .Child("cursos")
                .OnceAsync<Cursos>();

            var cursoKey = cursoToEdit.FirstOrDefault(c => c.Object.Name == curso.Name)?.Key;

            if (cursoKey != null)
            {
                await Navigation.PushAsync(new EditarCursoView(curso, cursoKey)); // Pasamos tanto el curso como el cursoKey
            }
            else
            {
                await DisplayAlert("Error", "No se encontró el ID del curso para editar.", "OK");
            }
        }



        private async Task EliminarCurso(Cursos curso)
        {
            bool confirmacion = await DisplayAlert("Confirmación", $"¿Estás seguro de que deseas eliminar el curso '{curso.Name}' y todas sus inscripciones?", "Sí", "No");
            if (!confirmacion)
                return;

            try
            {
                // 1. Eliminar el curso de la colección "cursos" en Firebase
                var cursoToDelete = (await client
                    .Child("cursos")
                    .OnceAsync<Cursos>())
                    .FirstOrDefault(c => c.Object.Name == curso.Name);

                if (cursoToDelete != null)
                {
                    await client.Child("cursos").Child(cursoToDelete.Key).DeleteAsync();
                }

                // 2. Eliminar todas las inscripciones del curso en Firebase
                var inscripcionesToDelete = await client
                    .Child("inscripciones")
                    .Child(curso.Name)
                    .OnceAsync<object>();

                foreach (var inscripcion in inscripcionesToDelete)
                {
                    await client.Child("inscripciones").Child(curso.Name).Child(inscripcion.Key).DeleteAsync();
                }

                // 3. Remover el curso de la colección local para actualizar la interfaz
                Device.BeginInvokeOnMainThread(() =>
                {
                    CursosDocente.Remove(curso);
                });

                await DisplayAlert("Éxito", "El curso y sus inscripciones han sido eliminados correctamente.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo eliminar el curso: {ex.Message}", "OK");
            }
        }


        private async Task VerEstudiantes(Cursos curso)
        {
            await Navigation.PushAsync(new EstudiantesCursoView(curso.Name));
        }
    }
}


