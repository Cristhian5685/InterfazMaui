using InterfazMaui.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Firebase.Database;
using Firebase.Database.Query;
using InterfazMaui.Views;
using Microsoft.Maui.Handlers;

namespace InterfazMaui
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Cursos> CursosDisponibles { get; set; }

        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false); // Oculta la barra de navegación
            ModifySearchBar();
            CursosDisponibles = new ObservableCollection<Cursos>(); // Inicializa la colección
            BindingContext = this;

            // Cargar los cursos desde Firebase
            CargarCursosDesdeFirebase();
        }

        private async void OnMatricularButtonClicked(object sender, EventArgs e)
        {
            // Navegar a la vista de matrícula
            await Navigation.PushAsync(new MatriculaView());
        }



        //private void CargarCursosDesdeFirebase()
        //{
        //    try
        //    {
        //        var firebaseClient = new FirebaseClient("https://cursosmovil-2b6d6-default-rtdb.firebaseio.com/");

        //        // Suscribirse a los cambios en tiempo real de la lista de cursos
        //        firebaseClient
        //            .Child("cursos")
        //            .AsObservable<Cursos>()
        //            .Subscribe(curso =>
        //            {
        //                if (curso.Object != null)
        //                {
        //                    // Verificar si el curso ya existe en la lista
        //                    var cursoExistente = CursosDisponibles.FirstOrDefault(c => c.Name == curso.Object.Name);

        //                    if (cursoExistente == null)
        //                    {
        //                        // Si el curso no existe, agregarlo a la lista
        //                        CursosDisponibles.Add(new Cursos
        //                        {
        //                            Name = curso.Object.Name,
        //                            Docente = curso.Object.Docente,
        //                            Image = curso.Object.Image,
        //                            Descripcion = curso.Object.Descripcion,
        //                            Duracion = curso.Object.Duracion,
        //                            Nivel = curso.Object.Nivel,
        //                            Requisitos = curso.Object.Requisitos,
        //                            MostrarDetalles = false
        //                        });
        //                    }
        //                    else
        //                    {
        //                        // Si el curso ya existe, actualizar sus detalles
        //                        int index = CursosDisponibles.IndexOf(cursoExistente);
        //                        CursosDisponibles[index] = curso.Object;
        //                    }
        //                }
        //            },
        //            ex => Console.WriteLine($"Error al cargar cursos en tiempo real: {ex.Message}"));
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error al configurar la observación de cursos: {ex.Message}");
        //    }
        //}

        private void CargarCursosDesdeFirebase()
        {
            try
            {
                var firebaseClient = new FirebaseClient("https://cursosmovil-2b6d6-default-rtdb.firebaseio.com/");

                // Suscribirse a los cambios en tiempo real de la lista de cursos
                firebaseClient
                    .Child("cursos")
                    .AsObservable<Cursos>()
                    .Subscribe(curso =>
                    {
                        if (curso.Object != null)
                        {
                            // Ejecutar en el hilo principal para actualizar la UI
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                // Verificar si el curso ya existe en la lista
                                var cursoExistente = CursosDisponibles.FirstOrDefault(c => c.Name == curso.Object.Name);

                                if (cursoExistente == null)
                                {
                                    // Si el curso no existe, agregarlo a la lista
                                    CursosDisponibles.Add(new Cursos
                                    {
                                        Name = curso.Object.Name,
                                        Docente = curso.Object.Docente,
                                        Image = curso.Object.Image,
                                        Descripcion = curso.Object.Descripcion,
                                        Duracion = curso.Object.Duracion,
                                        Nivel = curso.Object.Nivel,
                                        Requisitos = curso.Object.Requisitos,
                                        MostrarDetalles = false
                                    });
                                }
                                else
                                {
                                    // Si el curso ya existe, actualizar sus detalles
                                    int index = CursosDisponibles.IndexOf(cursoExistente);
                                    CursosDisponibles[index] = curso.Object;
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
