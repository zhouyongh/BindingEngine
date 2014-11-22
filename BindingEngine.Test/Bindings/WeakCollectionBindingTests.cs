using System.Collections.Generic;
using System.Collections.ObjectModel;
using BindingEngine.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Illusion.Utility.Tests
{
    [TestClass]
    public class WeakCollectionBindingTests
    {
        private const string Name1 = "Yohan1";
        private const string Name2 = "Yohan2";

        [TestMethod]
        public void Test_WeakCollectionBinding_OneWay()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            new WeakCollectionBinding(view, "TestViewModelCollections", viewModel, "TestViewModelCollection")
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
        public void Test_WeakCollectionBinding_OneWayToTarget()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            new WeakCollectionBinding(viewModel, "TestViewModelCollection", view, "TestViewModelCollections")
                .Initialize<WeakCollectionBinding>()
                .SetMode(BindMode.OneWayToSource);

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
        public void Test_WeakCollectionBinding_TwoWay()
        {
            var viewModel = new TestViewModel();
            var viewModel3 = new TestViewModel3();

            new WeakCollectionBinding(viewModel3, "TestViewModelCollection", viewModel, "TestViewModelCollection")
                .Initialize<WeakCollectionBinding>()
                .SetMode(BindMode.TwoWay);

            viewModel3.TestViewModelCollection = new ObservableCollection<TestViewModel2>();
            viewModel.TestViewModelCollection = new ObservableCollection<TestViewModel2>();

            var viewModel2 = new TestViewModel2();
            viewModel3.TestViewModelCollection.Add(viewModel2);

            Assert.AreEqual(1, viewModel.TestViewModelCollection.Count);
            Assert.AreEqual(viewModel2, viewModel.TestViewModelCollection[0]);

            viewModel2 = new TestViewModel2();
            viewModel.TestViewModelCollection.Add(viewModel2);

            Assert.AreEqual(2, viewModel3.TestViewModelCollection.Count);
            Assert.AreEqual(viewModel2, viewModel3.TestViewModelCollection[1]);

            viewModel.TestViewModelCollection.Clear();
            Assert.AreEqual(0, viewModel3.TestViewModelCollection.Count);

            viewModel2 = new TestViewModel2();
            viewModel3.TestViewModelCollection.Add(viewModel2);
            Assert.AreEqual(1, viewModel.TestViewModelCollection.Count);

            viewModel3.TestViewModelCollection.RemoveAt(0);
            Assert.AreEqual(0, viewModel.TestViewModelCollection.Count);
        }

        [TestMethod]
        public void Test_WeakCollectionBinding_SourceDataGenerator()
        {
            var viewModel = new TestViewModel();
            var viewmodel3 = new TestViewModel3();

            new WeakCollectionBinding(viewModel, "TestViewModelCollection", viewmodel3, "StringValues")
                .Initialize<WeakCollectionBinding>()
                .SetTargetDataGenerator(new TestDataGenerator())
                .SetMode<WeakCollectionBinding>(BindMode.TwoWay)
                .SetTargetDataParameter("_END");

            var collections = new ObservableCollection<TestViewModel2>();
            viewModel.TestViewModelCollection = collections;

            viewmodel3.StringValues.Add(Name1);
            Assert.AreEqual(1, viewModel.TestViewModelCollection.Count);
            Assert.AreEqual(Name1 + "_END", viewModel.TestViewModelCollection[0].Name);
        }

        [TestMethod]
        public void Test_WeakCollectionBinding_TargetDataGenerator()
        {
            var viewModel = new TestViewModel();
            var viewmodel3 = new TestViewModel3();

            new WeakCollectionBinding(viewmodel3, "StringValues", viewModel, "TestViewModelCollection")
                .Initialize<WeakCollectionBinding>()
                .SetSourceDataGenerator(new TestDataGenerator())
                .SetMode<WeakCollectionBinding>(BindMode.TwoWay)
                .SetSourceDataParameter("_END");

            var collections = new ObservableCollection<TestViewModel2>();
            viewModel.TestViewModelCollection = collections;

            viewmodel3.StringValues.Add(Name1);
            Assert.AreEqual(1, viewModel.TestViewModelCollection.Count);
            Assert.AreEqual(Name1 + "_END", viewModel.TestViewModelCollection[0].Name);
        }


        [TestMethod]
        public void Test_WeakCollectionBinding_CollecionHandler()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            new WeakCollectionBinding(view, "CollectionNames", viewModel, "TestViewModelCollection")
                .Initialize<WeakCollectionBinding>()
                .SetTargetCollectionHandler(new TestDataCollectionHandler());

            view.CollectionNames = new List<string>();
            var collections = new ObservableCollection<TestViewModel2>();
            viewModel.TestViewModelCollection = collections;

            collections.Add(new TestViewModel2 { Name = Name1 });
            Assert.AreEqual(1, view.CollectionNames.Count);
            Assert.AreEqual(Name1, view.CollectionNames[0]);

            collections.RemoveAt(0);
            Assert.AreEqual(0, view.CollectionNames.Count);

            collections.Add(new TestViewModel2 { Name = Name2 });
            collections.Clear();
            Assert.AreEqual(0, view.CollectionNames.Count);

            viewModel = new TestViewModel();
            view = new TestView();

            new WeakCollectionBinding(viewModel, "TestViewModelCollection", view, "CollectionNames")
                .Initialize<WeakCollectionBinding>()
                .SetMode<WeakCollectionBinding>(BindMode.OneWayToSource)
                .SetSourceCollectionHandler(new TestDataCollectionHandler());

            view.CollectionNames = new List<string>();
            collections = new ObservableCollection<TestViewModel2>();
            viewModel.TestViewModelCollection = collections;

            collections.Add(new TestViewModel2 { Name = Name1 });
            Assert.AreEqual(1, view.CollectionNames.Count);
            Assert.AreEqual(Name1, view.CollectionNames[0]);

            collections.RemoveAt(0);
            Assert.AreEqual(0, view.CollectionNames.Count);

            collections.Add(new TestViewModel2 { Name = Name2 });
            collections.Clear();
            Assert.AreEqual(0, view.CollectionNames.Count);
        }

        [TestMethod]
        public void Test_WeakCollectionBinding_SourceGenerator()
        {
            var viewModel = new TestViewModel();
            var viewmodel3 = new TestViewModel3();

            new WeakCollectionBinding(viewModel, "TestViewModelCollection", viewmodel3, "StringValues")
                .Initialize<WeakCollectionBinding>()
                .SetMode<WeakCollectionBinding>(BindMode.TwoWay)
                .SetTargetDataParameter("_END")
                .SetTargetDataGenerator((o, o1) => new TestViewModel2 { Name = o.ToString() + o1 });

            var collections = new ObservableCollection<TestViewModel2>();
            viewModel.TestViewModelCollection = collections;

            viewmodel3.StringValues.Add(Name1);
            Assert.AreEqual(1, viewModel.TestViewModelCollection.Count);
            Assert.AreEqual(Name1 + "_END", viewModel.TestViewModelCollection[0].Name);
        }

        [TestMethod]
        public void Test_WeakCollectionBinding_TargetGenerator()
        {
            var viewModel = new TestViewModel();
            var viewmodel3 = new TestViewModel3();

            new WeakCollectionBinding(viewmodel3, "StringValues", viewModel, "TestViewModelCollection")
                .Initialize<WeakCollectionBinding>()
                .SetMode<WeakCollectionBinding>(BindMode.TwoWay)
                .SetSourceDataParameter("_END")
                .SetSourceDataGenerator((o, o1) => new TestViewModel2 { Name = o.ToString() + o1 });

            var collections = new ObservableCollection<TestViewModel2>();
            viewModel.TestViewModelCollection = collections;

            viewmodel3.StringValues.Add(Name1);
            Assert.AreEqual(1, viewModel.TestViewModelCollection.Count);
            Assert.AreEqual(Name1 + "_END", viewModel.TestViewModelCollection[0].Name);
        }
    }

    public class TestDataGenerator : IDataGenerator
    {
        public object Generate(object value, object parameter)
        {
            return new TestViewModel2 { Name = value.ToString() + parameter };
        }
    }

    public class TestDataCollectionHandler : ICollectionHandler
    {
        public bool AddItem(int index, object item, object source, object sourceProperty)
        {
            var target = item as TestViewModel2;
            var list = sourceProperty as IList<string>;
            if(list == null || target == null)
                return false;

            list.Add(target.Name);
            return true;
        }

        public bool RemoveItem(int index, object item, object source, object sourceProperty)
        {
            var target = item as TestViewModel2;
            var list = sourceProperty as IList<string>;
            if(list == null || target == null)
                return false;

            list.RemoveAt(index);
            return true;
        }

        public bool Clear(object source, object sourceProperty)
        {
            var list = sourceProperty as IList<string>;
            if(list == null)
                return false;

            list.Clear();
            return true;
        }
    }
}