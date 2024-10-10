using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using InterfazMaui.Models;

namespace InterfazMaui.Views;

public partial class CursosView : ContentPage
{
    public ObservableCollection<FairyTale> CursosDisponibles { get; set; }
 
    public CursosView()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Oculta la barra de navegaci�n
        InitializeTales2();
        BindingContext = this;


    }

    private async void OnMatricularButtonClicked(object sender, EventArgs e)
    {
        // Navegar a la vista de matr�cula
        await Navigation.PushAsync(new MatriculaView());
    }

   

    private void InitializeTales2()
    {
        CursosDisponibles = new ObservableCollection<FairyTale>
            {

            new FairyTale
        {
            Name = "Ciber Seguridad",
            Docente = "Marcos Antonio Solis",
            Image = "seguridad.png",
            Descripcion = "Aprende los fundamentos de la seguridad inform�tica y c�mo proteger sistemas de ataques.",
            Duracion = "3 meses",
            Nivel = "Intermedio",
            Requisitos = "Conocimientos b�sicos de inform�tica"
        },

            new FairyTale
        {
            Name = "Cursos de dibujo",
            Docente = "Maria Antonieta de las Nieves",
            Image = "dibujo.jpeg",
            Descripcion = "Curso para aprender t�cnicas b�sicas y avanzadas de dibujo.",
            Duracion = "2 meses",
            Nivel = "B�sico",
            Requisitos = "Ninguno"
        },
            new FairyTale { Name = "Ingles Avanzado",
            Docente="Juan Marino Rugama", Image = "a.webp", 
            Descripcion = "Curso para aprender t�cnicas b�sicas y avanzadas de dibujo.",
            Duracion = "2 meses",
            Nivel = "B�sico",
            Requisitos = "Ninguno" },

            new FairyTale { Name = "Macanica Basica",
            Docente="Jesus Cristhian Lopez Gutierrez", 
            Image = "moto.jpg", 
            Descripcion = "Curso para aprender t�cnicas b�sicas y avanzadas de dibujo.",
            Duracion = "2 meses",
            Nivel = "B�sico",
            Requisitos = "Ninguno" },

            new FairyTale {Name = "Electricidad",
            Docente = "Alexande Castro", 
            Image = "electricidad.jpg", 
            Descripcion = "Curso para aprender t�cnicas b�sicas y avanzadas de dibujo.", 
            Duracion = "2 meses", 
            Nivel = "B�sico", 
            Requisitos = "Ninguno"},

            };
    }

}   