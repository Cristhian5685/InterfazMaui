using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfazMaui.Models
{
    class MatriculaModel
    {

        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int Edad { get; set; }
        public string Email { get; set; }
        public string CursoId { get; set; } // ID del curso al que se matricula el estudiante
        public DateTime FechaMatricula { get; set; } = DateTime.Now; // Fecha de matrícula

    }
}
