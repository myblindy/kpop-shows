using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Shapes;

namespace kpop_shows.Helpers
{
    public enum Show
    {
        TheShow,
        ShowChampion,
    }

    public class ShowImageConverter : IValueConverter
    {
        private static readonly Dictionary<Show, Rect> Mapping = new Dictionary<Show, Rect>
        {
            [Show.TheShow] = new Rect(0, 133, 139, 133),
            [Show.ShowChampion] = new Rect(139, 133, 139, 133),
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            Mapping[(Show)value];

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
