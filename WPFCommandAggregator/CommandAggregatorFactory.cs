using System.Collections.Generic;
using System.Windows.Input;

namespace WPFCommandAggregator
{
    /// <summary>
    /// Factory class for new command aggregator instances.
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
    ///   <description>Apr/11/2015</description>
    ///   </item>
    ///   <item>
    ///   <term><b>Remarks:</b></term>
    ///   <description>Initial version.</description>
    ///   </item>
    ///   </list>
    /// </remarks>
    public sealed class CommandAggregatorFactory
    {
        /// <summary>
        /// Gets the new command aggregator.
        /// </summary>
        /// <returns></returns>
        public static ICommandAggregator GetNewCommandAggregator()
        {
            return new CommandAggregator();
        }

        /// <summary>
        /// Gets the new command aggregator.
        /// </summary>
        /// <param name="commands">The commands.</param>
        /// <returns></returns>
        public static ICommandAggregator GetNewCommandAggregator(IEnumerable<KeyValuePair<string, ICommand>> commands)
        {
            return new CommandAggregator(commands);
        }
    }
}