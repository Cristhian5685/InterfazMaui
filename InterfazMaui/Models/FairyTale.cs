using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfazMaui.Models
{
    public class FairyTale
    {
        public string Name { get; set; }
        public string Docente { get; set; }
        public string Image { get; set; }
        public string Descripcion { get; set; }  // Nueva propiedad para la descripción
        public string Duracion { get; set; }  // Duración del curso
        public string Nivel { get; set; }  // Nivel del curso, por ejemplo: "Básico", "Intermedio", "Avanzado"
        public string Requisitos { get; set; }

        public bool MostrarDetalles { get; set; } = false;



    }
}
