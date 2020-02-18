namespace UnitTests.AttachedProperties
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Windows.Controls;
    using WPFCommandAggregator.AttachedProperties;

    /// <summary>
    /// Tests for ObservabelCollectionExt class.
    /// </summary>
    [TestClass]
    public class FocusedTests
    {
        [TestMethod]
        [STAThread]
        public void SetFocusTest()
        {
            TextBox target = new TextBox();
            target.SetValue(Focused.FocusedProperty, true);

            Assert.IsTrue(target.IsFocused);
        }

        [TestMethod]
        [STAThread]
        public void RemoveFocusTest()
        {
            TextBox target = new TextBox();            
            target.SetValue(Focused.FocusedProperty, false);

            Assert.IsFalse(target.IsFocused);
        }
    }
}