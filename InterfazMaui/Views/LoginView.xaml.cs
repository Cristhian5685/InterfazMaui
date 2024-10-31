using FirebaseAuthNet = Firebase.Auth;
using Firebase.Database;
using System;
using System.Linq;
using Microsoft.Maui.Controls;
using System.Text.RegularExpressions;
using Firebase.Database.Query;

namespace InterfazMaui.Views;

public partial class LoginView : ContentPage
{
	public LoginView()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Oculta la barra de navegación
    }

    private async void OnLoginLabelTapped(object sender, EventArgs e)
    {
        // Aquí navegas a la página de login
        await Navigation.PushAsync(new RegistrarView());
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        string email = emailEntry.Text;
        string password = passwordEntry.Text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Error", "Por favor, ingresa el correo electrónico y la contraseña.", "OK");
            return;
        }

        try
        {
            // Autenticar al usuario con Firebase
            var authProvider = new FirebaseAuthNet.FirebaseAuthProvider(new FirebaseAuthNet.FirebaseConfig("AIzaSyDHQIM8REv5OKqzIMJ-vc_jwyh2viBZqBY"));
            var auth = await authProvider.SignInWithEmailAndPasswordAsync(email, password);
            var user = auth.User;

            if (user != null)
            {

                // Almacena el UID en las preferencias
                Preferences.Set("UserId", user.LocalId);

                // Conectarse a la base de datos Firebase para obtener el rol del usuario
                var firebaseClient = new FirebaseClient("https://cursosmovil-2b6d6-default-rtdb.firebaseio.com/");
                var userData = await firebaseClient
                    .Child("users")
                    .Child(user.LocalId)
                    .OnceSingleAsync<UserData>();

                if (userData != null)
                {
                    // Redirigir al usuario según su rol
                    if (userData.Role == "Docente")
                    {
                        await Navigation.PushAsync(new MainTabbedPageDocente());
                    }
                    else if (userData.Role == "Estudiante")
                    {
                        await Navigation.PushAsync(new MainTabbedPageEstudiante());
                    }
                    else
                    {
                        await DisplayAlert("Error", "Rol de usuario no reconocido.", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "No se encontraron datos adicionales del usuario.", "OK");
                }
            }
        }
        catch (FirebaseAuthNet.FirebaseAuthException ex)
        {
            await DisplayAlert("Error", $"Error al iniciar sesión: {ex.Reason}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error inesperado: {ex.Message}", "OK");
        }
    }


    private async void OnForgotPasswordTapped(object sender, EventArgs e)
    {
        string email = await DisplayPromptAsync("Recuperar Contraseña", "Ingresa tu correo electrónico:");

        if (!string.IsNullOrEmpty(email))
        {
            try
            {
                // Proveedor de autenticación de Firebase
                var authProvider = new FirebaseAuthNet.FirebaseAuthProvider(new FirebaseAuthNet.FirebaseConfig("AIzaSyDHQIM8REv5OKqzIMJ-vc_jwyh2viBZqBY"));

                // Solicitud de restablecimiento de contraseña
                await authProvider.SendPasswordResetEmailAsync(email);
                await DisplayAlert("Éxito", "Se ha enviado un correo para restablecer tu contraseña.", "OK");
            }
            catch (FirebaseAuthNet.FirebaseAuthException ex)
            {
                await DisplayAlert("Error", $"Error al enviar el correo de recuperación: {ex.Message}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            }
        }
        else
        {
            await DisplayAlert("Error", "El correo no puede estar vacío.", "OK");
        }
    }


    public class UserData
    {
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }

}