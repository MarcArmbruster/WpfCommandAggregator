using System;

namespace WPFCommandAggregator
{
    /// <summary>
    /// Command Aggregator exception.    
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
    public class CommandNotDefinedException : Exception
    {
        /// <summary>
        /// Gets or sets the command key.
        /// </summary>
        /// <value>
        /// The command key.
        /// </value>
        public string CommandKey { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandNotDefinedException"/> class.
        /// </summary>
        /// <param name="message">Die Meldung, in der der Fehler beschrieben wird.</param>
        public CommandNotDefinedException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandNotDefinedException"/> class.
        /// </summary>
        /// <param name="commandKey">The command key.</param>
        /// <param name="message">The message.</param>
        public CommandNotDefinedException(string commandKey, string message) : base(message)
        {
            this.CommandKey = commandKey;
        }
    }
}
