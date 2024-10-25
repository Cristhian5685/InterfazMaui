using FirebaseAuthNet = Firebase.Auth;
using System;
using Microsoft.Maui.Controls;
using Firebase.Database;
using System.Text.RegularExpressions;
using Firebase.Database.Query;
using InterfazMaui.Models;

namespace InterfazMaui.Views
{
    public partial class RegistrarView : ContentPage
    {
        public RegistrarView()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false); // Oculta la barra de navegaci�n
        }

        private void OnRolePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedRole = rolePicker.SelectedItem?.ToString();

            if (selectedRole == "Docente")
            {
                // Mostrar el Entry de especialidad si el rol es Docente
                specialtyGrid.IsVisible = true;
            }
            else
            {
                // Ocultar el Entry de especialidad si el rol no es Docente
                specialtyGrid.IsVisible = false;
            }
        }


        private async void OnLoginLabelTapped(object sender, EventArgs e)
        {
            // Aqu� navegas a la p�gina de login
            await Navigation.PushAsync(new LoginView());
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            string nombreCompleto = nombreCompletoEntry.Text;
            string email = emailEntry.Text;
            string password = passwordEntry.Text;
            string confirmPassword = confirmPasswordEntry.Text;
            string selectedRole = rolePicker.SelectedItem?.ToString() ?? string.Empty; // Obtener el rol seleccionado
            string especialidad = especialidadEntry.Text; // Obtener la especialidad


            if (string.IsNullOrEmpty(nombreCompleto) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword) || string.IsNullOrEmpty(selectedRole))
            {
                await DisplayAlert("Error", "Todos los campos son obligatorios, incluyendo el rol.", "OK");
                return;
            }

            if (selectedRole == "Docente" && string.IsNullOrEmpty(especialidad))
            {
                await DisplayAlert("Error", "Por favor ingrese su especialidad.", "OK");
                return;
            }

            if (!IsValidEmail(email))
            {
                await DisplayAlert("Error", "El formato del correo electr�nico no es v�lido.", "OK");
                return;
            }

            if (password != confirmPassword)
            {
                await DisplayAlert("Error", "Las contrase�as no coinciden.", "OK");
                return;
            }

            try
            {
                // Crear el usuario usando Firebase Authentication
                var authProvider = new FirebaseAuthNet.FirebaseAuthProvider(new FirebaseAuthNet.FirebaseConfig("AIzaSyDHQIM8REv5OKqzIMJ-vc_jwyh2viBZqBY"));
                var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(email, password, nombreCompleto, true);
                var user = auth.User;

                if (user != null)
                {
                    await DisplayAlert("�xito", $"Usuario creado exitosamente: {user.Email}", "OK");

                    // Crear el modelo de usuario y almacenar los datos
                    UserModel newUser = new UserModel
                    {
                        NombreCompleto = nombreCompleto,
                        Email = user.Email,
                        Role = selectedRole,
                        Especialidad = selectedRole == "Docente" ? especialidad : null // Solo guardar especialidad si es docente
                    };

                    // Almacenar datos adicionales en Realtime Database, incluyendo el rol
                    var firebaseClient = new FirebaseClient("https://cursosmovil-2b6d6-default-rtdb.firebaseio.com/"); // URL de tu base de datos
                    await firebaseClient
                        .Child("users") // Puedes cambiar "users" al nombre que prefieras
                        .Child(user.LocalId) // Usar el ID del usuario como clave
                         .PutAsync(newUser); // Guardar el modelo completo en la base de datos

                    // Redirigir al usuario a la p�gina correspondiente seg�n el rol
                    if (selectedRole == "Docente")
                    {
                        await Navigation.PushAsync(new MainTabbedPageDocente()); // Navega a la p�gina Docente
                    }
                    else if (selectedRole == "Estudiante")
                    {
                        await Navigation.PushAsync(new MainTabbedPageEstudiante()); // Navega a la p�gina Estudiante
                    }
                }
            }
            catch (FirebaseAuthNet.FirebaseAuthException ex)
            {
                // Manejar errores de Firebase con mensajes m�s espec�ficos
                string errorMessage = GetFirebaseErrorMessage(ex.Reason);
                await DisplayAlert("Error", errorMessage, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error inesperado: {ex.Message}", "OK");
            }
        }

        private string GetFirebaseErrorMessage(FirebaseAuthNet.AuthErrorReason reason)
        {
            return reason switch
            {
                FirebaseAuthNet.AuthErrorReason.WeakPassword => "La contrase�a es demasiado d�bil.",
                FirebaseAuthNet.AuthErrorReason.InvalidEmailAddress => "El formato del correo electr�nico no es v�lido.",
                FirebaseAuthNet.AuthErrorReason.EmailExists => "Ya existe una cuenta con este correo electr�nico.",
                _ => $"Error al crear el usuario: {reason.ToString()}" // Mensaje gen�rico para otros errores
            };
        }
    }
}
