using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace UI.Converters
{
    /// <summary>
    /// Classes that inherit this class don't need to provide singleton instance when using in XAML
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ValueConverterBase<T> : MarkupExtension, IValueConverter
        where T : class, new()
    {
        /// <summary>
        /// Static converter instance that is going to used for every EnumToPage conversion
        /// </summary>
        private static T _converterInstance = null;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converterInstance ?? (_converterInstance = new T());
        }

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }
}