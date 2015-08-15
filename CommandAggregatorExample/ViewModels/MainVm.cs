using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using WPFCommandAggregator.Implementation;

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
        /// The save all checker.
        /// </summary>
        private PerformanceChecker printOutChecker = new PerformanceChecker("PrintOut");

        /// <summary>
        /// The can save1 value.
        /// </summary>
        private bool canSave1;

        /// <summary>
        /// The can save2 value.
        /// </summary>
        private bool canSave2;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainVm"/> class.
        /// </summary>
        public MainVm()
        {           
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can save1.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can save1; otherwise, <c>false</c>.
        /// </value>
        public bool CanSave1
        {
            get
            {
                return this.canSave1;
            }

            set
            {
                this.canSave1 = value;
                this.OnPropertyChanged("CanSave1");                
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can save2.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can save2; otherwise, <c>false</c>.
        /// </value>
        public bool CanSave2
        {
            get
            {
                return this.canSave2;
            }

            set
            {
                this.canSave2 = value;
                this.OnPropertyChanged("CanSave2");                
            }
        }

        /// <summary>
        /// Initializes the commands.
        /// </summary>
        protected sealed override void InitCommands()
        {
            // AddOrSetCommand method is overridden --> provide ICommad or Action / Predicate delegates
            this.CmdAgg.AddOrSetCommand("Exit", new RelayCommand(p1 => MessageBox.Show("Exit called"), p2 => true));
            this.CmdAgg.AddOrSetCommand("Print", new RelayCommand(p1 => MessageBox.Show("Print called"), p2 => true, this.printOutChecker.Start, this.printOutChecker.Stop));
            this.CmdAgg.AddOrSetCommand("Options", new RelayCommand(p1 => MessageBox.Show("Options called"), p2 => true));
            this.CmdAgg.AddOrSetCommand("About", new RelayCommand(p1 => MessageBox.Show("About" + (p1 == null ? string.Empty : " [" + p1 + "]") + " called"), p2 => true));
            this.CmdAgg.AddOrSetCommand("AboutAsnyc", new RelayCommand(p1 => OpenAboutAsync(p1), p2 => true));

            // Adding a hierarchy command
            ICommand save1Cmd = new RelayCommand(new Action<object>(p1 => MessageBox.Show("Save 1 called")), new Predicate<object>(p2 => this.CanSave1));
            ICommand save2Cmd = new RelayCommand(new Action<object>(p1 => MessageBox.Show("Save 2 called")), new Predicate<object>(p2 => this.CanSave2));
            this.CmdAgg.AddOrSetCommand("SaveCmd1", save1Cmd);
            this.CmdAgg.AddOrSetCommand("SaveCmd2", save2Cmd);

            HierarchyCommand saveAllCmd = new HierarchyCommand(
                p1 => { },
                null,
                HierarchyExecuteStrategy.AllChildsOnly,
                HierarchyCanExecuteStrategy.DependsOnAllChilds);

            saveAllCmd.AddChildsCommand(new List<ICommand> { save1Cmd, save2Cmd });
            this.CmdAgg.AddOrSetCommand("SaveAll", saveAllCmd);            
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
