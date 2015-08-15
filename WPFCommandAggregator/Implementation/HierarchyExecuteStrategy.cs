namespace WPFCommandAggregator.Implementation
{
    /// <summary>
    /// The strategy the command depend on its childs for Execute.
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
    public enum HierarchyExecuteStrategy
    {
        /// <summary>
        /// Executes only the master command.
        /// </summary>
        MasterCommandOnly = 0,

        /// <summary>
        /// Executes only childs commands.
        /// </summary>
        AllChildsOnly = 1,

        /// <summary>
        /// Executes Master command and all child commands (childs first).
        /// </summary>
        MasterAndAllChilds = 2
    }
}
