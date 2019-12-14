namespace UnitTests.Collections
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Summary description for ObservabelCollectionExtTests
    /// </summary>
    [TestClass]
    public class ObservableCollectionExtTests
    {        
        private class TestClass
        {
            public int Id { get; set; }
            public string Value { get; set; }
        }

        [TestMethod]
        public void ReplaceTest()
        {
            ObservableCollectionExt<TestClass> collection = new ObservableCollectionExt<TestClass>();

            var obj1 = new TestClass { Id = 1, Value = "one" };
            var obj2 = new TestClass { Id = 2, Value = "two" };
            var obj3 = new TestClass { Id = 3, Value = "three" };
            var obj4 = new TestClass { Id = 4, Value = "four" };

            collection.Add(obj1);
            collection.Add(obj2);
            collection.Add(obj3);

            collection.Replace(obj2, obj4);

            Assert.AreEqual(3, collection.Count);
            Assert.AreEqual(1, collection[0].Id);
            Assert.AreEqual(4, collection[1].Id);
            Assert.AreEqual("four", collection[1].Value);
            Assert.AreEqual(3, collection[2].Id);
        }

        [TestMethod]
        public void AddRangeTest()
        {
            ObservableCollectionExt<TestClass> collection = new ObservableCollectionExt<TestClass>();

            var obj1 = new TestClass { Id = 1, Value = "one" };
            var obj2 = new TestClass { Id = 2, Value = "two" };
            var obj3 = new TestClass { Id = 3, Value = "three" };
            var obj4 = new TestClass { Id = 4, Value = "four" };

            int counter = 0;
            collection.CollectionChanged += (s, e) => counter++;

            collection.AddRange(new List<TestClass> { obj1, obj2, obj3, obj4 });

            Assert.AreEqual(1, counter);
            Assert.AreEqual(4, collection.Count);
        }

        [TestMethod]
        public void RemoveRangeTest()
        {
            ObservableCollectionExt<TestClass> collection = new ObservableCollectionExt<TestClass>();

            var obj1 = new TestClass { Id = 1, Value = "one" };
            var obj2 = new TestClass { Id = 2, Value = "two" };
            var obj3 = new TestClass { Id = 3, Value = "three" };
            var obj4 = new TestClass { Id = 4, Value = "four" };
            collection.AddRange(new List<TestClass> { obj1, obj2, obj3, obj4 });

            int counter = 0;
            collection.CollectionChanged += (s, e) => counter++;

            collection.RemoveItems(new List<TestClass> { obj2, obj4 });

            Assert.AreEqual(1, counter);
            Assert.AreEqual(2, collection.Count);
            Assert.AreEqual(1, collection[0].Id);
            Assert.AreEqual(3, collection[1].Id);
        }
    }
}
