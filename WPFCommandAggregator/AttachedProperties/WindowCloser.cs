namespace WPFCommandAggregator.AttachedProperties
{
    using System.Windows;

    /// <summary>
    /// Attached Property to close windows out of there's view model (DataContext).
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
    public class WindowCloser
    {
        /// <summary>
        /// Sets the WindowResult attached property value.
        /// </summary>
        /// <param name="obj">The target object.</param>
        /// <returns></returns>
        public static bool? GetWindowResult(DependencyObject obj)
        {
            return (bool?)obj.GetValue(WindowResultProperty);
        }

        /// <summary>
        /// Sets the WindowResult attached property value.
        /// </summary>
        /// <param name="obj">The target object.</param>
        /// <param name="value">The new value.</param>
        public static void SetWindowResult(DependencyObject obj, bool? value)
        {
            obj.SetValue(WindowResultProperty, value);
        }

        /// <summary>
        /// This attached property can be used to close a window from its view model. 
        /// If the property WindowResult (see BaseVm class) property is bound to this attached property 
        /// and setting its value to true or false, the window will be closed. 
        /// A null value will not close the window. Null represents the window is still available state.
        /// </summary>
        public static readonly DependencyProperty WindowResultProperty =
            DependencyProperty.RegisterAttached(
                "WindowResult", 
                typeof(bool?),
                typeof(WindowCloser), 
                new PropertyMetadata(null, OnWindowResultChanged));

        /// <summary>
        /// Handles the value changed event. 
        /// Closes the window if new value is true or false.
        /// </summary>
        /// <param name="sender">The window.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private static void OnWindowResultChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            var window = sender as Window;
            if (eventArgs.NewValue != null)
            {
                window?.Close();
            }
        }
    }
}