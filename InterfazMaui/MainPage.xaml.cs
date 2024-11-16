using InterfazMaui.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Firebase.Database;
using Firebase.Database.Query;
using InterfazMaui.Views;
using Microsoft.Maui.Handlers;
using InterfazMaui.Services;


namespace InterfazMaui
{
    public partial class MainPage : ContentPage
    {
        FirebaseClient client = new FirebaseClient("https://cursosmovil-2b6d6-default-rtdb.firebaseio.com/");
        public ObservableCollection<Cursos> CursosDisponibles { get; set; }
        public ObservableCollection<Cursos> CursosMatriculados { get; set; } // Nueva colección para cursos matriculados

       

        public ICommand ToggleInscripcionCommand { get; }
        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false); // Oculta la barra de navegación
            ModifySearchBar();

            CursosDisponibles = new ObservableCollection<Cursos>(); // Inicializa la colección
            CursosMatriculados = new ObservableCollection<Cursos>(); // Inicializa la colección de cursos matriculados
            BindingContext = this;

            ToggleInscripcionCommand = new Command<Cursos>(async (curso) => await ToggleInscripcion(curso));



            // Suscribirse a notificación de desinscripción
            MessagingCenter.Subscribe<CursosView, string>(this, "CursoDesinscrito", (sender, nombreCurso) =>
            {
                var cursoADesinscribir = CursosMatriculados.FirstOrDefault(c => c.Name == nombreCurso);
                if (cursoADesinscribir != null)
                {
                   
                    CursosMatriculados.Remove(cursoADesinscribir);
                }
            });

            // Suscribirse a la notificación de inscripción
            MessagingCenter.Subscribe<CursosView, Cursos>(this, "CursoInscrito", (sender, cursoInscrito) =>
            {
                if (!CursosMatriculados.Any(c => c.Name == cursoInscrito.Name))
                {
                    
                    CursosMatriculados.Add(cursoInscrito);
                }
            });

            // Suscribirse a notificaciones de inscripción y desinscripción enviadas desde cursos
            MessagingCenter.Subscribe<CursosView, Cursos>(this, "CursoInscrito", (sender, cursoInscrito) =>
            {
                var curso = CursosDisponibles.FirstOrDefault(c => c.Name == cursoInscrito.Name);
                if (curso != null)
                {
                    curso.EstaInscrito = true;
                    curso.EstudiantesInscritos = cursoInscrito.EstudiantesInscritos;
                }
            });

            MessagingCenter.Subscribe<CursosView, string>(this, "CursoDesinscrito", (sender, nombreCurso) =>
            {
                var curso = CursosDisponibles.FirstOrDefault(c => c.Name == nombreCurso);
                if (curso != null)
                {
                    curso.EstaInscrito = false;
                    curso.EstudiantesInscritos--; // Reducir el contador de inscritos
                }
            });

            // Cargar los cursos desde Firebase
            CargarCursosDesdeFirebase();
            CargarCursosMatriculados();
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

                    // Notificar inscripción para actualizar en tiempo real en otras vistas
                    MessagingCenter.Send(this, "CursoInscrito", curso);

                    await DisplayAlert("Inscripción exitosa", "Te has inscrito en el curso correctamente.", "OK");

                    // Asegurarse de que el curso también esté en CursosMatriculados en esta vista
                    if (!CursosMatriculados.Any(c => c.Name == curso.Name))
                    {
                        CursosMatriculados.Add(curso);
                    }
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

                // Remover el curso de la lista de cursos matriculados
                var cursoMatriculado = CursosMatriculados.FirstOrDefault(c => c.Name == curso.Name);
                if (cursoMatriculado != null)
                {
                    CursosMatriculados.Remove(cursoMatriculado);
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



        private void CargarCursosDesdeFirebase()
        {
            try
            {
                client
                    .Child("cursos")
                    .AsObservable<Cursos>()
                    .Subscribe(async curso =>
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

                                        CursosDisponibles.Add(curso.Object);

                                        // Verificar si el usuario está inscrito
                                        var usuarioId = Preferences.Get("UserId", string.Empty);
                                        var inscripciones = await client.Child("inscripciones").Child(curso.Object.Name).OnceAsync<string>();
                                        if (inscripciones.Any(i => i.Object == usuarioId))
                                        {
                                            CursosMatriculados.Add(curso.Object); // Agregar a cursos matriculados si el usuario está inscrito
                                        }
                                    }
                                    else
                                    {
                                        int index = CursosDisponibles.IndexOf(cursoExistente);
                                        cursoExistente.EstaInscrito = await VerificarEstadoInscripcion(curso.Object.Name); // Actualiza el estado si el curso ya existe
                                        CursosDisponibles[index] = curso.Object;
                                    }
                                }
                                else if (curso.EventType == Firebase.Database.Streaming.FirebaseEventType.Delete)
                                {
                                    var cursoAEliminar = CursosDisponibles.FirstOrDefault(c => c.Name == curso.Object.Name);
                                    if (cursoAEliminar != null)
                                    {
                                        CursosDisponibles.Remove(cursoAEliminar);
                                        CursosMatriculados.Remove(cursoAEliminar); // Remover de cursos matriculados si se elimina
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


        private async void CargarCursosMatriculados()
        {
            var usuarioId = Preferences.Get("UserId", string.Empty);

            // Cargar todos los cursos y verificar si el usuario está inscrito en ellos
            foreach (var curso in CursosDisponibles)
            {
                var inscripciones = await client.Child("inscripciones").Child(curso.Name).OnceAsync<string>();
                if (inscripciones.Any(i => i.Object == usuarioId))
                {
                    CursosMatriculados.Add(curso); // Agregar a cursos matriculados si el usuario está inscrito
                }
            }
        }

        private void ModifySearchBar()
        {
            SearchBarHandler.Mapper.AppendToMapping("CustomSearchIconColor", (handler, view) =>
            {
#if ANDROID
                var context = handler.PlatformView.Context;
                var searchIconId = context.Resources.GetIdentifier("search_mag_icon", "id", context.PackageName);
        
                if (searchIconId != 0)
                {
                    var searchIcon = handler.PlatformView.FindViewById<Android.Widget.ImageView>(searchIconId);
                    searchIcon?.SetColorFilter(Android.Graphics.Color.Rgb(172, 157, 185), Android.Graphics.PorterDuff.Mode.SrcIn);
                }
#endif
            });
        }
    }
}
