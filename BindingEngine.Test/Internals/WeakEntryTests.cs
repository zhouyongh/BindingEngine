using BindingEngine.Test.Helpers;
using Illusion.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BindingEngine.Test.Internals
{
    [TestClass]
    public class WeakEntryTests
    {
        [TestMethod]
        public void Test_WeakEntry_Equals()
        {
            var viewModel = new TestViewModel();
            var viewModel2 = new TestViewModel();

            var weakEntry = new WeakEntry("Text1", viewModel, "Name");
            var weakEntry1 = new WeakEntry("Text1", viewModel2, "Name");
            var weakEntry2 = new WeakEntry("Text2", viewModel, "Name");
            var weakEntry3 = new WeakEntry("Text1", viewModel, "Age");
            var weakEntry4 = new WeakEntry("Text1", viewModel, "Name");
            var weakEntry5 = new WeakEntry(string.Empty, viewModel, "Name");
            var weakEntry6 = new WeakEntry("Text2", viewModel, string.Empty);

            Assert.IsFalse(weakEntry.Equals(weakEntry1));
            Assert.IsFalse(weakEntry.Equals(weakEntry2));
            Assert.IsFalse(weakEntry.Equals(weakEntry3));
            Assert.IsFalse(weakEntry1.Equals(weakEntry2));
            Assert.IsFalse(weakEntry1.Equals(weakEntry3));
            Assert.IsFalse(weakEntry2.Equals(weakEntry5));
            Assert.IsFalse(weakEntry2.Equals(weakEntry6));
            Assert.IsTrue(weakEntry1 != weakEntry3);

            Assert.IsTrue(weakEntry.Equals(weakEntry4));
            Assert.IsTrue(weakEntry == weakEntry4);
        }
    }
}