using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace WPFCommandAggregator.Implementation
{
    /// <summary>
    /// Hierarchy Command. This command type can have child commands.
    /// The command can trigger the child command execution depending on the can-execute strategy.
    /// Hierarchy Command is derived from RelayCommand.
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
    ///   <description>Apr/12/2015</description>
    ///   </item>
    ///   <item>
    ///   <term><b>Remarks:</b></term>
    ///   <description>Initial version.</description>
    ///   </item>
    ///   </list>
    /// </remarks>
    public class HierarchyCommand : RelayCommand
    {
        #region Private Properties

        /// <summary>
        /// The child commands.
        /// </summary>
        private ConcurrentBag<ICommand> childCommands = new ConcurrentBag<ICommand>();

        #endregion Private Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets the child commands. Null value leads to an empty collection.
        /// </summary>
        /// <value>
        /// The child commands.
        /// </value>
        public IEnumerable<ICommand> ChildCommands 
        { 
            get
            {
                return this.childCommands;
            }

            set
            {
                if (value != null)
                {
                    this.childCommands = new ConcurrentBag<ICommand>(value);
                }
                else
                {
                    this.childCommands = new ConcurrentBag<ICommand>();
                }
            }
        }

        /// <summary>
        /// Gets or sets the execute strategy.
        /// </summary>
        /// <value>
        /// The execute strategy.
        /// </value>
        public HierarchyExecuteStrategy ExecuteStrategy { get; set; }

        /// <summary>
        /// Gets or sets the can execute strategy.
        /// </summary>
        /// <value>
        /// The can execute strategy.
        /// </value>
        public HierarchyCanExecuteStrategy CanExecuteStrategy { get; set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyCommand" /> class.
        /// </summary>
        /// <param name="execute">The execute delegate.</param>
        /// <param name="canExecute">The can execute delegate.</param>
        /// <param name="executeStrategy">The execute strategy (default = MasterCommandOnly).</param>
        /// <param name="canExecuteStrategy">The can execute strategy (default = DependsOnAllChilds).</param>
        public HierarchyCommand(
            Action<object> execute, 
            Predicate<object> canExecute, 
            HierarchyExecuteStrategy executeStrategy = HierarchyExecuteStrategy.MasterCommandOnly, 
            HierarchyCanExecuteStrategy canExecuteStrategy = HierarchyCanExecuteStrategy.DependsOnAllChilds) : base(execute, canExecute)
        {
            this.ExecuteStrategy = executeStrategy;
            this.CanExecuteStrategy = canExecuteStrategy;
        }
       
        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyCommand" /> class.
        /// </summary>
        /// <param name="childCommands">The child commands.</param>
        /// <param name="execute">The execute delegate.</param>
        /// <param name="canExecute">The can execute delegate.</param>
        /// <param name="executeStrategy">The execute strategy (default = MasterCommandOnly).</param>
        /// <param name="canExecuteStrategy">The can execute strategy (default = DependsOnAllChilds).</param>
        public HierarchyCommand(
            IEnumerable<ICommand> childCommands,
            Action<object> execute, 
            Predicate<object> canExecute, 
            HierarchyExecuteStrategy executeStrategy = HierarchyExecuteStrategy.MasterCommandOnly, 
            HierarchyCanExecuteStrategy canExecuteStrategy = HierarchyCanExecuteStrategy.DependsOnAllChilds) : base(execute, canExecute)
        {
            this.ExecuteStrategy = executeStrategy;
            this.CanExecuteStrategy = canExecuteStrategy;
            this.ChildCommands = childCommands;
        }

        #endregion Constructors

        #region Private Methods

        /// <summary>
        /// Executes the command via base class (RelayCommand). Ignoring child commands.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        private void ExecuteBase(object parameter)
        {
            base.Execute(parameter);
        }

        /// <summary>
        /// Calls the base CanExecute (RelayCommand). Ignoring child commands.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        private bool CanExecuteBase(object parameter)
        {
            return base.CanExecute(parameter);
        }        

        /// <summary>
        /// Executes the child commands.
        /// </summary>
        /// <param name="parameter">The parameter (will be transfered to child commands).</param>
        private void ExecuteChildCommands(object parameter)
        {
            if (this.ChildCommands == null || this.ChildCommands.Any() == false)
            {
                return;
            }

            foreach (var childCommand in this.ChildCommands.Where(c => c != null).Reverse())
            {
                childCommand.Execute(parameter);
            }
        }

        /// <summary>
        /// Calculates the CanExecute value based on all childs only.
        /// </summary>
        /// <param name="parameter">The parameter (will be transfered to child commands).</param>
        /// <returns>True if all child commands can be executed.</returns>
        private bool CanExecuteAllChildAggregation(object parameter)
        {
            if (this.ChildCommands == null || this.ChildCommands.Any() == false)
            {
                return false;
            }

            return this.ChildCommands.Where(ce => ce != null).All(canExec => canExec.CanExecute(parameter) == true);
        }

        /// <summary>
        /// Calculates the CanExecute value based on all childs only.
        /// </summary>
        /// <param name="parameter">The parameter (will be transfered to child commands).</param>
        /// <returns>True if at least on child command can be executed.</returns>
        private bool CanExecuteAnyChildAggregation(object parameter)
        {
            if (this.ChildCommands == null || this.ChildCommands.Any() == false)
            {
                return false;
            }

            return this.ChildCommands.Where(ce => ce != null).Any(canExec => canExec.CanExecute(parameter) == true);
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Adds the child command (if it is not null).
        /// </summary>
        /// <param name="childCommand">The child command.</param>
        public void AddChildCommand(ICommand childCommand)
        {
            if (childCommand != null)
            {
                this.childCommands.Add(childCommand);               
            }
        }

        /// <summary>
        /// Adds the child commands.
        /// </summary>
        /// <param name="childs">The childs.</param>
        public void AddChildsCommand(IEnumerable<ICommand> childs)
        {
            if (childs != null)
            {
                foreach (var child in childs.Where(c => c != null))
                {
                    this.childCommands.Add(child);
                }
            }
        }
        
        /// <summary>
        /// Clears the commands.
        /// </summary>
        public void ClearCommands()
        {
            this.childCommands = new ConcurrentBag<ICommand>();
        }

        /// <summary>
        /// The Execute method. Calls the given Execute delegate. 
        /// Child commands will be executed based on the defined HierarchyExecuteStrategy value.
        /// </summary>
        /// <param name="parameter">The Execute parameter value.</param>
        public override void Execute(object parameter)
        {
           if (this.ChildCommands == null || this.ChildCommands.Any() == false)
           {
               if (this.ExecuteStrategy != HierarchyExecuteStrategy.AllChildsOnly)
               {
                   this.ExecuteBase(parameter);
               }

               return;
           }

            switch (this.ExecuteStrategy)
            {
                case HierarchyExecuteStrategy.AllChildsOnly:
                    this.ExecuteChildCommands(parameter);
                    break;

                case HierarchyExecuteStrategy.MasterAndAllChilds:
                    this.ExecuteChildCommands(parameter);
                    this.ExecuteBase(parameter);
                    break;

                case HierarchyExecuteStrategy.MasterCommandOnly:
                    this.ExecuteBase(parameter);
                    break;
            }
        }

        /// <summary>
        /// The CanExecute method. Calls the given CanExecute delegate.
        /// Child commands will also be checked based on the defined HierarchyCanExecuteStrategy value.
        /// </summary>
        /// <param name="parameter">The CanExecute parameter value.</param>
        /// <returns>
        /// True, if command can be executed; false otheriwse.
        /// </returns>
        public override bool CanExecute(object parameter)
        {
            if (this.ChildCommands == null || this.ChildCommands.Any() == false)
            {               
                return base.CanExecute(parameter);
            }

            bool canExecuteThisCommand = false;

            switch (this.CanExecuteStrategy)
            {
                case HierarchyCanExecuteStrategy.DependsOnAllChilds:
                    canExecuteThisCommand = this.CanExecuteAllChildAggregation(parameter);
                    break;

                case HierarchyCanExecuteStrategy.DependsOnAtLeastOneChild:
                    canExecuteThisCommand = this.CanExecuteAnyChildAggregation(parameter);
                    break;

                case HierarchyCanExecuteStrategy.DependsOnMasterCommandOnly:
                    canExecuteThisCommand = this.CanExecuteBase(parameter);
                    break;
            }

            return canExecuteThisCommand;
        }

        #endregion Public Methods
    }
}