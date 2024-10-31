using System.ComponentModel;

namespace InterfazMaui.Models
{
    public class UserModel
    {

       
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Especialidad { get; set; }

        // Nueva propiedad para almacenar la URL de la foto de perfil
        public string FotoPerfilUrl { get; set; }
        // Propiedad calculada para verificar si es docente
        public bool IsDocente => Role == "Docente";
    }

}
