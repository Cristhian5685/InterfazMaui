//using System.ComponentModel;

//namespace InterfazMaui.Models
//{
//    public class UserModel
//    {


//        public string NombreCompleto { get; set; }
//        public string Email { get; set; }
//        public string Role { get; set; }
//        public string Especialidad { get; set; }

//        // Nueva propiedad para almacenar la URL de la foto de perfil
//        public string FotoPerfilUrl { get; set; }
//        // Propiedad calculada para verificar si es docente
//        public bool IsDocente => Role == "Docente";

//        public bool IsEstudiante => Role == "Estudiante";

//    }

//}


using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace InterfazMaui.Models
{
    public class UserModel : INotifyPropertyChanged
    {
        private string _nombreCompleto;
        private string _email;
        private string _role;
        private string _especialidad;
        private string _fotoPerfilUrl;

        public event PropertyChangedEventHandler PropertyChanged;

        public string NombreCompleto
        {
            get => _nombreCompleto;
            set
            {
                _nombreCompleto = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public string Role
        {
            get => _role;
            set
            {
                _role = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsDocente));
                OnPropertyChanged(nameof(IsEstudiante));
            }
        }

        public string Especialidad
        {
            get => _especialidad;
            set
            {
                _especialidad = value;
                OnPropertyChanged();
            }
        }

        public string FotoPerfilUrl
        {
            get => _fotoPerfilUrl;
            set
            {
                _fotoPerfilUrl = value;
                OnPropertyChanged();
            }
        }

        public bool IsDocente => Role == "Docente";
        public bool IsEstudiante => Role == "Estudiante";

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
