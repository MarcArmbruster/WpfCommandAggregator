using System;

namespace WPFCommandAggregator
{
    /// <summary>
    /// DependsOn attribute to manage dependend notifications
    /// in an declarative way.
    /// Used within the BaseVm class notification logic.
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
    ///   <description>May/12/2018</description>
    ///   </item>
    ///   <item>
    ///   <term><b>Remarks:</b></term>
    ///   <description>Initial version.</description>
    ///   </item>
    ///   </list>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DependsOnAttribute : Attribute
    {
        /// <summary>
        /// Gets the names of dependend properties.
        /// </summary>
        public string[] PropertyNames { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="propertyNames">Names of the dependend properties.</param>
        public DependsOnAttribute(params string[] propertyNames)
        {
            this.PropertyNames = propertyNames;
        }
    }
}