using InterfazMaui.Views;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace InterfazMaui.Converters
{
    public class BoolToInscribirCommandConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool estaInscrito && parameter is CursosView view)
            {
                return estaInscrito ? view.DesinscribirseCommand : view.InscribirseCommand;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
