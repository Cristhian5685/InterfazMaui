using InterfazMaui.Models;
using Microsoft.Maui.Handlers;
using System.Collections.ObjectModel;
using System.Windows.Input;
using InterfazMaui.Views;

namespace InterfazMaui
{
    public partial class MainPage : ContentPage
    {

        public ObservableCollection<FairyTale> FairyTales { get; set; }
        public ObservableCollection<FairyTale> FairyTales2 { get; set; }

        public ICommand NavigateCommand { get; }



        public MainPage()
        {
            InitializeComponent();
            ModifySearchBar();
            InitializeTales();
            NavigateCommand = new Command<string>(Navigate);
            BindingContext = this;
           
        }



        private async void Navigate(string page)
        {

            if (page == "Inicio")
            {
                await Navigation.PopToRootAsync(); // Regresar a la página principal
                return;
            }

            Page newPage = null;

            switch (page)
            {
                case "Buscar":
                    newPage = new BuscarView();// Navegar a la página de Buscar
                    break;
                case "Cursos":
                    newPage = new CursosView();// Navegar a la página de Cursos
                    break;
                case "Perfil":
                    newPage = new PerfilView();// Navegar a la página de Perfil
                    break;
            }

            if (newPage != null)
            {
               await Navigation.PushAsync(newPage);
            }
        }


        private void InitializeTales()
        {
            FairyTales = new ObservableCollection<FairyTale>
            {
                new FairyTale { Name = "Ciber Seguridad",Docente="Marcos Antonio solis", Image = "seguridad.png" },
                new FairyTale { Name = "Cursos de dibujo",Docente="Maria Antonieta de las nieves", Image = "dibujo.jpeg" },
                new FairyTale { Name = "Ingles Avanzado",Docente="Juan Marino Rugama", Image = "a.webp" },
                new FairyTale { Name = "Macanica Basica",Docente="Jesus Cristhian Lopez Gutierrez", Image = "moto.jpg" },
                new FairyTale { Name = "Electricidad",Docente="Alexande Castro", Image = "electricidad.jpg" },

            };

            FairyTales2 = new ObservableCollection<FairyTale>
                {

               new FairyTale { Name = "Ciber Seguridad",Docente="Marcos Antonio solis", Image = "seguridad.png" },
                new FairyTale { Name = "Cursos de dibujo",Docente="Maria Antonieta de las nieves", Image = "dibujo.jpeg" },
                new FairyTale { Name = "Ingles Avanzado",Docente="Juan Marino Rugama", Image = "a.webp" },
                new FairyTale { Name = "Macanica Basica",Docente="Jesus Cristhian Lopez Gutierrez", Image = "moto.jpg" },
                new FairyTale { Name = "Electricidad",Docente="Alexande Castro", Image = "electricidad.jpg" },


            };


        
        
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
