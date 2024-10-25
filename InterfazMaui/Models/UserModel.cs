using System.ComponentModel;

namespace InterfazMaui.Models
{
    public class UserModel
    {
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Especialidad { get; set; }

        // Propiedad calculada para verificar si es docente
        public bool IsDocente => Role == "Docente";
    }

}
