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

        private async void CargarCursosDesdeFirebase()
        {
            try
            {
                var firebaseClient = new FirebaseClient("https://cursosmovil-2b6d6-default-rtdb.firebaseio.com/");
                var cursos = await firebaseClient
                    .Child("cursos")
                    .OnceAsync<Cursos>();

                CursosDisponibles.Clear();
                foreach (var curso in cursos)
                {
                    CursosDisponibles.Add(new Cursos
                    {
                        Name = curso.Object.Name,
                        Docente = curso.Object.Docente,
                        Image = curso.Object.Image,
                        Descripcion = curso.Object.Descripcion,
                        Duracion = curso.Object.Duracion,
                        Nivel = curso.Object.Nivel,
                        Requisitos = curso.Object.Requisitos
                    });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudieron cargar los cursos: {ex.Message}", "OK");
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
