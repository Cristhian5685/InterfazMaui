namespace InterfazMaui.Views;

using Firebase.Database;
using Firebase.Storage;
using InterfazMaui.Models;

public partial class CrearView : ContentPage
{
    private string imageUrl; // Guardará la URL de la imagen subida
    private FileResult selectedImage; // Para almacenar la imagen seleccionada temporalmente
    public CrearView()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false); // Oculta la barra de navegación
    }

    private async void OnSeleccionarImagenClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "Selecciona una imagen"
            });

            if (result != null)
            {
                // Mostrar la imagen seleccionada en la interfaz
                var stream = await result.OpenReadAsync();
                CursoImage.Source = ImageSource.FromStream(() => stream);

                // Subir la imagen a Firebase Storage
                await SubirImagenAFirebase(result);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo seleccionar la imagen: {ex.Message}", "OK");
        }
    }

   

    private async Task SubirImagenAFirebase(FileResult file)
    {
        try
        {
            var firebaseStorage = new FirebaseStorage("cursosmovil-2b6d6.appspot.com");

            // Obtener el stream del archivo
            var stream = await file.OpenReadAsync();

            // Subir la imagen a Firebase Storage
            var imageUrl = await firebaseStorage
                .Child("images")
                .Child(file.FileName)
                .PutAsync(stream);

            this.imageUrl = imageUrl; // Guardar la URL pública de la imagen
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo subir la imagen: {ex.Message}", "OK");
        }
    }
    // Método para limpiar los campos
    private void LimpiarCampos()
    {
        NameEntry.Text = string.Empty;
        DocenteEntry.Text = string.Empty;
        CursoImage.Source = null; // O puedes usar una imagen predeterminada
        DescripcionEditor.Text = string.Empty;
        DuracionEntry.Text = string.Empty;
        NivelPicker.SelectedItem = null; // Limpia la selección del Picker
        RequisitosEditor.Text = string.Empty;
        imageUrl = null; // Resetea la URL de la imagen
    }
    private async void OnCrearCursoClicked(object sender, EventArgs e)
    {
        // Subir la imagen solo si se seleccionó
        if (selectedImage != null)
        {
            await SubirImagenAFirebase(selectedImage);
        }

        // Crear una nueva instancia de Cursos
        var nuevoCurso = new Cursos
        {
            Name = NameEntry.Text,
            Docente = DocenteEntry.Text,
            Image = imageUrl, // Asegúrate de que la imagen se subió y se asignó la URL
            Descripcion = DescripcionEditor.Text,
            Duracion = DuracionEntry.Text,
            Nivel = NivelPicker.SelectedItem?.ToString(),
            Requisitos = RequisitosEditor.Text
        };

        try
        {
            var firebaseClient = new FirebaseClient("https://cursosmovil-2b6d6-default-rtdb.firebaseio.com/");
            // Almacenar el curso en la base de datos y obtener el ID generado
            var curso = await firebaseClient
                .Child("cursos")
                .PostAsync(nuevoCurso);

            await DisplayAlert("Éxito", "El curso ha sido creado con éxito.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo crear el curso: {ex.Message}", "OK");
        }


        LimpiarCampos();

    }
}