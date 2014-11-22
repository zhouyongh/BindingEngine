using BindingEngine.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Illusion.Utility.Tests
{
    using System;
    using System.Windows.Media;

    [TestClass]
    public class WeakCommandBindingTests
    {
        [TestMethod]
        public void Test_WeakCommandBinding_Basic()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            new WeakCommandBinding(view, null, viewModel, "AddAgeCommand")
                .Initialize<WeakCommandBinding>()
                .AttachTargetEvent("TestViewEvent");

            Assert.AreEqual(0, viewModel.Age);

            view.RaiseTestViewEvent();

            Assert.AreEqual(1, viewModel.Age);
        }

        [TestMethod]
        public void Test_WeakCommandBinding_CanExecute()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            new WeakCommandBinding(view, null, viewModel, "AddAgeCommand")
                .Initialize<WeakCommandBinding>()
                .AttachTargetEvent("TestViewEvent");

            Assert.AreEqual(0, viewModel.Age);

            view.RaiseTestViewEvent();
            Assert.AreEqual(1, viewModel.Age);

            view.RaiseTestViewEvent();
            Assert.AreEqual(2, viewModel.Age);

            view.RaiseTestViewEvent();
            Assert.AreEqual(2, viewModel.Age);
        }

        [TestMethod]
        public void Test_WeakCommandBinding_EnableProperty()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            Assert.IsFalse(view.Enable);

            new WeakCommandBinding(view, null, viewModel, "AddAgeCommand")
                .Initialize<WeakCommandBinding>()
                .AttachTargetEvent("TestViewEvent")
                .OfType<WeakCommandBinding>()
                .AddEnableProperty("Enable");

            Assert.IsTrue(view.Enable);

            Assert.AreEqual(0, viewModel.Age);

            view.RaiseTestViewEvent();
            Assert.AreEqual(1, viewModel.Age);

            Assert.IsTrue(view.Enable);

            view.RaiseTestViewEvent();
            Assert.AreEqual(2, viewModel.Age);

            Assert.IsFalse(view.Enable);

            view.RaiseTestViewEvent();

            Assert.IsFalse(view.Enable);
            Assert.AreEqual(2, viewModel.Age);
        }

        [TestMethod]
        public void Test_WeakCommandBinding_EnableProperty_Expression()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            Assert.IsFalse(view.Enable);

            new WeakCommandBinding(view, null, viewModel, "AddAgeCommand")
                .Initialize<WeakCommandBinding>()
                .AttachTargetEvent("TestViewEvent")
                .OfType<WeakCommandBinding>()
                .AddEnableProperty<TestView>(i => i.Enable);

            Assert.IsTrue(view.Enable);

            Assert.AreEqual(0, viewModel.Age);

            view.RaiseTestViewEvent();
            Assert.AreEqual(1, viewModel.Age);

            Assert.IsTrue(view.Enable);

            view.RaiseTestViewEvent();
            Assert.AreEqual(2, viewModel.Age);

            Assert.IsFalse(view.Enable);

            view.RaiseTestViewEvent();

            Assert.IsFalse(view.Enable);
            Assert.AreEqual(2, viewModel.Age);
        }

        [TestMethod]
        public void Test_WeakCommandBinding_RemoveEnableProperty()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            Assert.IsFalse(view.Enable);

            WeakCommandBinding weakCommandBinding = new WeakCommandBinding(view, null, viewModel, "AddAgeCommand")
                .Initialize<WeakCommandBinding>()
                .AttachTargetEvent("TestViewEvent")
                .OfType<WeakCommandBinding>()
                .AddEnableProperty("Enable");

            Assert.IsTrue(view.Enable);

            Assert.AreEqual(0, viewModel.Age);

            view.RaiseTestViewEvent();
            Assert.AreEqual(1, viewModel.Age);

            Assert.IsTrue(view.Enable);

            weakCommandBinding.RemoveEnableProperty("Enable");

            view.RaiseTestViewEvent();
            Assert.IsTrue(view.Enable);
        }

        [TestMethod]
        public void Test_WeakCommandBinding_RemoveEnableProperty_Expression()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            Assert.IsFalse(view.Enable);

            WeakCommandBinding weakCommandBinding = new WeakCommandBinding(view, null, viewModel, "AddAgeCommand")
                .Initialize<WeakCommandBinding>()
                .AttachTargetEvent("TestViewEvent")
                .OfType<WeakCommandBinding>()
                .AddEnableProperty<TestView>(i => i.Enable);

            Assert.IsTrue(view.Enable);

            Assert.AreEqual(0, viewModel.Age);

            view.RaiseTestViewEvent();
            Assert.AreEqual(1, viewModel.Age);

            Assert.IsTrue(view.Enable);

            weakCommandBinding.RemoveEnableProperty<TestView>(i => i.Enable);

            view.RaiseTestViewEvent();
            Assert.IsTrue(view.Enable);
        }

        [TestMethod]
        public void Test_WeakCommandBinding_Watch()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            Assert.IsFalse(view.Enable);

            new WeakCommandBinding(view, null, viewModel, "AddAgeCommand")
                .Initialize<WeakCommandBinding>()
                .AttachTargetEvent("TestViewEvent")
                .OfType<WeakCommandBinding>()
                .AddEnableProperty("Enable")
                .Watch("Age");

            Assert.IsTrue(view.Enable);

            Assert.AreEqual(0, viewModel.Age);

            view.RaiseTestViewEvent();
            Assert.AreEqual(1, viewModel.Age);

            Assert.IsTrue(view.Enable);

            viewModel.Age++;

            Assert.IsFalse(view.Enable);
        }

        [TestMethod]
        public void Test_WeakCommandBinding_Watch_Expression()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            Assert.IsFalse(view.Enable);

            new WeakCommandBinding(view, null, viewModel, "AddAgeCommand")
                .Initialize<WeakCommandBinding>()
                .AttachTargetEvent("TestViewEvent")
                .OfType<WeakCommandBinding>()
                .AddEnableProperty("Enable")
                .Watch<TestViewModel>(i => i.Age);

            Assert.IsTrue(view.Enable);

            Assert.AreEqual(0, viewModel.Age);

            view.RaiseTestViewEvent();
            Assert.AreEqual(1, viewModel.Age);

            Assert.IsTrue(view.Enable);

            viewModel.Age++;

            Assert.IsFalse(view.Enable);
        }

        [TestMethod]
        public void Test_WeakCommandBinding_UnWatch()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            Assert.IsFalse(view.Enable);

            WeakCommandBinding commandBinding = new WeakCommandBinding(view, null, viewModel, "AddAgeCommand")
                .Initialize<WeakCommandBinding>()
                .AttachTargetEvent("TestViewEvent")
                .OfType<WeakCommandBinding>()
                .AddEnableProperty("Enable")
                .Watch(viewModel, "Age");
            commandBinding.Watch(viewModel, "Age");

            Assert.IsTrue(view.Enable);

            Assert.AreEqual(0, viewModel.Age);

            view.RaiseTestViewEvent();
            Assert.AreEqual(1, viewModel.Age);
            Assert.IsTrue(view.Enable);

            Exception exception = null;
            try
            {
                commandBinding.Watch(null, "Age");
            }
            catch (ArgumentNullException e)
            {
                exception = e;
            }
            Assert.IsNotNull(exception);

            viewModel.Age++;
            Assert.IsFalse(view.Enable);

            viewModel.Age--;
            Assert.IsTrue(view.Enable);

            commandBinding.UnWatch("Age");

            viewModel.Age++;
            Assert.IsTrue(view.Enable);

            commandBinding.Watch(viewModel, "Age");
            Assert.IsFalse(view.Enable);

            commandBinding.UnWatch(viewModel, "Age");
            viewModel.Age--;
            Assert.IsFalse(view.Enable);
        }

        [TestMethod]
        public void Test_WeakCommandBinding_UnWatch_Expression()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            Assert.IsFalse(view.Enable);

            WeakCommandBinding commandBinding = new WeakCommandBinding(view, null, viewModel, "AddAgeCommand")
                .Initialize<WeakCommandBinding>()
                .AttachTargetEvent("TestViewEvent")
                .OfType<WeakCommandBinding>()
                .AddEnableProperty("Enable")
                .Watch<TestViewModel>(i => i.Age);

            Assert.IsTrue(view.Enable);

            Assert.AreEqual(0, viewModel.Age);

            view.RaiseTestViewEvent();
            Assert.AreEqual(1, viewModel.Age);

            Assert.IsTrue(view.Enable);

            viewModel.Age++;
            Assert.IsFalse(view.Enable);

            viewModel.Age--;
            Assert.IsTrue(view.Enable);

            commandBinding.UnWatch<TestViewModel>(null);
            commandBinding.UnWatch(null);

            Exception exception = null;
            try
            {
                commandBinding.UnWatch(null, null);
            }
            catch (ArgumentNullException e)
            {
                exception = e;
            }
            Assert.IsNotNull(exception);

            commandBinding.UnWatch<TestViewModel>(o => o.Age);

            viewModel.Age++;
            Assert.IsTrue(view.Enable);
        }

        [TestMethod]
        public void Test_WeakCommandBinding_CanExecuteChangedCallback()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            Assert.IsFalse(view.Enable);
            bool isEnable = false;

            WeakCommandBinding commandBinding = new WeakCommandBinding(view, null, viewModel, "AddAgeCommand")
                .Initialize<WeakCommandBinding>()
                .AttachTargetEvent("TestViewEvent")
                .OfType<WeakCommandBinding>()
                .AddEnableProperty("Enable")
                .Watch("Age")
                .SetCanExecuteChanged((sender, args) => { isEnable = args.CanExecute; });

            Assert.IsTrue(view.Enable);
            Assert.IsTrue(isEnable);

            Assert.AreEqual(0, viewModel.Age);

            view.RaiseTestViewEvent();
            Assert.AreEqual(1, viewModel.Age);

            Assert.IsTrue(view.Enable);
            Assert.IsTrue(isEnable);

            commandBinding.SetCanExecuteChanged(null);

            viewModel.Age++;
            Assert.IsFalse(view.Enable);
            Assert.IsTrue(isEnable);
        }

        [TestMethod]
        public void Test_WeakCommandBinding_Parameter()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            new WeakCommandBinding(view, null, viewModel, "SetAgeCommand")
                .Initialize<WeakCommandBinding>()
                .AttachTargetEvent("TestViewEvent")
                .OfType<WeakCommandBinding>()
                .SetParameter(2);

            Assert.AreEqual(0, viewModel.Age);

            view.RaiseTestViewEvent();

            Assert.AreEqual(2, viewModel.Age);
        }

        [TestMethod]
        public void Test_WeakCommandBinding_Parameter_2()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            new WeakCommandBinding(view, null, viewModel, "SetAgeCommand")
                .Initialize<WeakCommandBinding>()
                .AttachTargetEvent("TestViewEvent")
                .OfType<WeakCommandBinding>()
                .SetParameter(2, null);

            Assert.AreEqual(0, viewModel.Age);

            view.RaiseTestViewEvent();

            Assert.AreEqual(2, viewModel.Age);
        }
    }
}