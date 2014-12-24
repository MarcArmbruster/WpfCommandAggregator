using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CommandAggregatorExample.CmdAgg
{
    /// <summary>
    /// Command Aggregator class. 
    /// Maintaining a bag of ICommand objects, identified by a string key including get and set methods, added by an indexer access (for easy binding usage).
    /// GetMethod never returns null. If command key does not exist or command is null a dummy command/delegate is returned (doing nothing).
    /// This avoids null reference exceptions.
    /// The method 'Exists' can be used to check for the availability of a command.
    /// The method 'HasNullCommand' can be used for an explicit check for null command.
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
    public class CommandAggregator
    {
        #region Private Members

        /// <summary>
        /// The private (thread save) collection of commands.
        /// </summary>
        private readonly ConcurrentDictionary<string, ICommand> commands;

        #endregion Private Members

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAggregator"/> class.
        /// </summary>
        public CommandAggregator()
        {
            commands = new ConcurrentDictionary<string, ICommand>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAggregator"/> class.
        /// </summary>
        /// <param name="commands">The commands.</param>
        public CommandAggregator(IEnumerable<KeyValuePair<string, ICommand>> commands) : this()
        {
            foreach (KeyValuePair<string, ICommand> item in commands)
            {
                this.AddOrSetCommand(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="CommandAggregator"/> class.
        /// </summary>
        ~CommandAggregator()
        {
            this.commands.Clear();           
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Adds or set the command.
        /// </summary>
        /// <param name="key">The command key.</param>
        /// <param name="command">The command.</param>
        public void AddOrSetCommand(string key, ICommand command)
        {
            if (this.commands.Any(k => k.Key == key))
            {
                this.commands[key] = command;
            }
            else
            {
                this.commands.AddOrUpdate(key, command, (exkey, excmd) => command);
            }
        }

        /// <summary>
        /// Adds or set the command.
        /// </summary>
        /// <param name="key">The command key.</param>
        /// <param name="executeDelegate">The execute delegate.</param>
        /// <param name="canExecuteDelegate">The can execute delegate.</param>
        public void AddOrSetCommand(string key, Action<object> executeDelegate, Predicate<object> canExecuteDelegate)
        {
            if (this.commands.Any(k => k.Key == key))
            {
                this.commands[key] = new RelayCommand(executeDelegate, canExecuteDelegate);
            }
            else
            {
                ICommand command = new RelayCommand(executeDelegate, canExecuteDelegate);
                this.commands.AddOrUpdate(key, command, (exkey, excmd) => command);
            }
        }

        /// <summary>
        /// Gets the command. If command not exists, a dummy Action delegate will be returned (doing nothing).
        /// </summary>
        /// <param name="key">The command key.</param>
        /// <returns>The command for the given key (Empty command if not found/exists).</returns>
        public ICommand GetCommand(string key)
        {           
            if (this.commands.Any(k => k.Key == key))
            {
                return this.commands[key];
            }
            else
            {
                // Empty command (to avoid null reference exceptions)
                return new RelayCommand(p1 => { });
            }           
        }

        /// <summary>
        /// Gets the <see cref="ICommand"/> with the specified key.
        /// Indexer access is important for usage in XAML DataBindings.
        /// Please Note: indexers can only be used for OneWay-Mode in 
        /// XAML-Bindings (read only).
        /// </summary>
        /// <value>
        /// The <see cref="ICommand"/>.
        /// </value>
        /// <param name="key">The command key.</param>
        /// <returns>The command for the given key (Empty command if not found/exists).</returns>
        public ICommand this[string key]
        {
            get
            {
                // The indexer is using the GetCommand-Method
                return this.GetCommand(key);
            }
        }

        /// <summary>
        /// Removes the ICommand corresponding the specified key.
        /// </summary>
        /// <param name="key">The command key.</param>
        public void Remove(string key)
        {
            lock (this.commands)
            {
                if (this.commands.Any(k => k.Key == key))
                {
                    ICommand oldCommand;
                    this.commands.TryRemove(key, out oldCommand);
                }
            }
        }

        /// <summary>
        /// Removes all ICommands = Clear-Functionality.
        /// </summary>
        public void RemoveAll()
        {
            if (this.commands != null)
            {
                lock (this.commands)
                {
                    this.commands.Clear();
                }
            }
        }

        /// <summary>
        /// Checks the existance of a command for the given key.
        /// </summary>
        /// <param name="key">The command key.</param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            lock (this.commands)
            {
                return this.commands.Any(k => k.Key == key);            
            }            
        }

        /// <summary>
        /// Determines whether the ICommand corresponding the specified key is null.
        /// </summary>
        /// <param name="key">The command key.</param>
        /// <returns>True if ICommand is null, false otherwise.</returns>
        /// <exception cref="CommandNotDefinedException">Command with this key is not registered, yet.</exception>
        public bool HasNullCommand(string key)
        {
            if (!this.Exists(key))
            {
                throw new CommandNotDefinedException(key, "Command with such a key is actually not registered.");
            }

            return this.commands.FirstOrDefault(k => k.Key == key).Value == null;
        }

        /// <summary>
        /// Executes the command asynchronous.
        /// IMPORTANT: This assumes the possible asynchronous call/execution of the action delegate!
        /// </summary>
        /// <param name="key">The command key.</param>
        /// <param name="parameter">The optional parameter.</param>
        /// <returns>
        /// The created Task.
        /// </returns>
        /// <exception cref="CommandNotDefinedException">Command with such a key is actually not registered.</exception>
        public Task ExecuteAsync(string key, object parameter = null)
        {
            if (!this.Exists(key))
            {
                throw new CommandNotDefinedException(key, "Command with such a key is actually not registered.");
            }

            return Task.Factory.StartNew(() => this.GetCommand(key).Execute(parameter));
        }

        #endregion Methods
    }
}
