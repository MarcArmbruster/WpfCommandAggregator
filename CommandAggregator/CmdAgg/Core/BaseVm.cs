using CommandAggregatorExample.CmdAgg;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace CommandAggregatorExample
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
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseVm"/> class.
        /// </summary>
        protected BaseVm()
        {
            this.InitCommands();
        }

        #endregion Constructor        

        /// <summary>
        /// The command aggregate.
        /// </summary>
        private CommandAggregator cmdAgg = new CommandAggregator();

        /// <summary>
        /// Gets the command aggregate.
        /// </summary>
        /// <value>
        /// The command aggregate.
        /// </value>
        public CommandAggregator CmdAgg
        {
            get
            {
                return this.cmdAgg;
            }
        }

        /// <summary>
        /// Initializes the commands.
        /// </summary>
        protected abstract void InitCommands();

        /// <summary>
        /// Verifies the name of the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,             
            // public, instance property on this object.             
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;
                Debug.Fail(msg);
                throw new Exception(msg);                
            }
        }
       
        /// <summary>
        /// Tritt ein, wenn sich ein Eigenschaftenwert ändert.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler == null)
            {
                return;
            }

            var e = new PropertyChangedEventArgs(propertyName);
            handler(this, e);
        }
    }
}
