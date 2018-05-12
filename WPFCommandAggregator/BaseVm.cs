using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace WPFCommandAggregator
{
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
        private static ConcurrentDictionary<Type, Dictionary<string, List<string>>> propertyDependencies 
            = new ConcurrentDictionary<Type, Dictionary<string, List<string>>>();

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
        private ICommandAggregator cmdAgg = CommandAggregatorFactory.GetNewCommandAggregator();

        /// <summary>
        /// Gets the command aggregate.
        /// </summary>
        /// <value>
        /// The command aggregate.
        /// </value>
        public ICommandAggregator CmdAgg
        {
            get
            {
                return this.cmdAgg;
            }
        }

        /// <summary>
        /// Initializes the commands - has to be overridden in derived classes.
        /// This is the place to register your view model specific commands.
        /// </summary>
        protected abstract void InitCommands();

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
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler == null)
            {
                return;
            }

            var eventArgs = new PropertyChangedEventArgs(propertyName);
            handler(this, eventArgs);
        }

        /// <summary>
        /// Sets the value and raises the property changed event (includes dependend properties).
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="property">The target.</param>
        /// <param name="value">The new value.</param>
        /// <param name="propertyName">Name of the property - automatically resolved by the framework if not explicitly defined.</param>
        protected virtual void SetPropertyValue<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            property = value;
                        
            this.RaisePropertyChangedEvent(propertyName);
            this.RaisePropertyChangedEventForDependendProperties(propertyName);
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
                  "This method will be removed in further versions.")]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.NotifyPropertyChanged(propertyName);
        }

        #endregion INotifyPropertyChanged Implementation
    }
}