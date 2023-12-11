namespace WPFCommandAggregator
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Base View Model
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
    ///   <description>Dec/23/2014</description>
    ///   </item>
    ///   <item>
    ///   <term><b>Remarks:</b></term>
    ///   <description>Initial version.</description>
    ///   </item>
    ///   </list>
    /// </remarks>
    public abstract class BaseVm : INotifyPropertyChanged
    {
        /// <summary>
        /// Global (static) storage for property dependencies.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Dictionary<string, List<string>>> propertyDependencies 
            = new ConcurrentDictionary<Type, Dictionary<string, List<string>>>();

        /// <summary>
        /// Dictionary to hold private values of bindable properties.
        /// </summary>
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();

        /// <summary>
        /// Property to suppress notifications.
        /// Set to true if you want to supress all notifications.
        /// Default is false (active notifications).
        /// </summary>
        protected bool SuppressNotifications {get; set; }

        /// <summary>
        /// This property is used by the attached property 'WindowCloser'. 
        /// If this attached property is attached to a window, this WindowResult property can be used to close the window 
        /// from this current view model by setting value to true or false. A null value will not close the window.
        /// </summary>
        public bool? WindowResult 
        {
            get => this.GetPropertyValue<bool?>();
            set => this.SetPropertyValue<bool?>(value);
        }

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseVm"/> class.
        /// </summary>
        protected BaseVm()
        {
            this.InitCommands();
            this.ReadPropertyDependencies();
        }

        #endregion Constructor        

        #region WPF Command Aggregator

        /// <summary>
        /// The command aggregate.
        /// </summary>
        private readonly ICommandAggregator cmdAgg = CommandAggregatorFactory.GetNewCommandAggregator();

        /// <summary>
        /// Gets the command aggregate.
        /// </summary>
        /// <value>
        /// The command aggregate.
        /// </value>
        public ICommandAggregator CmdAgg => this.cmdAgg;

        /// <summary>
        /// Initializes the commands - has to be overridden in derived classes.
        /// This is the place to register your view model specific commands.
        /// </summary>
        protected virtual void InitCommands()
        {
            // this method should be used to register commands
        }

        #endregion WPF Command Aggregator

        #region Dependencies

        /// <summary>
        /// Reads the property dependencies and sores it in the static dictionary.
        /// </summary>
        private void ReadPropertyDependencies()
        {
            Type type = this.GetType();
            if (propertyDependencies.ContainsKey(type))
            {
                return;
            }

            propertyDependencies[type] = new Dictionary<string, List<string>>();

            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (var propInfo in propertyInfos)
            {
                
                object[] attributes = propInfo.GetCustomAttributes(true);
                foreach (DependsOnAttribute attribute in attributes.Where(a => a is DependsOnAttribute))
                {
                    var names = attribute.PropertyNames.ToList();
                    propertyDependencies[type].Add(propInfo.Name, names);
                }
            }
        }

        /// <summary>
        /// Raises the property changed event for all dependend properties.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        private void RaisePropertyChangedEventForDependendProperties(string propertyName)
        {
            var dependencies = propertyDependencies[this.GetType()];
            var keysOfDependencies = dependencies.Where(d => d.Value.Contains(propertyName)).Select(d => d.Key);
            keysOfDependencies.ToList().ForEach(depName => this.RaisePropertyChangedEvent(depName));
        }

        #endregion Dependencies

        #region INotifyPropertyChanged Implementation

        /// <summary>
        /// Tritt ein, wenn sich ein Eigenschaftenwert ändert.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="propertyName"></param>
        private void RaisePropertyChangedEvent(string propertyName)
        {            
            if (this.SuppressNotifications)
            {
                return;
            }

            if (PropertyChanged == null)
            {
                return;
            }

            var eventArgs = new PropertyChangedEventArgs(propertyName);
            PropertyChanged(this, eventArgs);
        }

        /// <summary>
        /// Sets the value and raises the property changed event (includes dependend properties) - if effective value is changed.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="property">The target.</param>
        /// <param name="value">The new value.</param>
        /// <param name="propertyName">Name of the property - automatically resolved by the framework if not explicitly defined.</param>
        protected virtual void SetPropertyValue<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            bool notify = !Equals(property, value) && !this.SuppressNotifications;
            property = value;

            if (notify)
            {
                this.RaisePropertyChangedEvent(propertyName);
                this.RaisePropertyChangedEventForDependendProperties(propertyName);
            }
        }

        /// <summary>
        /// Sets the value and raises the property changed event (includes dependend properties) - if effective value is changed.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="property">The target.</param>
        /// <param name="value">The new value.</param>
        /// <param name="preSetAction">An Action that will execute before set and notification is done.</param>
        /// <param name="postSetAction">An Action that will execute after set and notification is done.</param>
        /// <param name="propertyName">Name of the property - automatically resolved by the framework if not explicitly defined.</param>
        protected virtual void SetPropertyValue<T>(ref T property, T value, Action preSetAction,  Action postSetAction, [CallerMemberName] string propertyName = null)
        {
            preSetAction?.Invoke();
            this.SetPropertyValue<T>(ref property, value, propertyName);
            postSetAction?.Invoke();            
        }

        /// <summary>
        /// Sets the value by using the automatic private values storage and raises the property changed event (includes dependend properties) - if effective value is changed.
        /// </summary>
        /// <remarks>
        /// Using automatic values storage - DO NOT combine with private fields fior bindable properties in view models.
        /// </remarks>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="value">The new value.</param>
        /// <param name="propertyName">Name of the property - automatically resolved by the framework if not explicitly defined.</param>
        protected void SetPropertyValue<T>(T value, [CallerMemberName] string propertyName = "")
        {
            bool notify = !Equals(value, GetPropertyValue<T>(propertyName)) && !this.SuppressNotifications;

            this.values[propertyName] = value;

            if (notify)
            {
                this.RaisePropertyChangedEvent(propertyName);
                this.RaisePropertyChangedEventForDependendProperties(propertyName);
            }
        }

        /// <summary>
        /// Sets the value by using the automatic private values storage and raises the property changed event (includes dependend properties) - if effective value is changed.
        /// </summary>
        /// <remarks>
        /// Using automatic values storage - DO NOT combine with private fields fior bindable properties in view models.
        /// </remarks>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="preSetAction">An Action that will execute before set and notification is done.</param>
        /// <param name="postSetAction">An Action that will execute after set and notification is done.</param>
        /// <param name="value">The new value.</param>
        /// <param name="propertyName">Name of the property - automatically resolved by the framework if not explicitly defined.</param>
        protected void SetPropertyValue<T>(T value, Action preSetAction, Action postSetAction, [CallerMemberName] string propertyName = "")
        {            
            preSetAction?.Invoke();
            this.SetPropertyValue(value, propertyName);
            postSetAction?.Invoke();
        }

        /// <summary>
        /// Gets the property value by using the automatic private values storage.
        /// </summary>
        /// <remarks>
        /// Using automatic values storage - DO NOT combine with private fields fior bindable properties in view models.
        /// </remarks>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="propertyName">Name of the property - automatically resolved by the framework if not explicitly defined.</param>
        /// <returns>The value of the property. If not found, the default value of the type will be returned.</returns>
        protected T GetPropertyValue<T>([CallerMemberName] string propertyName = "")
        {
            if (!this.values.ContainsKey(propertyName))
            {
                this.values[propertyName] = default(T);
            }

            return (T)this.values[propertyName];
        }

        /// <summary>
        /// Calls the property changed event (includes dependend properties).
        /// </summary>
        /// <param name="propertyName">Name of the property - automatically resolved by the framework if not explicitly defined.</param>
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.RaisePropertyChangedEvent(propertyName);
            this.RaisePropertyChangedEventForDependendProperties(propertyName);
        }

        /// <summary>
        /// Called when property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [Obsolete("Please use method SetPropertyValue or NotifyPropertyChanged instead. " +
                  "This method will be removed in further versions (up from nuGet package version 1.6.)")]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.NotifyPropertyChanged(propertyName);
        }

        #endregion INotifyPropertyChanged Implementation
    }
}