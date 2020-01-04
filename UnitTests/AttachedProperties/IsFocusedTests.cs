namespace UnitTests.AttachedProperties
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Windows.Controls;
    using WPFCommandAggregator.AttachedProperties;

    /// <summary>
    /// Tests for ObservabelCollectionExt class.
    /// </summary>
    [TestClass]
    public class IsFocusedTests
    {
        [TestMethod]
        public void SetFocusTest()
        {
            TextBox target = new TextBox();
            target.SetValue(Focused.FocusedProperty, true);

            Assert.IsTrue(target.IsFocused);
        }

        [TestMethod]
        public void RemoveFocusTest()
        {
            TextBox target = new TextBox();            
            target.SetValue(Focused.FocusedProperty, false);

            Assert.IsFalse(target.IsFocused);
        }
    }
}
