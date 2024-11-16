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
            EstudiantesInscritos.Clear();

            client.Child("inscripciones")
                .Child(cursoId)
                .AsObservable<string>()
                .Subscribe(async inscripcion =>
                {
                    if (inscripcion.Object == null)
                        return;

                    string estudianteUID = inscripcion.Object;
                    var estudianteData = await client
                        .Child("users")
                        .Child(estudianteUID)
                        .OnceSingleAsync<Estudiante>();

                    if (estudianteData != null)
                    {
                        // Asegurar que se asigne la imagen predeterminada si no hay una URL
                        estudianteData.FotoPerfilUrl ??= "avatar.png";

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
        public string FotoPerfilUrl { get; set; } = "avatar.png";

    }
}






