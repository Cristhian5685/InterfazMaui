using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using InterfazMaui.Models;

namespace InterfazMaui.Views;

public partial class CursosView : ContentPage
{
    public ObservableCollection<FairyTale> CursosDisponibles { get; set; }
    public ICommand VerMasCommand { get; set; }
    public CursosView()
    {
        InitializeComponent();
        InitializeTales2();


        VerMasCommand = new Command<FairyTale>(OnVerMas);
        BindingContext = this;


    }

  private void OnVerMas(FairyTale curso)
    {
        if (curso != null)
        {
            // Cambiar la visibilidad de los detalles
            curso.MostrarDetalles = !curso.MostrarDetalles;

            // Notificar que la propiedad ha cambiado para refrescar la vista
            // Esto es necesario cuando cambiamos algo dentro de una colección enlazada
            OnPropertyChanged(nameof(CursosDisponibles));
        }
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
            Descripcion = "Aprende los fundamentos de la seguridad informática y cómo proteger sistemas de ataques.",
            Duracion = "3 meses",
            Nivel = "Intermedio",
            Requisitos = "Conocimientos básicos de informática"
        },

            new FairyTale
        {
            Name = "Cursos de dibujo",
            Docente = "Maria Antonieta de las Nieves",
            Image = "dibujo.jpeg",
            Descripcion = "Curso para aprender técnicas básicas y avanzadas de dibujo.",
            Duracion = "2 meses",
            Nivel = "Básico",
            Requisitos = "Ninguno"
        },
            new FairyTale { Name = "Ingles Avanzado",
            Docente="Juan Marino Rugama", Image = "a.webp", 
            Descripcion = "Curso para aprender técnicas básicas y avanzadas de dibujo.",
            Duracion = "2 meses",
            Nivel = "Básico",
            Requisitos = "Ninguno" },

            new FairyTale { Name = "Macanica Basica",
            Docente="Jesus Cristhian Lopez Gutierrez", 
            Image = "moto.jpg", 
            Descripcion = "Curso para aprender técnicas básicas y avanzadas de dibujo.",
            Duracion = "2 meses",
            Nivel = "Básico",
            Requisitos = "Ninguno" },

            new FairyTale {Name = "Electricidad",
            Docente = "Alexande Castro", 
            Image = "electricidad.jpg", 
            Descripcion = "Curso para aprender técnicas básicas y avanzadas de dibujo.", 
            Duracion = "2 meses", 
            Nivel = "Básico", 
            Requisitos = "Ninguno"},

            };
    }

}   