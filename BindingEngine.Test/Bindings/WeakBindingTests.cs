using System.Windows.Forms;
using BindingEngine.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Illusion.Utility.Tests
{
    using System;
    using System.Globalization;

    [TestClass]
    public class WeakBindingTests
    {
        private const string Name1 = "Yohan1";
        private const string Name2 = "Yohan2";

        [TestMethod]
        public void Test_WeakBinding_AttachTargetEvent()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            new WeakPropertyBinding(view, "Text1", viewModel, "Name")
                .Initialize<WeakPropertyBinding>()
                .SetMode(BindMode.TwoWay)
                .AttachTargetEvent("TestViewEvent");
            viewModel.Name = Name1;

            Assert.AreEqual(view.Text1, Name1);

            view.Text1 = Name2;
            Assert.AreNotEqual(viewModel.Name, Name2);

            view.RaiseTestViewEvent();
            Assert.AreEqual(viewModel.Name, Name2);
        }

        [TestMethod]
        public void Test_WeakBinding_AttachTargetEvent_2()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();
            var button = new Button();

            new WeakPropertyBinding(view, "Text1", viewModel, "Name")
                .Initialize<WeakPropertyBinding>()
                .SetMode(BindMode.TwoWay)
                .AttachTargetEvent(button, null, "Click");
            viewModel.Name = Name1;

            Assert.AreEqual(view.Text1, Name1);

            view.Text1 = Name2;
            Assert.AreNotEqual(viewModel.Name, Name2);

            button.PerformClick();
            Assert.AreEqual(viewModel.Name, Name2);
        }

        [TestMethod]
        public void Test_WeakBinding_AttachTargetEvent_3()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            new WeakPropertyBinding(view, "Text1", viewModel, "Name")
                .Initialize<WeakPropertyBinding>()
                .SetMode(BindMode.TwoWay)
                .AttachTargetEvent(typeof(TestView), "StaticTestViewEvent");
            viewModel.Name = Name1;

            Assert.AreEqual(view.Text1, Name1);

            view.Text1 = Name2;
            Assert.AreNotEqual(viewModel.Name, Name2);

            TestView.RaiseStaticTestViewEvent();
            Assert.AreEqual(viewModel.Name, Name2);
        }

        [TestMethod]
        public void Test_WeakBinding_AttachSourceEvent()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            new WeakPropertyBinding(view, "Text1", viewModel, "Name")
                .Initialize<WeakPropertyBinding>()
                .AttachSourceEvent("TestViewModelEvent");
            viewModel.Name = Name1;

            Assert.AreNotEqual(view.Text1, Name1);

            viewModel.RaiseTestViewModelEvent();
            Assert.AreEqual(view.Text1, Name1);
        }

        [TestMethod]
        public void Test_WeakBinding_AttachSourceEvent_2()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();
            var button = new Button();

            WeakBinding binding = new WeakPropertyBinding(viewModel, "Name", view, "Text1")
                .Initialize<WeakPropertyBinding>()
                .SetMode(BindMode.OneWay)
                .AttachSourceEvent(button, "Click");

            view.Text1 = Name1;
            Assert.AreNotEqual(viewModel.Name, Name1);

            button.PerformClick();
            Assert.AreEqual(viewModel.Name, Name1);
            binding.Clear();

            viewModel = new TestViewModel();
            view = new TestView();
            button = new Button();

            new WeakPropertyBinding(viewModel, "Name", view, "Text1")
                .Initialize<WeakPropertyBinding>()
                .SetMode(BindMode.OneWay)
                .AttachSourceEvent(button, null, "Click");

            view.Text1 = Name1;
            Assert.AreNotEqual(viewModel.Name, Name1);

            button.PerformClick();
            Assert.AreEqual(viewModel.Name, Name1);
        }

        [TestMethod]
        public void Test_WeakBinding_AttachSourceEvent_3()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            new WeakPropertyBinding(viewModel, "Name", view, "Text1")
                .Initialize<WeakPropertyBinding>()
                .SetMode(BindMode.TwoWay)
                .AttachSourceEvent(typeof(TestView), "StaticTestViewEvent");
            viewModel.Name = Name1;

            Assert.AreEqual(view.Text1, Name1);

            view.Text1 = Name2;
            Assert.AreNotEqual(viewModel.Name, Name2);

            TestView.RaiseStaticTestViewEvent();
            Assert.AreEqual(viewModel.Name, Name2);
        }

        [TestMethod]
        public void Test_WeakBinding_DetachSourceEvent()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            WeakBinding properyBinding = new WeakPropertyBinding(view, "Text1", viewModel, "Name")
                .Initialize<WeakPropertyBinding>()
                .SetMode(BindMode.TwoWay)
                .AttachTargetEvent("TestViewEvent");
            viewModel.Name = Name1;

            Assert.AreEqual(view.Text1, Name1);

            view.Text1 = Name2;
            Assert.AreNotEqual(viewModel.Name, Name2);

            view.RaiseTestViewEvent();
            Assert.AreEqual(viewModel.Name, Name2);

            properyBinding.DetachSourceEvent();
            view.Text1 = Name1;
            Assert.AreNotEqual(viewModel.Name, Name1);
        }

        [TestMethod]
        public void Test_WeakBinding_DetachTargetEvent()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            WeakBinding properyBinding = new WeakPropertyBinding(view, "Text1", viewModel, "Name")
                .Initialize<WeakPropertyBinding>()
                .AttachSourceEvent("TestViewModelEvent");
            viewModel.Name = Name1;

            Assert.AreNotEqual(view.Text1, Name1);

            viewModel.RaiseTestViewModelEvent();
            Assert.AreEqual(view.Text1, Name1);

            properyBinding.DetachTargetEvent();
            viewModel.Name = Name2;
            Assert.AreNotEqual(view.Text1, Name2);
        }

        [TestMethod]
        public void Test_WeakBinding_Clear()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            WeakBinding binding = new WeakPropertyBinding(view, "Text1", viewModel, "Name")
                .Initialize<WeakPropertyBinding>()
                .SetMode(BindMode.TwoWay)
                .AttachTargetEvent("TestViewEvent");
            viewModel.Name = Name1;

            Assert.AreEqual(view.Text1, Name1);

            view.Text1 = Name2;
            Assert.AreNotEqual(viewModel.Name, Name2);

            view.RaiseTestViewEvent();
            Assert.AreEqual(viewModel.Name, Name2);

            binding.Clear();

            view.Text1 = Name1;
            Assert.AreNotEqual(viewModel.Name, Name1);
        }

        [TestMethod]
        public void Test_WeakBinding_Update()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            WeakBinding binding = new WeakPropertyBinding(view, "Text1", viewModel, "Name")
                .Initialize<WeakPropertyBinding>()
                .SetMode(BindMode.TwoWay)
                .AttachTargetEvent("TestViewEvent");
            viewModel.Name = Name1;

            Assert.AreEqual(view.Text1, Name1);

            view.Text1 = Name2;
            Assert.AreNotEqual(viewModel.Name, Name2);

            view.RaiseTestViewEvent();
            Assert.AreEqual(viewModel.Name, Name2);

            binding.Update("Text2", viewModel, "Name");

            viewModel.Name = Name1;

            Assert.AreNotEqual(view.Text1, Name1);
            Assert.AreEqual(view.Text2, Name1);
        }

        [TestMethod]
        public void Test_WeakBinding_DeActive()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            WeakBinding binding = new WeakPropertyBinding(view, "Text1", viewModel, "Name")
                .Initialize<WeakPropertyBinding>()
                .SetMode(BindMode.OneWay)
                .AttachTargetEvent("TestViewEvent");
            viewModel.Name = Name1;

            binding.DeActivate();
            viewModel.Name = Name2;

            Assert.AreNotEqual(view.Text1, Name2);

            binding.Activate();
            Assert.AreEqual(view.Text1, Name2);
        }

        [TestMethod]
        public void Test_WeakBinding_DoConventions()
        {
            var viewModel = new TestViewModel();
            var viewModel2 = new TestViewModel2();

            var binding = new WeakPropertyBinding(viewModel2, "Name", viewModel, "Name")
                .Initialize<WeakPropertyBinding>();
            binding.DeActivate();

            Assert.IsFalse(binding.IsActivate);
            binding.Activate();

            viewModel.Name = Name1;

            binding.DeActivate();
            viewModel.Name = Name2;

            Assert.AreNotEqual(viewModel2.Name, Name2);

            binding.Activate();
            Assert.AreEqual(viewModel2.Name, Name2);
        }

        [TestMethod]
        public void Test_WeakBinding_GetBindSource()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            WeakBinding binding = new WeakPropertyBinding(viewModel, "Name", view, "Text1")
                .Initialize<WeakPropertyBinding>()
                .SetMode(BindMode.TwoWay)
                .AttachSourceEvent(typeof(TestView), "StaticTestViewEvent");
            viewModel.Name = Name1;

            Assert.AreEqual(view.Text1, Name1);

            view.Text1 = Name2;
            Assert.AreNotEqual(viewModel.Name, Name2);

            TestView.RaiseStaticTestViewEvent();
            Assert.AreEqual(viewModel.Name, Name2);

            BindSource source = binding.GetBindSource(viewModel, "Name");
            Assert.AreEqual(binding.BindTarget, source);

            BindSource source2 = binding.GetBindSource(view, "Text1");
            Assert.AreEqual(binding.BindSource, source2);
        }

        [TestMethod]
        public void Test_WeakBinding_MemoryLeak()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            int changed = 0;

            WeakPropertyBinding binding = new WeakPropertyBinding(view, "Text1", viewModel, "Name")
                .Initialize<WeakPropertyBinding>()
                .SetMode(BindMode.OneWay)
                .AttachTargetEvent("TestViewEvent")
                .SetTargetChanged<WeakPropertyBinding>((sender, args) => { changed++; });

            viewModel.Name = Name1;
            Assert.AreEqual(Name1, view.Text1);
            Assert.AreEqual(1, changed);

            viewModel.Name = Name2;
            Assert.AreEqual(Name2, view.Text1);
            Assert.AreEqual(2, changed);


            view = null;
            GC.Collect();
            viewModel.Name = Name1;
            Assert.AreEqual(2, changed);
        }
    }

    public class TestDataConverter : IDataConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            Assert.AreEqual(typeof(string), targetType);
            Assert.AreEqual(CultureInfo.CurrentUICulture, cultureInfo);
            return string.Format("{0}{1}", value, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            Assert.AreEqual(typeof(int), targetType);
            Assert.AreEqual(CultureInfo.CurrentUICulture, cultureInfo);

            if(value.Equals("0") || parameter == null)
            {
                return 0;
            }
            return int.Parse(value.ToString().LastLeft(parameter.ToString()));
        }
    }

    public class TestDataParameterConverter : IDataConverter
    {
        public const string Prefix = "Test ";

        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            int para = parameter != null ? int.Parse(parameter.ToString()) : 0;
            para = (int)value + para;
            return Prefix + para;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            if(value.Equals("0"))
            {
                return 0;
            }

            int para = parameter != null ? int.Parse(parameter.ToString()) : 0;
            return int.Parse(value.ToString().Right(Prefix)) - para;
        }
    }
}