using BindingEngine.Test.Helpers;
using Illusion.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BindingEngine.Test.Internals
{
    [TestClass]
    public class WeakBindEventTests
    {
        private const string Name = "yohan";
        private const string Name2 = "yohan2";

        [TestMethod]
        public void Test_WeakBindEvent_NormalProperty()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();
            var propertyBinding = new WeakPropertyBinding(view, "Text1", viewModel, "Name")
                .Initialize<WeakPropertyBinding>();

            var weakEvent = new WeakEvent(viewModel);

            Assert.IsFalse(weakEvent.IsAttached);
        }
    }
}