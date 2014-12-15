using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using BindingEngine.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Illusion.Utility.Tests
{
    [TestClass]
    public class BindingEngineTests
    {
        private const string Name1 = "Yohan1";
        private const string Name2 = "Yohan2";

        [TestMethod]
        public void Test_BindingEngine_PropertyBinding()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            var binding = BindingEngine.SetPropertyBinding(view, "Text1", viewModel, "Name")
                .SetMode(BindMode.TwoWay)
                .AttachTargetEvent("TestViewEvent");

            Assert.AreEqual("Name", binding.SourceProperty);
            Assert.AreEqual("Text1", binding.TargetProperty);

            viewModel.Name = Name1;

            Assert.AreEqual(Name1, view.Text1);

            view.Text1 = Name2;
            Assert.AreNotEqual(Name2, viewModel.Name);

            view.RaiseTestViewEvent();
            Assert.AreEqual(Name2, viewModel.Name);
        }

        [TestMethod]
        public void Test_BindingEngine_GenericBinding()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            BindingEngine.SetBinding<WeakPropertyBinding>(view, "Text1", viewModel, "Name")
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
        public void Test_BindingEngine_GenericBinding_Expression()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            BindingEngine.SetBinding<TestView, TestViewModel, WeakPropertyBinding>(
                view,
                o => o.Text1,
                viewModel,
                i => i.Name).SetMode(BindMode.TwoWay)
                .AttachTargetEvent("TestViewEvent");

            viewModel.Name = Name1;

            Assert.AreEqual(view.Text1, Name1);

            view.Text1 = Name2;
            Assert.AreNotEqual(viewModel.Name, Name2);

            view.RaiseTestViewEvent();
            Assert.AreEqual(viewModel.Name, Name2);
        }

        [TestMethod]
        public void Test_BindingEngine_PropertyBinding_Expression()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            BindingEngine.SetPropertyBinding(view, o => o.Text1, viewModel, i => i.Name)
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
        public void Test_BindingEngine_CommandBinding()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            BindingEngine.SetCommandBinding(view, null, viewModel, "AddAgeCommand")
                .Initialize<WeakCommandBinding>()
                .AttachTargetEvent("TestViewEvent");

            Assert.AreEqual(0, viewModel.Age);

            view.RaiseTestViewEvent();

            Assert.AreEqual(1, viewModel.Age);
        }

        [TestMethod]
        public void Test_BindingEngine_CommandBinding_Expression()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            BindingEngine.SetCommandBinding(view, null, viewModel, i => i.AddAgeCommand)
                .Initialize<WeakCommandBinding>()
                .AttachTargetEvent("TestViewEvent");

            Assert.AreEqual(0, viewModel.Age);

            view.RaiseTestViewEvent();

            Assert.AreEqual(1, viewModel.Age);
        }

        [TestMethod]
        public void Test_BindingEngine_CollectionBinding()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            BindingEngine.SetCollectionBinding(view, "TestViewModelCollections", viewModel, "TestViewModelCollection")
                         .Initialize<WeakCollectionBinding>();

            view.TestViewModelCollections = new List<TestViewModel2>();
            var collections = new ObservableCollection<TestViewModel2>();
            viewModel.TestViewModelCollection = collections;

            var viewModel2 = new TestViewModel2();
            collections.Add(viewModel2);
            collections.Add(new TestViewModel2());

            Assert.AreEqual(2, view.TestViewModelCollections.Count);
            Assert.AreEqual(viewModel2, view.TestViewModelCollections[0]);

            collections.RemoveAt(0);
            Assert.AreEqual(1, view.TestViewModelCollections.Count);

            collections.Clear();
            Assert.AreEqual(0, view.TestViewModelCollections.Count);
        }

        [TestMethod]
        public void Test_BindingEngine_CollectionBinding_Expression()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            BindingEngine.SetCollectionBinding(view, o => o.TestViewModelCollections, viewModel,
                i => i.TestViewModelCollection)
                .Initialize<WeakCollectionBinding>();

            view.TestViewModelCollections = new List<TestViewModel2>();
            var collections = new ObservableCollection<TestViewModel2>();
            viewModel.TestViewModelCollection = collections;

            var viewModel2 = new TestViewModel2();
            collections.Add(viewModel2);
            collections.Add(new TestViewModel2());

            Assert.AreEqual(2, view.TestViewModelCollections.Count);
            Assert.AreEqual(viewModel2, view.TestViewModelCollections[0]);

            collections.RemoveAt(0);
            Assert.AreEqual(1, view.TestViewModelCollections.Count);

            collections.Clear();
            Assert.AreEqual(0, view.TestViewModelCollections.Count);
        }

        [TestMethod]
        public void Test_BindingEngine_NotifyBinding()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();
            int changedCount = 0;

            BindingEngine.SetNotifyBinding(viewModel, o => o.Age, view, null)
                .Initialize<WeakNotifyBinding>()
                .AttachSourceEvent("TestViewEvent")
                .OfType<WeakNotifyBinding>()
                .SetSourceChanged<WeakNotifyBinding>((source, args) => { changedCount++; });

            Assert.AreEqual(0, changedCount);

            view.RaiseTestViewEvent();

            Assert.AreEqual(1, changedCount);

            BindingEngine.SetNotifyBinding(viewModel, o => o.Age, view, null)
                         .Initialize<WeakNotifyBinding>()
                         .AttachSourceEvent("TestViewEvent")
                         .OfType<WeakNotifyBinding>()
                         .SetSourceChanged<WeakNotifyBinding>((source, args) => { changedCount++; });

            view.RaiseTestViewEvent();

            Assert.AreEqual(2, changedCount);
        }

        [TestMethod]
        public void Test_BindingEngine_NotifyBinding_Expression()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();
            int changedCount = 0;

            BindingEngine.SetNotifyBinding(viewModel, "Age", view, null)
                         .Initialize<WeakNotifyBinding>()
                         .AttachSourceEvent("TestViewEvent")
                         .OfType<WeakNotifyBinding>().SetSourceChanged((source, args) => { changedCount++; });

            Assert.AreEqual(0, changedCount);

            view.RaiseTestViewEvent();

            Assert.AreEqual(1, changedCount);
        }

        [TestMethod]
        public void Test_BindingEngine_MethodBinding()
        {
            var viewModel = new TestViewModel5();
            var button = new Button();

            BindingEngine.SetMethodBinding(button, null, viewModel, "TestViewModel")
                .Initialize<WeakMethodBinding>()
                .AttachSourceMethod("AddAge", null)
                .AttachSourceCanInvokeMethod("CanAddAge", null)
                .AttachTargetEvent("Click");

            viewModel.TestViewModel = new TestViewModel();

            Assert.AreEqual(0, viewModel.TestViewModel.Age);

            button.PerformClick();

            Assert.AreEqual(1, viewModel.TestViewModel.Age);
        }

        [TestMethod]
        public void Test_BindingEngine_MethodBinding_Expression()
        {
            var viewModel = new TestViewModel5();
            var button = new Button();

            BindingEngine.SetMethodBinding(button, null, viewModel, o => o.TestViewModel)
                .Initialize<WeakMethodBinding>()
                .AttachSourceMethod("AddAge", null)
                .AttachSourceCanInvokeMethod("CanAddAge", null)
                .AttachTargetEvent("Click");

            viewModel.TestViewModel = new TestViewModel();

            Assert.AreEqual(0, viewModel.TestViewModel.Age);

            button.PerformClick();

            Assert.AreEqual(1, viewModel.TestViewModel.Age);
        }

        [TestMethod]
        public void Test_BindingEngine_ClearBinding()
        {
            var viewModel = new TestViewModel();
            var view = new TestViewModel2();

            BindingEngine.SetPropertyBinding(view, "Name", viewModel, "Name")
                .SetMode(BindMode.TwoWay);

            viewModel.Name = Name1;
            Assert.AreEqual(view.Name, Name1);

            BindingEngine.ClearBinding(view, "Name", viewModel, "Name");

            viewModel.Name = Name2;
            Assert.AreNotEqual(view.Name, Name2);
        }

        [TestMethod]
        public void Test_BindingEngine_ClearBinding_Object()
        {
            var viewModel = new TestViewModel();
            var viewModel2 = new TestViewModel2();

            BindingEngine.SetPropertyBinding(viewModel2, "Name", viewModel, "Name")
                .SetMode(BindMode.TwoWay);

            viewModel.Name = Name1;
            Assert.AreEqual(viewModel2.Name, Name1);

            BindingEngine.SetPropertyBinding(viewModel2, "Age", viewModel, "Age")
                .SetMode(BindMode.TwoWay);

            BindingEngine.ClearBinding(new TestViewModel());

            viewModel.Age = 2;
            Assert.AreEqual(viewModel2.Age, 2);

            BindingEngine.ClearBinding(viewModel2);

            viewModel.Name = Name2;
            Assert.AreNotEqual(viewModel2.Name, Name2);
            viewModel.Age = 3;
            Assert.AreNotEqual(viewModel2.Age, 3);

        }

        [TestMethod]
        public void Test_BindingEngine_ClearBinding_Expression()
        {
            var viewModel = new TestViewModel();
            var view = new TestViewModel2();

            BindingEngine.SetPropertyBinding(view, "Name", viewModel, "Name")
                .SetMode(BindMode.TwoWay);

            viewModel.Name = Name1;
            Assert.AreEqual(view.Name, Name1);

            BindingEngine.ClearBinding(view, i => i.Name, viewModel, o => o.Name);

            viewModel.Name = Name2;
            Assert.AreNotEqual(view.Name, Name2);
        }

        [TestMethod]
        public void Test_BindingEngine_ClearBinding_Object_Property()
        {
            var viewModel = new TestViewModel();
            var viewModel2 = new TestViewModel2();
            var viewModel22 = new TestViewModel2();

            BindingEngine.SetPropertyBinding(viewModel, "Age", viewModel2, "Age");
            BindingEngine.SetPropertyBinding(viewModel, "Age", viewModel22, "Age");

            viewModel2.Age = 2;
            Assert.AreEqual(viewModel.Age, 2);
            viewModel22.Age = 3;
            Assert.AreEqual(viewModel.Age, 3);

            BindingEngine.ClearBinding(viewModel, "Age");

            viewModel2.Age = 4;
            Assert.AreNotEqual(viewModel.Age, 4);
            viewModel22.Age = 5;
            Assert.AreNotEqual(viewModel.Age, 5);

            BindingEngine.SetPropertyBinding(viewModel, "Age", viewModel2, "Age");
            Assert.AreEqual(viewModel.Age, 4);
            BindingEngine.SetPropertyBinding(viewModel, "Age", viewModel22, "Age");
            Assert.AreEqual(viewModel.Age, 5);
        }

        [TestMethod]
        public void Test_BindingEngine_ClearBinding_Object_Property_Expression()
        {
            var viewModel = new TestViewModel();
            var viewModel2 = new TestViewModel2();
            var viewModel22 = new TestViewModel2();

            BindingEngine.SetPropertyBinding(viewModel, "Age", viewModel2, "Age");
            BindingEngine.SetPropertyBinding(viewModel, "Age", viewModel22, "Age");

            viewModel2.Age = 2;
            Assert.AreEqual(viewModel.Age, 2);
            viewModel22.Age = 3;
            Assert.AreEqual(viewModel.Age, 3);

            BindingEngine.ClearBinding(viewModel, model => model.Age);

            viewModel2.Age = 4;
            Assert.AreNotEqual(viewModel.Age, 4);
            viewModel22.Age = 5;
            Assert.AreNotEqual(viewModel.Age, 5);

            BindingEngine.SetPropertyBinding(viewModel, "Age", viewModel2, "Age");
            Assert.AreEqual(viewModel.Age, 4);
            BindingEngine.SetPropertyBinding(viewModel, "Age", viewModel22, "Age");
            Assert.AreEqual(viewModel.Age, 5);
        }

        [TestMethod]
        public void Test_BindingEngine_ClearAllBinding()
        {
            var viewModel = new TestViewModel();
            var view = new TestViewModel2();

            BindingEngine.SetPropertyBinding(view, "Name", viewModel, "Name")
                .SetMode(BindMode.TwoWay);

            viewModel.Name = Name1;
            Assert.AreEqual(view.Name, Name1);

            BindingEngine.ClearAllBindings();

            viewModel.Name = Name2;
            Assert.AreNotEqual(view.Name, Name2);
        }
    }
}