using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFCommandAggregator;

namespace UnitTests
{
    /// <summary>
    /// Tests for register/unregister a cusatom aggregator implementation.
    /// </summary>   
    /// <remarks>
    ///   <para><b>History</b></para>
    ///   <list type="table">
    ///   <item>
    ///   <term><b>Author:</b></term>
    ///   <description>Test methods for the usage of an external command aggregator.</description>
    ///   </item>
    ///   <item>
    ///   <term><b>Date:</b></term>
    ///   <description>Feb/05/2019</description>
    ///   </item>
    ///   <item>
    ///   <term><b>Remarks:</b></term>
    ///   <description>Initial version.</description>
    ///   </item>
    ///   </list>
    /// </remarks>
    [TestClass]
    public class ExternalAggregatorTest
    {
        [TestInitialize]
        public void Register()
        {
            CommandAggregatorFactory.RegisterAggreagtorImplementation<OwnAggregator>();
        }

        [TestCleanup]
        public void Unregister()
        {
            CommandAggregatorFactory.UnregisterAggreagtorImplementation<OwnAggregator>();
        }

        [TestMethod]
        public void TypeTest()
        {
            var aggregator = CommandAggregatorFactory.GetNewCommandAggregator();

            Assert.IsNotInstanceOfType(aggregator, typeof(CommandAggregator));
            Assert.IsInstanceOfType(aggregator, typeof(OwnAggregator));
        }
    }

    /// <summary>
    /// Test aggregator.
    /// </summary>
    internal class OwnAggregator : ICommandAggregator
    {
        public ICommand this[string key] => throw new NotImplementedException();

        public void AddOrSetCommand(string key, ICommand command)
        {
            throw new NotImplementedException();
        }

        public void AddOrSetCommand(string key, Action<object> executeDelegate, Predicate<object> canExecuteDelegate)
        {
            throw new NotImplementedException();
        }

        public void AddOrSetCommand(string key, Action<object> executeDelegate)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public Task ExecuteAsync(string key, object parameter = null)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string key)
        {
            throw new NotImplementedException();
        }

        public ICommand GetCommand(string key)
        {
            throw new NotImplementedException();
        }

        public bool HasNullCommand(string key)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public void RemoveAll()
        {
            throw new NotImplementedException();
        }
    }
}
