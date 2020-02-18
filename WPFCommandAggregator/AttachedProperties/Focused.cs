namespace WPFCommandAggregator.AttachedProperties
{
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Attached Property to set the focus to the bound control.
    /// </summary>   
    /// <remarks>
    ///   <para><b>History</b></para>
    ///   <list type="table">
    ///   <item>
    ///   <term><b>Author:</b></term>
    ///   <description>Marc Armbruster</description>
    ///   </item>
    ///   <item>
    ///   <term><b>Date:</b></term>
    ///   <description>Dec/26/2019</description>
    ///   </item>
    ///   <item>
    ///   <term><b>Remarks:</b></term>
    ///   <description>Initial version.</description>
    ///   </item>
    ///   </list>
    /// </remarks>
    public class Focused
    {
        /// <summary>
        /// Gets the IsFocused value.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>The IsFocus value of the object..</returns>
        public static bool GetFocused(DependencyObject obj)
        {
            return (bool)obj.GetValue(FocusedProperty);
        }

        /// <summary>
        /// Sets the IsFocused value.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The value to set.</param>
        public static void SetFocused(DependencyObject obj, bool value)
        {
            obj.SetValue(FocusedProperty, value);
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsFocused.
        /// </summary>
        public static readonly DependencyProperty FocusedProperty =
            DependencyProperty.RegisterAttached(
                "Focused", 
                typeof(bool), 
                typeof(Focused),
                new PropertyMetadata(false, OnFocusedChanged));

        /// <summary>
        /// Handles the changing of the value.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private static void OnFocusedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {       
            var element = sender as UIElement;
            if ((bool)eventArgs.NewValue == true)
            {
                element?.Focus();
            }
            else
            {
                Keyboard.ClearFocus();
            }
        }
    }
}