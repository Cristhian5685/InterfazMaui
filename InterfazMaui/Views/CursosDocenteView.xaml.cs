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

            //EditarCursoCommand = new Command<Cursos>(async (curso) => await EditarCurso(curso));
            EliminarCursoCommand = new Command<Cursos>(async (curso) => await EliminarCurso(curso));

            CargarCursosDocente();
        }

        //private void CargarCursosDocente()
        //{
        //    try
        //    {
        //        var docenteId = Preferences.Get("UserId", string.Empty);

        //        if (!string.IsNullOrEmpty(docenteId))
        //        {
        //            // Consulta para obtener cursos donde DocenteId coincide con el usuario autenticado
        //            var observable = client
        //                .Child("cursos")
        //                .AsObservable<Cursos>()
        //                .Where(c => c.Object.DocenteId == docenteId);

        //            observable.Subscribe(async curso =>
        //            {
        //                if (curso.EventType == Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate)
        //                {
        //                    CursosDocente.Add(curso.Object);
        //                }
        //                else if (curso.EventType == Firebase.Database.Streaming.FirebaseEventType.Delete)
        //                {
        //                    var cursoToRemove = CursosDocente.FirstOrDefault(c => c.Name == curso.Object.Name);
        //                    if (cursoToRemove != null)
        //                    {
        //                        CursosDocente.Remove(cursoToRemove);
        //                    }
        //                }
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error al cargar los cursos del docente: {ex.Message}");
        //    }
        //}


        private void CargarCursosDocente()
        {
            try
            {
                var docenteId = Preferences.Get("UserId", string.Empty);

                if (!string.IsNullOrEmpty(docenteId))
                {
                    var observable = client
                        .Child("cursos")
                        .AsObservable<Cursos>()
                        .Where(c => c.Object != null && c.Object.DocenteId == docenteId);

                    observable.Subscribe(async curso =>
                    {
                        if (curso.Object == null)
                            return;

                  

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            if (curso.EventType == Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate)
                            {
                                var existingCurso = CursosDocente.FirstOrDefault(c => c.Name == curso.Object.Name);
                                if (existingCurso == null)
                                {
                                    CursosDocente.Add(curso.Object);
                                }
                                else
                                {
                                    int index = CursosDocente.IndexOf(existingCurso);
                                    CursosDocente[index] = curso.Object;
                                }
                            }
                            else if (curso.EventType == Firebase.Database.Streaming.FirebaseEventType.Delete)
                            {
                                var cursoToRemove = CursosDocente.FirstOrDefault(c => c.Name == curso.Object.Name);
                                if (cursoToRemove != null)
                                {
                                    CursosDocente.Remove(cursoToRemove);
                                }
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

        //private async Task EditarCurso(Cursos curso)
        //{
        //    // Puedes navegar a una nueva página de edición pasando el curso como parámetro
        //    await Navigation.PushAsync(new EditarCursoView(curso));
        //}

        private async Task EliminarCurso(Cursos curso)
        {
            //// Confirmación de eliminación
            //bool confirmacion = await DisplayAlert("Confirmación", "¿Estás seguro de que deseas eliminar este curso?", "Sí", "No");
            //if (confirmacion)
            //{
            //    try
            //    {
            //        // Encuentra el curso por su clave en Firebase
            //        var cursoKey = CursosDocente.FirstOrDefault(c => c.Name == curso.Name)?.CursoKey;
            //        if (cursoKey != null)
            //        {
            //            await client.Child("cursos").Child(cursoKey).DeleteAsync();
            //            CursosDocente.Remove(curso); // Remueve el curso de la colección local
            //            await DisplayAlert("Éxito", "Curso eliminado correctamente.", "OK");
            //        }
            //        else
            //        {
            //            await DisplayAlert("Error", "No se pudo encontrar el ID del curso en Firebase.", "OK");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        await DisplayAlert("Error", $"No se pudo eliminar el curso: {ex.Message}", "OK");
            //    }
            //}
        }





        private async Task VerEstudiantes(Cursos curso)
        {
            await Navigation.PushAsync(new EstudiantesCursoView(curso.Name));
        }
    }
}





