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
            private string defaultTestProperty;
            public string DefaultTestProperty
            {
                get => this.defaultTestProperty;
                set => this.SetPropertyValue(ref this.defaultTestProperty, value);
            }

            private string preAndPostActionTestProperty;
            public string PreAndPostActionTestProperty
            {
                get => this.preAndPostActionTestProperty;
                set => this.SetPropertyValue(
                    ref this.preAndPostActionTestProperty,                     
                    value,
                    () => this.PreSetResult = "before",
                    () => this.PostSetResult = "after");
            }

            internal string PreSetResult { get; private set; }
            internal string PostSetResult { get; private set; }

            protected override void InitCommands()
            {
                this.CmdAgg.AddOrSetCommand("TestCommand", new RelayCommand(new Action<object>(p1 => { })));
            }
        }

        [TestMethod]
        public void AddAndExistsTest()
        {
            BaseTestViewModel testVm = new BaseTestViewModel();

            testVm.DefaultTestProperty = "Dummy";
            Assert.AreEqual("Dummy", testVm.DefaultTestProperty);

            Assert.IsTrue(testVm.CmdAgg.Exists("TestCommand"));
        }

        [TestMethod]
        public void PreAndPostSetActionTest()
        {
            BaseTestViewModel testVm = new BaseTestViewModel();

            testVm.PreAndPostActionTestProperty = "Something";
            Assert.AreEqual("before", testVm.PreSetResult);
            Assert.AreEqual("after", testVm.PostSetResult);            
        }        
    }
}
