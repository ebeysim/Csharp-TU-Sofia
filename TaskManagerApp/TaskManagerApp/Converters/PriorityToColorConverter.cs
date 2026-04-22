using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
//using System.Windows.Media;
using TaskManagerApp.Models.Enums;

namespace TaskManagerApp.Converters
{
    public class PriorityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bc = new BrushConverter();
            if (value is Priority priority)
            {
                return priority switch
                {
                    Priority.Low => (Brush)bc.ConvertFrom("#27AE60"),
                    Priority.Medium => (Brush)bc.ConvertFrom("#F1C40F"),
                    Priority.High => (Brush)bc.ConvertFrom("#E67E22"),
                    Priority.Critical => (Brush)bc.ConvertFrom("#C0392B"),
                    _ => Brushes.Transparent
                };
            }

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}