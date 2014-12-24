using System;
using System.Windows;
using WPFCommandAggregator;

namespace CommandAggregatorExample.ViewModels
{
    /// <summary>
    /// View Model of MainWindow.
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
    public class MainVm : BaseVm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainVm"/> class.
        /// </summary>
        public MainVm()
        {           
        }

        /// <summary>
        /// Initializes the commands.
        /// </summary>
        protected sealed override void InitCommands()
        {
            // AddOrSetCommand method is overridden --> provide ICommad or Action / Predicate delegates
            this.CmdAgg.AddOrSetCommand("Exit", new RelayCommand(p1 => MessageBox.Show("Exit called"), p2 => true));
            this.CmdAgg.AddOrSetCommand("Save", new Action<object>(p1 => MessageBox.Show("Save called")), new Predicate<object>(p2 => true));
            this.CmdAgg.AddOrSetCommand("Print", new RelayCommand(p1 => MessageBox.Show("Print called"), p2 => true));
            this.CmdAgg.AddOrSetCommand("Options", new RelayCommand(p1 => MessageBox.Show("Options called"), p2 => true));
            this.CmdAgg.AddOrSetCommand("About", new RelayCommand(p1 => MessageBox.Show("About " + (p1 == null ? string.Empty : "[" + p1 + "]") +" called"), p2 => true));
            this.CmdAgg.AddOrSetCommand("AboutAsnyc", new RelayCommand(p1 => OpenAboutAsync(p1), p2 => true));
        }

        /// <summary>
        /// Opens the about message asynchronous.
        /// </summary>
        /// <param name="cmdParameter">The command parameter.</param>
        private void OpenAboutAsync(object cmdParameter)
        {            
            this.CmdAgg.ExecuteAsync("About", cmdParameter);
        }        
    }
}
