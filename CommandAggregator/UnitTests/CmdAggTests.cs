using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFCommandAggregator;

namespace UnitTests
{
    /// <summary>
    /// Test class for WPF Command Aggregator.
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
    ///   <description>Dec/24/2014</description>
    ///   </item>
    ///   <item>
    ///   <term><b>Remarks:</b></term>
    ///   <description>Initial version.</description>
    ///   </item>
    ///   </list>
    /// </remarks>
    [TestClass]
    public class CmdAggTests
    {      
        [TestMethod]
        public void AddAndExistsTest()
        {
            CommandAggregator cmdAgg = new CommandAggregator();
           
            cmdAgg.AddOrSetCommand("TestCommand1", new RelayCommand(p1 => { }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand2", new RelayCommand(p1 => { }, p2 => true));

            Action<object> act = new Action<object>(p0 => { });
            Predicate<object> pred = new Predicate<object>(p1 => { return true; });
            cmdAgg.AddOrSetCommand("TestCommand3", act , pred);

            Assert.IsTrue(cmdAgg.Exists("TestCommand1"));
            Assert.IsTrue(cmdAgg.Exists("TestCommand2"));
            Assert.IsTrue(cmdAgg.Exists("TestCommand3"));
            Assert.IsFalse(cmdAgg.Exists("TestCommand4"));
        }

        [TestMethod]
        public void HasNullCommandTest()
        {
            CommandAggregator cmdAgg = new CommandAggregator();

            cmdAgg.AddOrSetCommand("TestCommand1", null);
            cmdAgg.AddOrSetCommand("TestCommand2", new RelayCommand(p1 => { }, p2 => true));

            Assert.IsTrue(cmdAgg.HasNullCommand("TestCommand1"));
            Assert.IsFalse(cmdAgg.HasNullCommand("TestCommand2"));
        }        

        [TestMethod]
        public void CountTest()
        {
            CommandAggregator cmdAgg = new CommandAggregator();

            Assert.AreEqual(0, cmdAgg.Count());
            cmdAgg.AddOrSetCommand("TestCommand1", new RelayCommand(p1 => { }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand2", new RelayCommand(p1 => { }, p2 => true));

            Assert.AreEqual(2, cmdAgg.Count());
        }

        [TestMethod]
        public void RemoveTest()
        {
            CommandAggregator cmdAgg = new CommandAggregator();
            cmdAgg.AddOrSetCommand("TestCommand1", new RelayCommand(p1 => { }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand2", new RelayCommand(p1 => { }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand3", new RelayCommand(p1 => { }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand4", new RelayCommand(p1 => { }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand5", new RelayCommand(p1 => { }, p2 => true));
            Assert.IsTrue(cmdAgg.Count() == 5);

            cmdAgg.Remove("TestCommand3");
            Assert.IsTrue(cmdAgg.Count() == 4);
            Assert.IsTrue(cmdAgg.Exists("TestCommand1"));
            Assert.IsTrue(cmdAgg.Exists("TestCommand2"));
            Assert.IsFalse(cmdAgg.Exists("TestCommand3"));
            Assert.IsTrue(cmdAgg.Exists("TestCommand4"));
            Assert.IsTrue(cmdAgg.Exists("TestCommand5"));
            
            
            cmdAgg.RemoveAll();
            Assert.IsTrue(cmdAgg.Count() == 0);
            Assert.IsFalse(cmdAgg.Exists("TestCommand1"));
            Assert.IsFalse(cmdAgg.Exists("TestCommand2"));
            Assert.IsFalse(cmdAgg.Exists("TestCommand3"));
            Assert.IsFalse(cmdAgg.Exists("TestCommand4"));
            Assert.IsFalse(cmdAgg.Exists("TestCommand5"));
        }

        [TestMethod]
        public void GetCommandAndIndexerAndExecuteTest()
        {
            CommandAggregator cmdAgg = new CommandAggregator();
            StringBuilder strBld = new StringBuilder(1000);

            cmdAgg.AddOrSetCommand("TestCommand1", new RelayCommand(p1 => { strBld.Append("1"); }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand2", new RelayCommand(p1 => { strBld.Append("2"); }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand3", new RelayCommand(p1 => { strBld.Append("3"); }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand4", new RelayCommand(p1 => { strBld.Append("4"); }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand5", new RelayCommand(p1 => { strBld.Append("5"); }, p2 => true));

            ICommand cmd5 = cmdAgg.GetCommand("TestCommand5");
            cmd5.Execute(null);
            Assert.AreEqual("5", strBld.ToString());

            strBld.Clear();
            ICommand cmd2 = cmdAgg.GetCommand("TestCommand2");
            cmd2.Execute(null);
            Assert.AreEqual("2", strBld.ToString());

            strBld.Clear();
            ICommand cmd1 = cmdAgg["TestCommand1"];
            cmd1.Execute(null);
            Assert.AreEqual("1", strBld.ToString());
        }

        [TestMethod]
        public void ExecuteAsyncTest()
        {
            CommandAggregator cmdAgg = new CommandAggregator();
            StringBuilder strBld = new StringBuilder(1000);

            cmdAgg.AddOrSetCommand("TestCommand1", new RelayCommand(p1 => { strBld.Append("1"); }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand2", new RelayCommand(p1 => { strBld.Append("2"); }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand3", new RelayCommand(p1 => { strBld.Append("3"); }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand4", new RelayCommand(p1 => { strBld.Append("4"); }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand5", new RelayCommand(p1 => { strBld.Append("5"); }, p2 => true));

            Task t1 = cmdAgg.ExecuteAsync("TestCommand1");
            Task t2 = cmdAgg.ExecuteAsync("TestCommand2");
            Task t3 = cmdAgg.ExecuteAsync("TestCommand3");
            Task t4 = cmdAgg.ExecuteAsync("TestCommand4");
            Task t5 = cmdAgg.ExecuteAsync("TestCommand5");

            Task.WaitAll(t1, t2, t3, t4, t5);

            Assert.IsTrue(strBld.ToString().Contains("1"));
            Assert.IsTrue(strBld.ToString().Contains("2"));
            Assert.IsTrue(strBld.ToString().Contains("3"));
            Assert.IsTrue(strBld.ToString().Contains("4"));
            Assert.IsTrue(strBld.ToString().Contains("5"));
        }

        [TestMethod]
        public void CanExecuteTest()
        {
            CommandAggregator cmdAgg = new CommandAggregator();
            StringBuilder strBld = new StringBuilder(1000);

            cmdAgg.AddOrSetCommand("TestCommand1", new RelayCommand(p1 => { strBld.Append("1"); }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand2", new RelayCommand(p1 => { strBld.Append("2"); }, p2 => false));
            cmdAgg.AddOrSetCommand("TestCommand3", new RelayCommand(p1 => { strBld.Append("3"); }, p2 => true));
            cmdAgg.AddOrSetCommand("TestCommand4", new RelayCommand(p1 => { strBld.Append("4"); }, p2 => false));
            cmdAgg.AddOrSetCommand("TestCommand5", new RelayCommand(p1 => { strBld.Append("5"); }, p2 => true));

            ICommand cmd1 = cmdAgg["TestCommand1"];
            Assert.IsTrue(cmd1.CanExecute(null));

            ICommand cmd2 = cmdAgg["TestCommand2"];
            Assert.IsFalse(cmd2.CanExecute(null));

            ICommand cmd3 = cmdAgg["TestCommand3"];
            Assert.IsTrue(cmd3.CanExecute(null));

            ICommand cmd4 = cmdAgg["TestCommand4"];
            Assert.IsFalse(cmd4.CanExecute(null));
        
            ICommand cmd5 = cmdAgg["TestCommand5"];
            Assert.IsTrue(cmd5.CanExecute(null));
        }
    }
}