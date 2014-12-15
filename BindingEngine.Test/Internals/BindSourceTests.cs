using System.Collections.ObjectModel;
using BindingEngine.Test.Helpers;
using Illusion.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BindingEngine.Test.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;

    [TestClass]
    public class BindSourceTests
    {
        private const string Name = "yohan";

        [TestMethod]
        public void Test_BindSource_NormalProperty()
        {
            var viewModel = new TestViewModel();
            var bindSource = new BindContext(viewModel, "Name");

            Assert.AreEqual(viewModel, bindSource.Source);
            Assert.AreEqual("Name", bindSource.Property);
            Assert.AreEqual(typeof(string), bindSource.PropertyType);

            Assert.AreEqual(null, bindSource.Value);

            viewModel.Name = Name;
            Assert.AreEqual(Name, bindSource.Value);

            Assert.AreEqual(viewModel, bindSource.OriginalSource);
            viewModel = null;
            GC.Collect();
            Assert.AreEqual(null, bindSource.OriginalSource);
        }

        [TestMethod]
        public void Test_BindSource_RecurisonProperty()
        {
            var viewModel = new TestViewModel();
            var bindSource = new BindContext(viewModel, "TestViewModel2.Name");

            Assert.AreEqual(viewModel, bindSource.OriginalSource);
            Assert.AreEqual(null, bindSource.Source);
            Assert.AreEqual(null, bindSource.Value);
            Assert.AreEqual(null, bindSource.PropertyType);

            var viewModel2 = new TestViewModel2();
            viewModel.TestViewModel2 = viewModel2;

            Assert.AreEqual(viewModel2, bindSource.Source);
            Assert.AreEqual(typeof(string), bindSource.PropertyType);

            viewModel2.Name = Name;
            Assert.AreEqual(Name, bindSource.Value);
        }

        [TestMethod]
        public void Test_BindSource_RecurisonProperty2()
        {
            var viewModel = new TestViewModel();
            var bindSource = new BindContext(viewModel, "TestViewModel2.TestViewModel3.Name");

            Assert.AreEqual(null, bindSource.Source);
            Assert.AreEqual(null, bindSource.Value);
            Assert.AreEqual(null, bindSource.PropertyType);

            var viewModel2 = new TestViewModel2();
            viewModel.TestViewModel2 = viewModel2;

            Assert.AreEqual(null, bindSource.Source);
            Assert.AreEqual(null, bindSource.Value);
            Assert.AreEqual(null, bindSource.PropertyType);

            var viewModel3 = new TestViewModel3();
            viewModel2.TestViewModel3 = viewModel3;

            Assert.AreEqual(viewModel3, bindSource.Source);
            Assert.AreEqual(typeof(string), bindSource.PropertyType);

            viewModel3.Name = Name;
            Assert.AreEqual(Name, bindSource.Value);
        }

        [TestMethod]
        public void Test_BindSource_IndexPropertyInt()
        {
            var viewModel = new TestViewModel();
            var bindSource = new BindContext(viewModel, "TestViewModel2.TestViewModel3.IntValues[1]");

            var viewModel2 = new TestViewModel2();
            viewModel.TestViewModel2 = viewModel2;

            var viewModel3 = new TestViewModel3();
            viewModel2.TestViewModel3 = viewModel3;

            Assert.AreEqual(null, bindSource.PropertyType);
            Assert.AreEqual(null, bindSource.Value);

            var list = new ObservableCollection<int>();
            viewModel3.IntValues = list;
            Assert.AreEqual(list, bindSource.Source);
            Assert.AreEqual(typeof(int), bindSource.PropertyType);
            Assert.AreEqual(default(int), bindSource.Value);

            list.Add(1);
            list.Add(2);

            Assert.AreEqual(2, bindSource.Value);

            bindSource.Value = 2;
            Assert.AreEqual(2, list[1]);

            list.RemoveAt(1);

            Assert.AreEqual(default(int), bindSource.Value);
        }

        [TestMethod]
        public void Test_BindSource_IndexPropertyInt2()
        {
            var viewModel = new TestViewModel();
            var bindSource = new BindContext(viewModel, "TestViewModel2.TestViewModel3.TestViewModels[1].Name");

            var viewModel2 = new TestViewModel2();
            viewModel.TestViewModel2 = viewModel2;

            var viewModel3 = new TestViewModel3();
            viewModel2.TestViewModel3 = viewModel3;

            Assert.AreEqual(null, bindSource.Value);
            Assert.AreEqual(null, bindSource.Source);

            var testViewModels = new ObservableCollection<TestViewModel4>();
            viewModel3.TestViewModels = testViewModels;

            Assert.AreEqual(null, bindSource.Source);
            Assert.AreEqual(null, bindSource.Value);

            var t4 = new TestViewModel4();
            var t41 = new TestViewModel4();
            testViewModels.Add(t4);
            testViewModels.Add(t41);

            Assert.AreEqual(t41, bindSource.Source);
            Assert.AreEqual(null, bindSource.Value);

            t41.Name = Name;
            Assert.AreEqual(Name, bindSource.Value);

            testViewModels.RemoveAt(1);

            Assert.AreEqual(null, bindSource.Source);
            Assert.AreEqual(null, bindSource.Value);
        }

        [TestMethod]
        public void Test_BindSource_IndexPropertyDictionary()
        {
            var viewModel = new TestViewModel();
            var bindSource = new BindContext(viewModel, "TestViewModel2.StringValues[yohan]");

            Assert.IsNull(bindSource.Source);
            Assert.IsNull(bindSource.Value);

            var viewModel2 = new TestViewModel2();
            viewModel.TestViewModel2 = viewModel2;

            Assert.IsNull(bindSource.Source);
            Assert.IsNull(bindSource.Value);

            var dictionary = new Dictionary<string, string>();
            viewModel.TestViewModel2.StringValues = dictionary;

            Assert.AreEqual(dictionary, bindSource.Source);
            Assert.IsNull(bindSource.Value);

            viewModel.TestViewModel2.StringValues = new Dictionary<string, string>();

            viewModel.TestViewModel2.StringValues.Add("yohan", "1");
            Assert.AreEqual("1", bindSource.Value);
        }

        [TestMethod]
        public void Test_BindSource_NotifyValueChanged()
        {
            var viewModel = new TestViewModel();
            var bindSource = new BindContext(viewModel, "Name");

            int change = 0;

            bindSource.SourceChanged += (sender, args) => { change++; };

            bindSource.Update(true);

            Assert.AreEqual(1, change);
        }

        [TestMethod]
        public void Test_BindSource_NullProperty()
        {
            var viewModel = new TestViewModel();
            var weakBindSource = new BindContext(viewModel, null);

            Assert.AreEqual(viewModel, weakBindSource.Source);
            Assert.AreEqual(null, weakBindSource.PropertyType);
            Assert.AreEqual(viewModel, weakBindSource.Value);
        }
    }
}