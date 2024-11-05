using System.ComponentModel;

public class Cursos : INotifyPropertyChanged
{
    private bool _mostrarDetalles;

    
    public string Name { get; set; }
    public string Docente { get; set; }
    public string Image { get; set; }
    public string Descripcion { get; set; }
    public string Duracion { get; set; }
    public string Nivel { get; set; }
    public string Requisitos { get; set; }

    // Propiedad adicional para el contador de estudiantes inscritos
    public int EstudiantesInscritos { get; set; }

    public string DocenteId { get; set; }

    public bool MostrarDetalles
    {
        get => _mostrarDetalles;
        set
        {
            if (_mostrarDetalles != value)
            {
                _mostrarDetalles = value;
                OnPropertyChanged(nameof(MostrarDetalles));
                OnPropertyChanged(nameof(VerMasTexto));
            }
        }
    }

    public string VerMasTexto => MostrarDetalles ? "Ver Menos" : "Ver Más";

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}




