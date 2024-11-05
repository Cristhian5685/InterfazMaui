using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;

namespace InterfazMaui.Views
{
    public partial class EstudiantesCursoView : ContentPage
    {
        private FirebaseClient client = new FirebaseClient("https://cursosmovil-2b6d6-default-rtdb.firebaseio.com/");

        public ObservableCollection<Estudiante> EstudiantesInscritos { get; set; } = new ObservableCollection<Estudiante>();

        public EstudiantesCursoView(string cursoId)
        {
            InitializeComponent();
            BindingContext = this;
            CargarEstudiantesInscritos(cursoId);
        }

        private void CargarEstudiantesInscritos(string cursoId)
        {
            // Limpiar la lista actual de estudiantes
            EstudiantesInscritos.Clear();

            // Obtener la lista de UIDs de los estudiantes inscritos en el curso especificado
            var inscripciones = client
                .Child("inscripciones")
                .Child(cursoId)
                .AsObservable<string>()
                .Subscribe(async inscripcion =>
                {
                    if (inscripcion.Object == null)
                        return;

                    string estudianteUID = inscripcion.Object;

                    // Buscar los detalles del estudiante en el nodo de usuarios usando el UID
                    var estudianteData = await client
                        .Child("users")
                        .Child(estudianteUID)
                        .OnceSingleAsync<Estudiante>();

                    // Agregar el estudiante a la lista si se encontraron sus datos
                    if (estudianteData != null)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            EstudiantesInscritos.Add(estudianteData);
                        });
                    }
                });
        }
  




    }

    public class Estudiante
    {
        public string NombreCompleto { get; set; }
        public string Email { get; set; }

    }
}






