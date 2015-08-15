namespace WPFCommandAggregator.Implementation
{
    /// <summary>
    /// The strategy the command depend on its childs for CanExecute.
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
    public enum HierarchyCanExecuteStrategy
    {
        /// <summary>
        /// Master command can be executed by its own CanExecute property value. It does not depend on childs.
        /// </summary>
        DependsOnMasterCommandOnly = 0,

        /// <summary>
        /// Master command can be executed only if all child commands can be executed. CanExecute of Master will not be considered.
        /// </summary>
        DependsOnAllChilds = 1,

        /// <summary>
        /// Master command can be executed only if at least one child commands can be executed. CanExecute of Master will not be considered.
        /// </summary>
        DependsOnAtLeastOneChild = 2
    }
}
