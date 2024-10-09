namespace InterfazMaui.Views;
using InterfazMaui.Models;

public partial class CrearView : ContentPage
{
	public CrearView()
	{
		InitializeComponent();
	}

    private void OnCrearCursoClicked(object sender, EventArgs e)
    {
        // Crear una nueva instancia de FairyTale
        var nuevoCurso = new FairyTale
        {
            Name = NameEntry.Text,
            Docente = DocenteEntry.Text,
            Image = ImageEntry.Text,
            Descripcion = DescripcionEditor.Text,
            Duracion = DuracionEntry.Text,
            Nivel = NivelPicker.SelectedItem?.ToString(),
            Requisitos = RequisitosEditor.Text
        };

        // Aquí puedes manejar el nuevo curso, por ejemplo, guardarlo en Firebase o añadirlo a una lista
        // Ejemplo: FirebaseService.AddCourse(nuevoCurso);

        DisplayAlert("Curso Creado", "El curso ha sido creado con éxito.", "OK");
    }
}