using System.Windows.Forms;
using BindingEngine.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Illusion.Utility.Tests
{
    using System;

    [TestClass]
    public class WeakNotifyBindingTests
    {
        [TestMethod]
        public void Test_WeakNotify_OneWay()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            int changedCount = 0;

            BindingValueChangedHandler changeHandler = (source, args) =>
                {
                    changedCount++;
                    Assert.IsTrue(args.Data != null);
                    Assert.IsNull(args.OldValue);
                    Assert.IsNull(args.NewValue);
                };

            new WeakNotifyBinding(viewModel, "Age", view, null)
                .Initialize<WeakNotifyBinding>()
                .AttachSourceEvent("TestViewEvent")
                .OfType<WeakNotifyBinding>().SetSourceChanged(changeHandler);

            Assert.AreEqual(0, changedCount);

            GC.Collect();

            view.RaiseTestViewEvent();

            Assert.AreEqual(1, changedCount);
        }

        [TestMethod]
        public void Test_WeakNotify_OneWayToSource()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            int changedCount = 0;

            new WeakNotifyBinding(view, null, viewModel, "Age")
                .Initialize<WeakNotifyBinding>()
                .AttachTargetEvent("TestViewEvent")
                .OfType<WeakNotifyBinding>()
                .SetTargetChanged<WeakNotifyBinding>((source, args) => { changedCount++; });

            Assert.AreEqual(0, changedCount);

            view.RaiseTestViewEvent();

            Assert.AreEqual(1, changedCount);
        }

        [TestMethod]
        public void Test_WeakNotify_TwoWay()
        {
            var button = new Button();
            var view = new TestView();

            int changedCount = 0;
            int targetCount = 0;

            new WeakNotifyBinding(view, null, button, null)
                .Initialize<WeakNotifyBinding>()
                .AttachTargetEvent("TestViewEvent")
                .AttachSourceEvent("Click")
                .OfType<WeakNotifyBinding>()
                .SetTargetChanged((source, args) => { changedCount++; })
                .SetSourceChanged((source, args) => { targetCount++; });

            Assert.AreEqual(0, changedCount);

            view.RaiseTestViewEvent();
            Assert.AreEqual(1, changedCount);

            button.PerformClick();
            Assert.AreEqual(1, targetCount);
        }

        [TestMethod]
        public void Test_WeakNotify_WeakReference()
        {
            var button = new Button();
            var view = new TestView();

            int changedCount = 0;
            int targetCount = 0;

            BindingValueChangedHandler sourceChanged = (source, args) => { changedCount++; };
            BindingValueChangedHandler targetChanged = (source, args) => { targetCount++; };


            new WeakNotifyBinding(view, null, button, null)
                .Initialize<WeakNotifyBinding>()
                .AttachTargetEvent("TestViewEvent")
                .AttachSourceEvent("Click")
                .OfType<WeakNotifyBinding>()
                .SetTargetChanged(sourceChanged, true)
                .SetSourceChanged(targetChanged, true);

            Assert.AreEqual(0, changedCount);

            view.RaiseTestViewEvent();
            Assert.AreEqual(1, changedCount);

            button.PerformClick();
            Assert.AreEqual(1, targetCount);

            sourceChanged = null;
            GC.Collect();
            view.RaiseTestViewEvent();
            Assert.AreEqual(1, changedCount);


            targetChanged = null;
            GC.Collect();
            button.PerformClick();
            Assert.AreEqual(1, targetCount);
        }
    }
}