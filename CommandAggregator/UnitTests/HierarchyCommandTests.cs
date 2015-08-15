using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using WPFCommandAggregator;

namespace UnitTests
{
    /// <summary>
    /// Test class for HierarchyCommand.
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
    ///   <description>Apr/12/2015</description>
    ///   </item>
    ///   <item>
    ///   <term><b>Remarks:</b></term>
    ///   <description>Initial version.</description>
    ///   </item>
    ///   </list>
    /// </remarks>
    [TestClass]
    public class HierarchyCommandTests
    {
        [TestMethod]
        public void AddChildTest()
        {
            StringBuilder strBld = new StringBuilder(1000);

            HierarchyCommand hierCmd = new HierarchyCommand(
                new Action<object>(p1 => strBld.Append("Master")),
                new Predicate<object>(p2 => true),
                HierarchyExecuteStrategy.AllChildsOnly,
                HierarchyCanExecuteStrategy.DependsOnAllChilds);

            var child1 = new RelayCommand(new Action<object>(p1 => strBld.Append("1")), new Predicate<object>(p2 => true));
            var child2 = new RelayCommand(new Action<object>(p1 => strBld.Append("2")), new Predicate<object>(p2 => true));
            var child3 = new RelayCommand(new Action<object>(p1 => strBld.Append("3")), new Predicate<object>(p2 => true));

            hierCmd.AddChildCommand(child1);
            Assert.AreEqual(1, hierCmd.ChildCommands.Count());

            hierCmd.AddChildsCommand(new List<ICommand> { child2, child3, null });
            Assert.AreEqual(3, hierCmd.ChildCommands.Count());

            hierCmd.AddChildCommand(null);
            Assert.AreEqual(3, hierCmd.ChildCommands.Count());
        }

        [TestMethod]
        public void ChildCommandsTest()
        {
            StringBuilder strBld = new StringBuilder(1000);

            HierarchyCommand hierCmd = new HierarchyCommand(
                new Action<object>(p1 => strBld.Append("Master")),
                new Predicate<object>(p2 => true),
                HierarchyExecuteStrategy.AllChildsOnly,
                HierarchyCanExecuteStrategy.DependsOnAllChilds);

            Assert.IsNotNull(hierCmd.ChildCommands);
            
            hierCmd.ChildCommands = null;
            Assert.IsNotNull(hierCmd.ChildCommands);
            Assert.IsFalse(hierCmd.ChildCommands.Any());

            hierCmd.Execute(null);
            Assert.IsTrue(string.IsNullOrEmpty(strBld.ToString()));
        }

        [TestMethod]
        public void ClearChildsTest()
        {
            StringBuilder strBld = new StringBuilder(1000);

            HierarchyCommand hierCmd = new HierarchyCommand(
                new Action<object>(p1 => strBld.Append("Master")),
                new Predicate<object>(p2 => true),
                HierarchyExecuteStrategy.AllChildsOnly,
                HierarchyCanExecuteStrategy.DependsOnAllChilds);

            var child1 = new RelayCommand(new Action<object>(p1 => strBld.Append("1")), new Predicate<object>(p2 => true));
            var child2 = new RelayCommand(new Action<object>(p1 => strBld.Append("2")), new Predicate<object>(p2 => true));
            var child3 = new RelayCommand(new Action<object>(p1 => strBld.Append("3")), new Predicate<object>(p2 => true));
            
            hierCmd.AddChildsCommand(new List<ICommand> { child1, child2, child3, null });
            Assert.AreEqual(3, hierCmd.ChildCommands.Count());

            hierCmd.ClearCommands();
            Assert.IsNotNull(hierCmd.ChildCommands);
            Assert.AreEqual(0, hierCmd.ChildCommands.Count());
        }       

        [TestMethod]
        public void ExecutionTestAllChildOnly()
        {
            StringBuilder strBld = new StringBuilder(1000);

            HierarchyCommand hierCmd = new HierarchyCommand(
                new Action<object>(p1 => strBld.Append("Master")),
                new Predicate<object>(p2 => true),
                HierarchyExecuteStrategy.AllChildsOnly,
                HierarchyCanExecuteStrategy.DependsOnAllChilds);

            var child1 = new RelayCommand(new Action<object>(p1 => strBld.Append("1")), new Predicate<object>(p2 => true));
            var child2 = new RelayCommand(new Action<object>(p1 => strBld.Append("2")), new Predicate<object>(p2 => true));
            var child3 = new RelayCommand(new Action<object>(p1 => strBld.Append("3")), new Predicate<object>(p2 => true));
            
            hierCmd.AddChildsCommand(new List<ICommand> { child1, child2, child3 });
            Assert.AreEqual(3, hierCmd.ChildCommands.Count());

            hierCmd.Execute(null);
            Assert.IsTrue(strBld.ToString().Contains("1"));
            Assert.IsTrue(strBld.ToString().Contains("2"));
            Assert.IsTrue(strBld.ToString().Contains("3"));
            Assert.IsFalse(strBld.ToString().Contains("Master"));
        }    
    
