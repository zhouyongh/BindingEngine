using System;
using BindingEngine.Test.Helpers;
using Illusion.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BindingEngine.Test.Internals
{
    [TestClass]
    public class MemoryLeakTests
    {
        private const string Name1 = "yohan";
        private const string Name2 = "yohan2";

        [TestMethod]
        public void Test_MemoryLeak_WeakSource()
        {
            var v = new TestView();
            var reference = new WeakReference(v);
            var vm = new TestViewModel();
            var ws = new WeakTarget(v);
            ws.SetBinding<WeakPropertyBinding>("Text1", vm, "Name");

            vm.Name = "yohan";
            Assert.AreEqual("yohan", v.Text1);

            v = null;
            GC.Collect();
            Assert.IsFalse(reference.IsAlive);

            GC.KeepAlive(ws);
        }

        [TestMethod]
        public void Test_MemoryLeak_WeakEvent()
        {
            // 1. Normal Event will hold the strong reference which prevent the GC collect.
            var view = new TestView();
            var viewModel = new TestViewModel();
            var reference = new WeakReference(view);

            viewModel.TestViewModelEvent += view.OnEventOccured;
            viewModel.RaiseTestViewModelEvent();
            Assert.AreEqual(1, view.EventCount);
            Assert.AreEqual(1, viewModel.GetTestViewModelEventInvocationCount());

            view = null;
            GC.Collect();
            Assert.IsTrue(reference.IsAlive); // Still live

            viewModel.TestViewModelEvent -= reference.GetTarget<TestView>().OnEventOccured;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Assert.IsFalse(reference.IsAlive);

            reference = null;
            Assert.IsNull(reference.GetTarget<TestView>());

            // 2. WeakEvent hold the weak reference which will not prevent GC collect.
            var view2 = new TestView();
            var viewModel2 = new TestViewModel();
            var reference2 = new WeakReference(view2);

            var weakEvent = new WeakEvent(view2);
            weakEvent.AttachEvent(viewModel2, null, "TestViewModelEvent", "OnEventOccured");
            viewModel2.RaiseTestViewModelEvent();


            Assert.AreEqual(1, view2.EventCount);

            view2 = null;
            GC.Collect();
            Assert.IsFalse(reference2.IsAlive);

            viewModel2.RaiseTestViewModelEvent();
            Assert.AreEqual(0, viewModel2.GetTestViewModelEventInvocationCount());
        }

        [TestMethod]
        public void Test_MemoryLeak_BindSource()
        {
            var v = new TestViewModel();
            var v2 = new TestViewModel2();
            var reference = new WeakReference(v);
            var reference2 = new WeakReference(v2);

            int changeCount = 0;
            var bindSource = new BindContext(v, "TestViewModel2.Name");
            bindSource.SourceMode = SourceMode.Property;
            bindSource.SourceChanged += (sender, args) =>
                {
                    changeCount++;
                    Assert.AreNotEqual(args.OldSource, args.NewSource);
                    Assert.AreEqual(SourceMode.Property, args.SourceMode);
                };

            v.TestViewModel2 = v2;
            Assert.AreEqual(0, changeCount);

            v2 = null;
            v.TestViewModel2 = null;
            GC.Collect();
            Assert.IsFalse(reference2.IsAlive);

            v = null;
            GC.Collect();
            Assert.IsFalse(reference.IsAlive);

            GC.KeepAlive(bindSource);
        }

        [TestMethod]
        public void Test_MemoryLeak_WeakBinding_Basic()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();
            var reference = new WeakReference(view);
            var reference2 = new WeakReference(viewModel);

            var binding = new WeakPropertyBinding(view, "Text1", viewModel, "Name")
                .Initialize<WeakPropertyBinding>()
                .SetMode(BindMode.OneWay);
            viewModel.Name = Name1;

            Assert.AreEqual(view.Text1, Name1);

            view = null;
            GC.Collect();
            Assert.IsFalse(reference.IsAlive);

            viewModel = null;
            GC.Collect();
            Assert.IsFalse(reference2.IsAlive);


            GC.KeepAlive(binding);
        }

        [TestMethod]
        public void Test_MemoryLeak_WeakBinding_AttachEvent()
        {
            var viewModel = new TestViewModel();
            var viewModel2 = new TestViewModel2();
            var reference = new WeakReference(viewModel2);
            var reference2 = new WeakReference(viewModel);

            WeakBinding binding = new WeakPropertyBinding(viewModel2, "Name", viewModel, "Name")
                .Initialize<WeakPropertyBinding>()
                .SetMode(BindMode.TwoWay)
                .AttachTargetEvent("TestViewModelEvent");
            viewModel.Name = Name1;

            Assert.AreEqual(viewModel2.Name, Name1);

            viewModel2.Name = Name2;
            Assert.AreNotEqual(viewModel.Name, Name2);

            viewModel2.RaiseTestViewModelEvent();
            Assert.AreEqual(viewModel.Name, Name2);

            viewModel2 = null;
            GC.Collect();
            Assert.IsFalse(reference.IsAlive);

            viewModel = null;
            GC.Collect();
            Assert.IsFalse(reference2.IsAlive);

            GC.KeepAlive(binding);
        }

        [TestMethod]
        public void Test_MemoryLeak_WeakBinding_AttachEvent_2()
        {
            var viewModel = new TestViewModel();
            var viewModel2 = new TestViewModel2();
            var reference = new WeakReference(viewModel2);
            var reference2 = new WeakReference(viewModel);
            var view = new TestView();
            var reference3 = new WeakReference(view);

            WeakBinding binding = new WeakPropertyBinding(viewModel2, "Name", viewModel, "Name")
                .Initialize<WeakPropertyBinding>()
                .SetMode(BindMode.TwoWay)
                .AttachTargetEvent(view, "TestViewEvent");
            viewModel.Name = Name1;

            Assert.AreEqual(viewModel2.Name, Name1);

            viewModel2.Name = Name2;
            Assert.AreNotEqual(viewModel.Name, Name2);

            view.RaiseTestViewEvent();
            Assert.AreEqual(viewModel.Name, Name2);

            view = null;
            GC.Collect();
            Assert.IsFalse(reference3.IsAlive);

            viewModel2 = null;
            GC.Collect();
            Assert.IsFalse(reference.IsAlive);

            viewModel = null;
            GC.Collect();
            Assert.IsFalse(reference2.IsAlive);

            GC.KeepAlive(binding);
        }

        [TestMethod]
        public void Test_MemoryLeak_WeakPropertyBinding_Getter_Setter()
        {
            var viewModel = new TestViewModel();
            var viewModel2 = new TestViewModel2();

            var viewModel1 = new TestViewModel();
            var viewModel21 = new TestViewModel2();

            var reference = new WeakReference(viewModel1);
            var reference2 = new WeakReference(viewModel21);

            WeakBinding binding = new WeakPropertyBinding(viewModel2, null, viewModel, null)
                .Initialize<WeakPropertyBinding>()
                .SetMode<WeakPropertyBinding>(BindMode.TwoWay)
                .SetTargetPropertyGetter(arg => viewModel1.Name)
                .SetSourcePropertyGetter(arg => viewModel21.Name)
                .SetTargetPropertySetter((data, value) => viewModel1.Name = value.ToStringWithoutException())
                .SetSourcePropertySetter((data, value) => viewModel21.Name = value.ToStringWithoutException())
                .AttachTargetEvent("TestViewModelEvent")
                .AttachSourceEvent("TestViewModelEvent");

            viewModel21.Name = Name1;
            viewModel.RaiseTestViewModelEvent();
            Assert.AreEqual(viewModel1.Name, Name1);

            viewModel1.Name = Name2;
            viewModel2.RaiseTestViewModelEvent();
            Assert.AreEqual(viewModel21.Name, Name2);

            viewModel1 = null;
            GC.Collect();
            Assert.IsFalse(reference.IsAlive);

            viewModel21 = null;
            GC.Collect();
            Assert.IsFalse(reference2.IsAlive);

            GC.KeepAlive(binding);
        }
    }
}