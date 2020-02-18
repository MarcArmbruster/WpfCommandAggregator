namespace UnitTests.AttachedProperties
{
    using System;
    using System.Windows;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using WPFCommandAggregator.AttachedProperties;

    /// <summary>
    /// Tests for WindowCloser class.
    /// </summary>
    [TestClass]
    public class WindowCloserTests
    {
        [TestMethod]
        [STAThread]
        public void CloseOnTrueTest()
        {
            bool closed = false;
            Window target = new Window();
            target.Closed += (s,e) => closed = true;

            target.SetValue(WindowCloser.WindowResultProperty, true);

            Assert.IsTrue(closed);
        }

        [TestMethod]
        [STAThread]
        public void CloseOnFalseTest()
        {
            bool closed = false;
            Window target = new Window();
            target.Closed += (s, e) => closed = true;

            target.SetValue(WindowCloser.WindowResultProperty, false);

            Assert.IsTrue(closed);
        }

        [TestMethod]
        [STAThread]
        public void NotCloseOnNullTest()
        {
            bool closed = false;
            Window target = new Window();
            target.Closed += (s, e) => closed = true;

            target.SetValue(WindowCloser.WindowResultProperty, null);

            Assert.IsFalse(closed);
        }
    }
}