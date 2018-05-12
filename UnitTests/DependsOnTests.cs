using Microsoft.VisualStudio.TestTools.UnitTesting;
using WPFCommandAggregator;

namespace UnitTests
{
    [TestClass]
    public class DependsOnTests
    {
        /// <summary>
        /// Private test class -> testing object.
        /// </summary>
        private class BaseTestViewModel : BaseVm
        {
            private string testProperty1;
            private string testProperty2;            

            public string TestProperty1
            {
                get => this.testProperty1;
                set => this.SetPropertyValue(ref this.testProperty1, value);
            }

            [DependsOn(nameof(TestProperty1))]
            public string TestProperty2
            {
                get => this.testProperty2;
                set => this.SetPropertyValue(ref this.testProperty2, value);
            }

            protected override void InitCommands()
            {                
            }
        }

        [TestMethod]
        public void DependsOnCalledTest()
        {
            BaseTestViewModel target = new BaseTestViewModel();
            string testValue = string.Empty;

            target.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "TestProperty2")
                {
                    testValue = "Dependend Event Fired";
                }
            };

            target.TestProperty1 = "Run Test";

            Assert.AreEqual("Dependend Event Fired", testValue);
        }
    }
}
