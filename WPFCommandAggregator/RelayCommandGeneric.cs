namespace WPFCommandAggregator
{
    using System.Windows.Input;

    using System;

    /// <summary>
    /// Generic Relay Command.
    /// Extended by additional null checks and renamings (M. Armbruster).
    /// </summary>   
    /// <remarks>
    ///   <para><b>History</b></para>
    ///   <list type="table">
    ///   <item>
    ///   <term><b>Author:</b></term>
    ///   <description>See Josh Smith's implementation!</description>
    ///   </item>
    ///   <item>
    ///   <term><b>Date:</b></term>
    ///   <description>Jul/03/2023</description>
    ///   </item>
    ///   <item>
    ///   <term><b>Remarks:</b></term>
    ///   <description>Initial version.</description>
    ///   </item>
    ///   </list>
    /// </remarks>
    public class RelayCommand<T> : ICommand
    {
        /// <summary>
        /// The execute delegate.
        /// </summary>
        protected Action<T> executeDelegate;

        /// <summary>
        /// The can execute delegate.
        /// </summary>
        protected Predicate<T> canExecuteDelegate;

        /// <summary>
        /// The pre action delegate.
        /// </summary>
        protected Action preActionDelegate;

        /// <summary>
        /// The post action delegate.
        /// </summary>
        public Action postActionDelegate;

        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action<T> execute) : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
            {
                this.executeDelegate = new Action<T>(p1 => { });
            }

            this.executeDelegate = execute;
            this.canExecuteDelegate = canExecute;
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        /// <param name="preAction">The pre action.</param>
        /// <param name="postAction">The post action.</param>
        public RelayCommand(
                    Action<T> execute, 
                    Predicate<T> canExecute, 
                    Action preAction, 
                    Action postAction)
        {
            if (execute == null)
            {
                this.executeDelegate = new Action<T>(p1 => { });
            }

            this.executeDelegate = execute;
            this.canExecuteDelegate = canExecute;
            this.preActionDelegate = preAction;
            this.postActionDelegate = postAction;
        }

        /// <summary>
        /// The CanExecute method. Calls the given CanExecute delegate.
        /// </summary>
        /// <param name="parameter">The CanExecute parameter value.</param>
        /// <returns>
        /// True, if command can be executed; false otheriwse.
        /// </returns>
        public virtual bool CanExecute(object parameter)
        {
            return this.canExecuteDelegate == null ? true : this.canExecuteDelegate((T)parameter);
        }

        /// <summary>
        /// Registration of the CanExecute. Listening for changes using the CommandManager.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// The Execute method. Calls the given Execute delegate.
        /// </summary>
        /// <param name="parameter">The Execute parameter value.</param>
        public virtual void Execute(object parameter)
        {
            preActionDelegate?.Invoke();
            executeDelegate?.Invoke((T)parameter);
            postActionDelegate?.Invoke();
        }

        /// <summary>
        /// Overrides the pre action delegate.
        /// </summary>
        /// <param name="preAction">The pre action.</param>
        public void OverridePreActionDelegate(Action preAction)
        {
            this.preActionDelegate = preAction;
        }

        /// <summary>
        /// Overrides the post action delegate.
        /// </summary>
        /// <param name="postAction">The post action.</param>
        public void OverridePostActionDelegate(Action postAction)
        {
            this.postActionDelegate = postAction;
        }
    }
}