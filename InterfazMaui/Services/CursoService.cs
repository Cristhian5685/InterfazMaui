using Firebase.Database;
using Firebase.Database.Query;
using InterfazMaui.Models;
using System.Linq;
using System.Threading.Tasks;


namespace InterfazMaui.Services
{
    public class CursoService
    {
        private readonly FirebaseClient client = new FirebaseClient("https://cursosmovil-2b6d6-default-rtdb.firebaseio.com/");

        // Método para inscribirse a un curso
        public async Task<bool> InscribirseCurso(Cursos curso)
        {
            if (curso == null) return false;

            var usuarioId = Preferences.Get("UserId", string.Empty);

            // Verificar si el usuario ya está inscrito
            var inscripcionExistente = (await client.Child("inscripciones")
                .Child(curso.Name)
                .OnceAsync<string>())
                .Any(x => x.Object == usuarioId);

            if (inscripcionExistente)
            {
                await Application.Current.MainPage.DisplayAlert("Inscripción duplicada", "Ya estás inscrito en este curso.", "OK");
                return false;
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

                await Application.Current.MainPage.DisplayAlert("Inscripción exitosa", "Te has inscrito en el curso correctamente.", "OK");
                return true;
            }
        }

        // Método para desinscribirse de un curso
        public async Task<bool> DesinscribirseCurso(Cursos curso)
        {
            if (curso == null) return false;

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
                    MessagingCenter.Send(this, "CursoDesinscrito", curso.Name);
                });

                await Application.Current.MainPage.DisplayAlert("Desinscripción exitosa", "Te has desinscrito del curso correctamente.", "OK");
                return true;
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No estás inscrito en este curso.", "OK");
                return false;
            }
        }
    }
}
