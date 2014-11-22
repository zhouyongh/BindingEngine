namespace Illusion.Sample
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using Illusion.Utility;

    /// <summary>
    ///     Interaction logic for WPFControl.xaml
    /// </summary>
    public partial class WPFControl : UserControl
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WPFControl" /> class.
        /// </summary>
        public WPFControl()
        {
            this.InitializeComponent();
        }

        #endregion
    }

    public class HashCodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value == DependencyProperty.UnsetValue || !(value is Person))
            {
                return Binding.DoNothing;
            }
            return value.GetHashCode();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}