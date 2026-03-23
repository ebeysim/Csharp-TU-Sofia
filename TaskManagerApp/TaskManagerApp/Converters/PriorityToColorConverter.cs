using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media;
using TaskManagerApp.Models.Enums;

namespace TaskManagerApp.Converters
{
    public class PriorityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Priority priority)
            {
                return priority switch
                {
                    Priority.Low => Brushes.LightGreen,
                    Priority.Medium => Brushes.LightYellow,
                    Priority.High => Brushes.Orange,
                    Priority.Critical => Brushes.Red,
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