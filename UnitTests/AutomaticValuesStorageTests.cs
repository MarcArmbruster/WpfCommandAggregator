using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WPFCommandAggregator;

namespace UnitTests
{
    [TestClass]
    public class AutomaticValuesStorageTests
    {
        /// <summary>
        /// Private test class -> testing object.
        /// </summary>
        private class TestViewModel : BaseVm
        {
            protected override void InitCommands()
            {                
            }

            public string PropertyA
            {
                get => this.GetPropertyValue<string>();
                set => this.SetPropertyValue<string>(value);
            }
        }

        [TestMethod]
        public void DefaultValueTest()
        {
            TestViewModel target = new TestViewModel();

            Assert.IsTrue(Equals(default(string), target.PropertyA));
        }

        [TestMethod]
        public void AppliedValueTest()
        {
            TestViewModel target = new TestViewModel();

            target.PropertyA = "Marc";
            Assert.AreEqual("Marc", target.PropertyA);
        }
    }
}