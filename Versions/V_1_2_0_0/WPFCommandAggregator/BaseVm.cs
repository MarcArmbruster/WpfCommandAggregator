using System.ComponentModel;

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
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseVm"/> class.
        /// </summary>
        protected BaseVm()
        {
            this.InitCommands();
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

        #region INotifyPropertyChanged Implementation

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
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler == null)
            {
                return;
            }

            var e = new PropertyChangedEventArgs(propertyName);
            handler(this, e);
        }

        #endregion INotifyPropertyChanged Implementation
    }
}