using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfazMaui.Models;
using InterfazMaui.Views;

namespace InterfazMaui.Models
{

    public class FairyTale : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Docente { get; set; }
        public string Image { get; set; }
        public string Descripcion { get; set; }
        public string Duracion { get; set; }
        public string Nivel { get; set; }
        public string Requisitos { get; set; }

        private bool _mostrarDetalles;
        public bool MostrarDetalles
        {
            get => _mostrarDetalles;
            set
            {
                _mostrarDetalles = value;
                OnPropertyChanged(nameof(MostrarDetalles));
                OnPropertyChanged(nameof(VerMasTexto));
            }
        }

        public string VerMasTexto => MostrarDetalles ? "Ver menos" : "Ver más";

        public Command ToggleVerMasCommand { get; }

        public FairyTale()
        {
            ToggleVerMasCommand = new Command(() => MostrarDetalles = !MostrarDetalles);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
