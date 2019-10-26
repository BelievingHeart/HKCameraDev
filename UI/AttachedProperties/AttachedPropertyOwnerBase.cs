
using System.Windows;

namespace UI.AttachedProperties
{
      /// <summary>
    /// A base class for attached property owner class to re-implement
    /// The purpose of this class is to have an attached property automatically owned by the inheriting class by re-implementing it
    /// All the inheriting class has to do is to override <see cref="OnPropertyChanged"/>
    /// </summary>
    /// <typeparam name="AttachedPropertyOwnerType"></typeparam>
    /// <typeparam name="AttachedPropertyType"></typeparam>
    public abstract class AttachedPropertyOwnerBase<AttachedPropertyOwnerType, AttachedPropertyType> where AttachedPropertyOwnerType : AttachedPropertyOwnerBase<AttachedPropertyOwnerType, AttachedPropertyType>, new()
    {
        #region Properties

        /// <summary>
        /// Static object to allow providing the ability to override callbacks
        /// </summary>
        private static AttachedPropertyOwnerType Instance { get; } = new AttachedPropertyOwnerType();

        #endregion

        #region Attached Property

        public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(
            "Value", typeof(AttachedPropertyType), typeof(AttachedPropertyOwnerBase<AttachedPropertyOwnerType, AttachedPropertyType>), new PropertyMetadata(PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Instance.OnPropertyChanged(d, e);
        }

        public static void SetValue(DependencyObject element, AttachedPropertyType value)
        {
            element.SetValue(ValueProperty, value);
        }

        public static AttachedPropertyType GetValue(DependencyObject element)
        {
            return (AttachedPropertyType) element.GetValue(ValueProperty);
        }

        #endregion

        /// <summary>
        /// Callback that fires when ValueProperty changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public abstract void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e);
    }
}