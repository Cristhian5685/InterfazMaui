//using Firebase.Database;
//using Firebase.Database.Query;
//using System.Collections.ObjectModel;
//using System.Reactive.Linq;
//using System.Windows.Input;

//namespace InterfazMaui.Views
//{
//    public partial class CursosDocenteView : ContentPage
//    {
//        private FirebaseClient client = new FirebaseClient("https://cursosmovil-2b6d6-default-rtdb.firebaseio.com/");

//        public ObservableCollection<Cursos> CursosDocente { get; set; } = new ObservableCollection<Cursos>();
//        public ICommand VerEstudiantesCommand { get; }

//        public ICommand EditarCursoCommand { get; }
//        public ICommand EliminarCursoCommand { get; }

//        public CursosDocenteView()
//        {
//            InitializeComponent();
//            BindingContext = this;
//            NavigationPage.SetHasNavigationBar(this, false); // Oculta la barra de navegación
//            VerEstudiantesCommand = new Command<Cursos>(async (curso) => await VerEstudiantes(curso));

//            //EditarCursoCommand = new Command<Cursos>(async (curso) => await EditarCurso(curso));
//            EliminarCursoCommand = new Command<Cursos>(async (curso) => await EliminarCurso(curso));

//            CargarCursosDocente();
//        }



//        private void CargarCursosDocente()
//        {
//            try
//            {
//                var docenteId = Preferences.Get("UserId", string.Empty);

//                if (!string.IsNullOrEmpty(docenteId))
//                {
//                    var observable = client
//                        .Child("cursos")
//                        .AsObservable<Cursos>()
//                        .Where(c => c.Object != null && c.Object.DocenteId == docenteId);

//                    observable.Subscribe(async curso =>
//                    {
//                        if (curso.Object == null)
//                            return;



//                        Device.BeginInvokeOnMainThread(() =>
//                        {
//                            if (curso.EventType == Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate)
//                            {
//                                var existingCurso = CursosDocente.FirstOrDefault(c => c.Name == curso.Object.Name);
//                                if (existingCurso == null)
//                                {
//                                    CursosDocente.Add(curso.Object);
//                                }
//                                else
//                                {
//                                    int index = CursosDocente.IndexOf(existingCurso);
//                                    CursosDocente[index] = curso.Object;
//                                }
//                            }
//                            else if (curso.EventType == Firebase.Database.Streaming.FirebaseEventType.Delete)
//                            {
//                                var cursoToRemove = CursosDocente.FirstOrDefault(c => c.Name == curso.Object.Name);
//                                if (cursoToRemove != null)
//                                {
//                                    CursosDocente.Remove(cursoToRemove);
//                                }
//                            }
//                        });
//                    });
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error al cargar los cursos del docente: {ex.Message}");
//            }
//        }












//        //private async Task EditarCurso(Cursos curso)
//        //{
//        //    // Puedes navegar a una nueva página de edición pasando el curso como parámetro
//        //    await Navigation.PushAsync(new EditarCursoView(curso));
//        //}

//        private async Task EliminarCurso(Cursos curso)
//        {
//            //// Confirmación de eliminación
//            //bool confirmacion = await DisplayAlert("Confirmación", "¿Estás seguro de que deseas eliminar este curso?", "Sí", "No");
//            //if (confirmacion)
//            //{
//            //    try
//            //    {
//            //        // Encuentra el curso por su clave en Firebase
//            //        var cursoKey = CursosDocente.FirstOrDefault(c => c.Name == curso.Name)?.CursoKey;
//            //        if (cursoKey != null)
//            //        {
//            //            await client.Child("cursos").Child(cursoKey).DeleteAsync();
//            //            CursosDocente.Remove(curso); // Remueve el curso de la colección local
//            //            await DisplayAlert("Éxito", "Curso eliminado correctamente.", "OK");
//            //        }
//            //        else
//            //        {
//            //            await DisplayAlert("Error", "No se pudo encontrar el ID del curso en Firebase.", "OK");
//            //        }
//            //    }
//            //    catch (Exception ex)
//            //    {
//            //        await DisplayAlert("Error", $"No se pudo eliminar el curso: {ex.Message}", "OK");
//            //    }
//            //}
//        }





//        private async Task VerEstudiantes(Cursos curso)
//        {
//            await Navigation.PushAsync(new EstudiantesCursoView(curso.Name));
//        }
//    }
//}


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
                    var observableCursos = client
                        .Child("cursos")
                        .AsObservable<Cursos>()
                        .Where(c => c.Object != null && c.Object.DocenteId == docenteId);

                    observableCursos.Subscribe(async curso =>
                    {
                        if (curso.Object == null)
                            return;

                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            // Verifica si el curso ya existe
                            var existingCurso = CursosDocente.FirstOrDefault(c => c.Name == curso.Object.Name);
                            if (existingCurso == null)
                            {
                                // Si el curso no existe, agrégalo y suscríbete a sus cambios en inscripciones
                                curso.Object.EstudiantesInscritos = await ContarEstudiantesEnCurso(curso.Object.Name);
                                CursosDocente.Add(curso.Object);
                                SuscribirCambiosEnInscripciones(curso.Object);
                            }
                            else
                            {
                                // Si el curso ya existe, actualízalo
                                existingCurso.EstudiantesInscritos = await ContarEstudiantesEnCurso(curso.Object.Name);
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

        private void SuscribirCambiosEnInscripciones(Cursos curso)
        {
            // Escucha cambios en el nodo de inscripciones para el curso específico
            client.Child("inscripciones")
                .Child(curso.Name)
                .AsObservable<object>()
                .Subscribe(async _ =>
                {
                    // Cada vez que haya un cambio, actualiza el conteo de estudiantes
                    int estudiantesInscritos = await ContarEstudiantesEnCurso(curso.Name);

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        // Busca el curso y actualiza el conteo en la lista observable
                        var existingCurso = CursosDocente.FirstOrDefault(c => c.Name == curso.Name);
                        if (existingCurso != null)
                        {
                            existingCurso.EstudiantesInscritos = estudiantesInscritos;
                        }
                    });
                });
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


