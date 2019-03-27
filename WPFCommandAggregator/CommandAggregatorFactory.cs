using System;
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
        private static Type externalAggregatorType = null;

        /// <summary>
        /// Registers the given, custom command aggregator type.
        /// </summary>
        /// <typeparam name="T">Type of custom aggregator implementation.</typeparam>        
        public static void RegisterAggreagtorImplementation<T>() where T : ICommandAggregator
        {
            externalAggregatorType = typeof(T);
        }

        /// <summary>
        /// Unregister the given, custom command aggregator type.
        /// </summary>
        /// <typeparam name="T">Type of own aggregator implementation.</typeparam>
        public static void UnregisterAggreagtorImplementation<T>() where T : ICommandAggregator
        {
            externalAggregatorType = null;
        }
        
        /// <summary>
        /// Gets the new command aggregator.
        /// </summary>
        /// <returns></returns>
        public static ICommandAggregator GetNewCommandAggregator()
        {
            if (externalAggregatorType != null)
            {
                ICommandAggregator aggregator = Activator.CreateInstance(externalAggregatorType) as ICommandAggregator;
                if (aggregator == null)
                {
                    throw new InvalidCastException("Registered aggregator type could not handled as a valid command aggregator");
                }

                return aggregator;
            }

            return new CommandAggregator();
        }

        /// <summary>
        /// Gets the new command aggregator.
        /// </summary>
        /// <param name="commands">The commands.</param>
        /// <returns></returns>
        public static ICommandAggregator GetNewCommandAggregator(IEnumerable<KeyValuePair<string, ICommand>> commands)
        {
            if (externalAggregatorType != null)
            {
                ICommandAggregator aggregator = Activator.CreateInstance(externalAggregatorType) as ICommandAggregator;
                if (aggregator == null)
                {
                    throw new InvalidCastException("Registered aggregator type could not handled as a valid command aggregator");
                }

                foreach (var cmd in commands)
                {
                    aggregator.AddOrSetCommand(cmd.Key, cmd.Value);
                }

                return aggregator;
            }

            return new CommandAggregator(commands);
        }
    }
}