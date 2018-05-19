using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFCommandAggregator
{
    /// <summary>
    /// Command Aggregator interface.   
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
    public interface ICommandAggregator
    {
        /// <summary>
        /// Gets the <see cref="ICommand"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="ICommand"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        ICommand this[string key] { get; }

        /// <summary>
        /// Adds the or set command.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="command">The command.</param>
        void AddOrSetCommand(string key, ICommand command);

        /// <summary>
        /// Adds the or set command.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="executeDelegate">The execute delegate.</param>
        /// <param name="canExecuteDelegate">The can execute delegate.</param>
        void AddOrSetCommand(string key, Action<object> executeDelegate, Predicate<object> canExecuteDelegate);

        /// <summary>
        /// Adds or set the command.
        /// </summary>
        /// <param name="key">The command key.</param>
        /// <param name="executeDelegate">The execute delegate assuming it can always be executed (canExecute is true by definition).</param>
        void AddOrSetCommand(string key, Action<object> executeDelegate);

        /// <summary>
        /// Counts this instance.
        /// </summary>
        /// <returns></returns>
        int Count();
        
        /// <summary>
        /// Executes the asynchronous.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        Task ExecuteAsync(string key, object parameter = null);
        
        /// <summary>
        /// Existses the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        bool Exists(string key);
        ICommand GetCommand(string key);
        
        /// <summary>
        /// Determines whether [has null command] [the specified key].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        bool HasNullCommand(string key);
        
        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        void Remove(string key);
        
        /// <summary>
        /// Removes all.
        /// </summary>
        void RemoveAll();
    }
}