        [TestMethod]
        public void ExecutionTestMasterOnly()
        {
            StringBuilder strBld = new StringBuilder(1000);

            HierarchyCommand hierCmd = new HierarchyCommand(
                new Action<object>(p1 => strBld.Append("Master")),
                new Predicate<object>(p2 => true),
                HierarchyExecuteStrategy.MasterCommandOnly,
                HierarchyCanExecuteStrategy.DependsOnAllChilds);

            var child1 = new RelayCommand(new Action<object>(p1 => strBld.Append("1")), new Predicate<object>(p2 => true));
            var child2 = new RelayCommand(new Action<object>(p1 => strBld.Append("2")), new Predicate<object>(p2 => true));
            var child3 = new RelayCommand(new Action<object>(p1 => strBld.Append("3")), new Predicate<object>(p2 => true));
            
            hierCmd.AddChildsCommand(new List<ICommand> { child1, child2, child3 });
            Assert.AreEqual(3, hierCmd.ChildCommands.Count());

            hierCmd.Execute(null);
            Assert.IsFalse(strBld.ToString().Contains("1"));
            Assert.IsFalse(strBld.ToString().Contains("2"));
            Assert.IsFalse(strBld.ToString().Contains("3"));
            Assert.IsTrue(strBld.ToString().Contains("Master"));
        }   
 
        [TestMethod]
        public void ExecutionTestMasterAndAllChilds()
        {
            StringBuilder strBld = new StringBuilder(1000);
            var child1 = new RelayCommand(new Action<object>(p1 => strBld.Append("1")), new Predicate<object>(p2 => true));
            var child2 = new RelayCommand(new Action<object>(p1 => strBld.Append("2")), new Predicate<object>(p2 => true));
            var child3 = new RelayCommand(new Action<object>(p1 => strBld.Append("3")), new Predicate<object>(p2 => true));

            HierarchyCommand hierCmd = new HierarchyCommand(
                new List<ICommand> { child1, child2, child3 },
                new Action<object>(p1 => strBld.Append("Master")),
                new Predicate<object>(p2 => true),
                HierarchyExecuteStrategy.MasterAndAllChilds,
                HierarchyCanExecuteStrategy.DependsOnAllChilds);

            Assert.AreEqual(3, hierCmd.ChildCommands.Count());

            hierCmd.Execute(null);
            Assert.IsTrue(strBld.ToString().Contains("1"));
            Assert.IsTrue(strBld.ToString().Contains("2"));
            Assert.IsTrue(strBld.ToString().Contains("3"));
            Assert.IsTrue(strBld.ToString().Contains("Master"));
        } 
  
        [TestMethod]
        public void CanExecutionTestAllChilds()
        {
            StringBuilder strBld = new StringBuilder(1000);
            var child1 = new RelayCommand(new Action<object>(p1 => strBld.Append("1")), new Predicate<object>(p2 => true));
            var child2 = new RelayCommand(new Action<object>(p1 => strBld.Append("2")), new Predicate<object>(p2 => true));
            var child3 = new RelayCommand(new Action<object>(p1 => strBld.Append("3")), new Predicate<object>(p2 => true));

            HierarchyCommand hierCmd = new HierarchyCommand(
                new List<ICommand> { child1, child2, child3 },
                new Action<object>(p1 => strBld.Append("Master")),
                new Predicate<object>(p2 => true),
                HierarchyExecuteStrategy.MasterAndAllChilds,
                HierarchyCanExecuteStrategy.DependsOnAllChilds);

            Assert.AreEqual(3, hierCmd.ChildCommands.Count());
            Assert.IsTrue(hierCmd.CanExecute(null));

            hierCmd.ClearCommands();
            var child1b = new RelayCommand(new Action<object>(p1 => strBld.Append("1")), new Predicate<object>(p2 => true));
            var child2b = new RelayCommand(new Action<object>(p1 => strBld.Append("2")), new Predicate<object>(p2 => false));
            var child3b = new RelayCommand(new Action<object>(p1 => strBld.Append("3")), new Predicate<object>(p2 => true));
            hierCmd.AddChildsCommand(new List<ICommand> { child1b, child2b, child3b });
            Assert.AreEqual(3, hierCmd.ChildCommands.Count());
            Assert.IsFalse(hierCmd.CanExecute(null));
        }

