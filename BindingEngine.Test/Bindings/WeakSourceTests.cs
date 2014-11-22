using System;
using BindingEngine.Test.Helpers;
using Illusion.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BindingEngine.Test.Bindings
{
    /// <summary>
    ///     Summary description for WeakSourceTests
    /// </summary>
    [TestClass]
    public class WeakSourceTests
    {
        private const string Name1 = "yohan1";
        private const string Name2 = "yohan2";

        [TestMethod]
        public void Test_WeakSource_SetBinding()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();
            var ws = new WeakTarget(view);

            ws.SetBinding<WeakPropertyBinding>("Text1", viewModel, "Name");

            viewModel.Name = Name1;

            Assert.AreEqual(view.Text1, Name1);

            NotSupportedException exception = null;

            try
            {
                ws.SetBinding<WeakNotifyBinding>("Text1", viewModel, "Name");
            }
            catch(NotSupportedException e)
            {
                exception = e;
            }

            Assert.IsNotNull(exception);
        }

        [TestMethod]
        public void Test_WeakSource_ClearBinding()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();
            var ws = new WeakTarget(view);

            ws.SetBinding<WeakPropertyBinding>("Text1", viewModel, "Name");

            viewModel.Name = Name1;
            Assert.AreEqual(view.Text1, Name1);

            ws.ClearBinding("Text1", viewModel, "Name");

            viewModel.Name = Name2;
            Assert.AreNotEqual(view.Text2, Name2);
        }

        [TestMethod]
        public void Test_WeakSource_ClearAllBindings()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();
            var ws = new WeakTarget(view);

            ws.SetBinding<WeakPropertyBinding>( "Text1", viewModel,"Name");
            ws.SetBinding<WeakPropertyBinding>("ValueInt", viewModel, "Age");

            viewModel.Name = Name1;
            Assert.AreEqual(view.Text1, Name1);
            viewModel.Age = 2;
            Assert.AreEqual(view.ValueInt, 2);

            ws.ClearBindings();
            viewModel.Name = Name2;
            viewModel.Age = 3;


            Assert.AreNotEqual(view.Text1, Name2);
            Assert.AreNotEqual(view.ValueInt, Name2);
        }
    }
}