using System;
using System.Collections;
using System.Collections.Generic;
using Illusion.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BindingEngine.Test.Internals
{
    using System.Diagnostics;

    [TestClass]
    public class WeakKeyDictionaryTests
    {
        [TestMethod]
        public void Test_WeakKeyDictionary()
        {
            var normalKeyDiction = new Dictionary<TestKey, int>();
            var key = new TestKey("Yohan");
            var keyReference = new WeakReference(key);

            normalKeyDiction.Add(key, 1);
            Assert.AreEqual(1, normalKeyDiction.Count);
            Assert.AreEqual(1, normalKeyDiction[key]);

            Assert.IsTrue(keyReference.IsAlive);

            key = null;
            GC.Collect();

            //Normal Dictionary hold the strong reference to the key.
            Assert.IsTrue(keyReference.IsAlive);

            var weakKeyDictionary = new WeakKeyDictionary<TestKey, int>();

            key = new TestKey("Yohan");
            keyReference = new WeakReference(key);

            weakKeyDictionary.Add(key, 1);
            Assert.AreEqual(1, weakKeyDictionary.Count);
            Assert.AreEqual(1, weakKeyDictionary[key]);

            IList keys = weakKeyDictionary.Keys;
            Assert.AreEqual(1, keys.Count);

            Assert.IsTrue(keyReference.IsAlive);

            key = null;
            GC.Collect();

            //WeakKeyDictionary hold the WeakReference of the key.
            Assert.IsFalse(keyReference.IsAlive);

            GC.KeepAlive(normalKeyDiction);
        }

        [TestMethod]
        public void Test_WeakKeyDictionary_CRUD()
        {
            var weakKeyDictionary = new WeakKeyDictionary<TestKey, int>();

            var key = new TestKey("Yohan");

            weakKeyDictionary.Add(key, 1);
            Assert.AreEqual(1, weakKeyDictionary.Count);
            Assert.AreEqual(1, weakKeyDictionary[key]);

            weakKeyDictionary.Remove(key);
            Assert.AreEqual(0, weakKeyDictionary.Count);

            Exception exception = null;
            try
            {
                weakKeyDictionary.Remove(null);
            }
            catch (ArgumentNullException e)
            {
                exception = e;
            }
            Assert.IsNotNull(exception);

            exception = null;
            try
            {
                var i = weakKeyDictionary[new TestKey("1")];
            }
            catch (KeyNotFoundException e)
            {
                exception = e;
            }
            Assert.IsNotNull(exception);

            key = new TestKey("Yohan2");
            weakKeyDictionary.Add(key, 1);
            Assert.AreEqual(1, weakKeyDictionary[key]);

            weakKeyDictionary.Clear();
            Assert.AreEqual(0, weakKeyDictionary.Count);

            weakKeyDictionary.Add(key, 1);
            Assert.IsTrue(weakKeyDictionary.ContainsKey(key));
            Assert.IsTrue(weakKeyDictionary.ContainsValue(1));

            weakKeyDictionary[key] = 2;
            Assert.AreEqual(2, weakKeyDictionary[key]);

            bool contains = weakKeyDictionary.ContainsValue(2);
            Assert.IsTrue(contains);

            exception = null;
            try
            {
                weakKeyDictionary[null] = 3;
            }
            catch (ArgumentNullException e)
            {
                exception = e;
            }
            Assert.IsNotNull(exception);

            exception = null;
            try
            {
                weakKeyDictionary.Add(key, 1);
            }
            catch (ArgumentException e)
            {
                exception = e;
            }
            Assert.IsNotNull(exception);

            int value;
            weakKeyDictionary.TryGetValue(key, out value);
            Assert.AreEqual(2, value);

            var count = weakKeyDictionary.Count;
            key = null;
            GC.Collect();

            weakKeyDictionary.Add(new TestKey("yohan9"), 2);

            Assert.AreEqual(count, weakKeyDictionary.Keys.Count);
        }

        private class TestKey
        {
            public TestKey(string name)
            {
                Name = name;
            }

            public string Name { get; set; }

            public override int GetHashCode()
            {
                return (Name != null ? Name.GetHashCode() : 0);
            }

            public override bool Equals(object obj)
            {
                var testKey2 = obj as TestKey;
                if(testKey2 == null)
                {
                    return false;
                }

                return string.Equals(testKey2.Name, Name);
            }

            public static bool operator ==(TestKey key1, TestKey key2)
            {
                if(ReferenceEquals(key1, key2))
                {
                    return true;
                }

                if(ReferenceEquals(key1, null) ||
                   ReferenceEquals(key2, null))
                {
                    return false;
                }

                return key1.Equals(key2);
            }

            public static bool operator !=(TestKey key1, TestKey key2)
            {
                return !(key1 == key2);
            }
        }
    }
}