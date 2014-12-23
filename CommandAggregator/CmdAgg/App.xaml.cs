using CommandAggregatorExample.ViewModels;
using System.Windows;

namespace CommandAggregatorExample
{
    /// <summary>
    /// The Application Entry Point.
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
    public partial class App : Application
    {
        /// <summary>
        /// Definition of the starting view incl. the view model.
        /// </summary>
        /// <param name="e">The startup event arguments.</param>       
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow window = new MainWindow { DataContext = new MainVm() };
            window.Show();
        }
    }
}
