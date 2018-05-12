using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WPFCommandAggregator;

namespace UnitTests
{
    /// <summary>
    /// Tests for BaseVm class.
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
    ///   <description>Dec/23/2014</description>
    ///   </item>
    ///   <item>
    ///   <term><b>Remarks:</b></term>
    ///   <description>Initial version.</description>
    ///   </item>
    ///   </list>
    /// </remarks>
    [TestClass]
    public class BaseVmTest
    {
        /// <summary>
        /// Private test class -> testing object.
        /// </summary>
        private class BaseTestViewModel : BaseVm
        {
            private string testProperty;
            public string TestProperty
            {
                get => this.testProperty;
                set => this.SetPropertyValue(ref this.testProperty, value);
            }

            protected override void InitCommands()
            {
                this.CmdAgg.AddOrSetCommand("TestCommand", new RelayCommand(new Action<object>(p1 => { })));
            }
        }

        [TestMethod]
        public void AddAndExistsTest()
        {
            BaseTestViewModel testVm = new BaseTestViewModel();

            testVm.TestProperty = "Dummy";
            Assert.AreEqual("Dummy", testVm.TestProperty);

            Assert.IsTrue(testVm.CmdAgg.Exists("TestCommand"));
        }
    }
}