        [TestMethod]
        public void CanExecutionTestAtLeastOneChild()
        {
            StringBuilder strBld = new StringBuilder(1000);
            var child1 = new RelayCommand(new Action<object>(p1 => strBld.Append("1")), new Predicate<object>(p2 => true));
            var child2 = new RelayCommand(new Action<object>(p1 => strBld.Append("2")), new Predicate<object>(p2 => false));
            var child3 = new RelayCommand(new Action<object>(p1 => strBld.Append("3")), new Predicate<object>(p2 => false));

            HierarchyCommand hierCmd = new HierarchyCommand(
                new List<ICommand> { child1, child2, child3 },
                new Action<object>(p1 => strBld.Append("Master")),
                new Predicate<object>(p2 => true),
                HierarchyExecuteStrategy.MasterAndAllChilds,
                HierarchyCanExecuteStrategy.DependsOnAtLeastOneChild);

            Assert.AreEqual(3, hierCmd.ChildCommands.Count());
            Assert.IsTrue(hierCmd.CanExecute(null));

            hierCmd.ClearCommands();
            var child1b = new RelayCommand(new Action<object>(p1 => strBld.Append("1")), new Predicate<object>(p2 => false));
            var child2b = new RelayCommand(new Action<object>(p1 => strBld.Append("2")), new Predicate<object>(p2 => false));
            var child3b = new RelayCommand(new Action<object>(p1 => strBld.Append("3")), new Predicate<object>(p2 => false));
            hierCmd.AddChildsCommand(new List<ICommand> { child1b, child2b, child3b });
            Assert.AreEqual(3, hierCmd.ChildCommands.Count());
            Assert.IsFalse(hierCmd.CanExecute(null));
        }

        [TestMethod]
        public void CanExecutionTestMasterOnly()
        {
            StringBuilder strBld = new StringBuilder(1000);
            var child1 = new RelayCommand(new Action<object>(p1 => strBld.Append("1")), new Predicate<object>(p2 => true));
            var child2 = new RelayCommand(new Action<object>(p1 => strBld.Append("2")), new Predicate<object>(p2 => false));
            var child3 = new RelayCommand(new Action<object>(p1 => strBld.Append("3")), new Predicate<object>(p2 => false));

            HierarchyCommand hierCmd = new HierarchyCommand(
                new List<ICommand> { child1, child2, child3 },
                new Action<object>(p1 => strBld.Append("Master")),
                new Predicate<object>(p2 => true),
                HierarchyExecuteStrategy.MasterAndAllChilds,
                HierarchyCanExecuteStrategy.DependsOnMasterCommandOnly);

            Assert.AreEqual(3, hierCmd.ChildCommands.Count());
            Assert.IsTrue(hierCmd.CanExecute(null));

            hierCmd.ClearCommands();
            var child1b = new RelayCommand(new Action<object>(p1 => strBld.Append("1")), new Predicate<object>(p2 => false));
            var child2b = new RelayCommand(new Action<object>(p1 => strBld.Append("2")), new Predicate<object>(p2 => false));
            var child3b = new RelayCommand(new Action<object>(p1 => strBld.Append("3")), new Predicate<object>(p2 => false));
            hierCmd.AddChildsCommand(new List<ICommand> { child1b, child2b, child3b });
            Assert.AreEqual(3, hierCmd.ChildCommands.Count());
            Assert.IsTrue(hierCmd.CanExecute(null));

            hierCmd.ClearCommands();
            var child1c = new RelayCommand(new Action<object>(p1 => strBld.Append("1")), new Predicate<object>(p2 => true));
            var child2c = new RelayCommand(new Action<object>(p1 => strBld.Append("2")), new Predicate<object>(p2 => true));
            var child3c = new RelayCommand(new Action<object>(p1 => strBld.Append("3")), new Predicate<object>(p2 => true));

            HierarchyCommand hierCmdC = new HierarchyCommand(
                new List<ICommand> { child1c, child2c, child3c },
                new Action<object>(p1 => strBld.Append("Master")),
                new Predicate<object>(p2 => false),
                HierarchyExecuteStrategy.MasterAndAllChilds,
                HierarchyCanExecuteStrategy.DependsOnMasterCommandOnly);

            Assert.AreEqual(3, hierCmdC.ChildCommands.Count());
            Assert.IsFalse(hierCmdC.CanExecute(null));
        }
    }
}
