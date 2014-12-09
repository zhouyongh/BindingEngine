using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using BindingEngine.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Illusion.Utility.Tests
{
    using System.Collections.Generic;

    [TestClass]
    public class WeakPropertyBindingTests
    {
        private const string Name1 = "Yohan1";
        private const string Name2 = "Yohan2";

        [TestMethod]
        public void Test_WeakPropertyBinding_OneWay()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            new WeakPropertyBinding(view, "Text1", viewModel, "Name")
                .Initialize<WeakPropertyBinding>()
                .SetMode(BindMode.OneWay);
            viewModel.Name = Name1;

            Assert.AreEqual(view.Text1, Name1);
        }

        [TestMethod]
        public void Test_WeakPropertyBinding_OneTime()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            new WeakPropertyBinding(view, "Text1", viewModel, "Name")
                .Initialize<WeakPropertyBinding>()
                .SetMode(BindMode.OneTime);
            viewModel.Name = null;

            Assert.AreEqual(null, view.Text1);

            viewModel.Name = Name1;
            Assert.AreEqual(view.Text1, Name1);

            viewModel.Name = Name2;
            Assert.AreEqual(view.Text1, Name1);

            viewModel = new TestViewModel();
            view = new TestView();

            new WeakPropertyBinding(view, "Text1", viewModel, "TestViewModel2.Name")
                .Initialize<WeakPropertyBinding>()
                .SetMode(BindMode.OneTime);
            viewModel.Name = null;

            Assert.AreEqual(null, view.Text1);

            viewModel.TestViewModel2 = new TestViewModel2();
            Assert.AreEqual(null, view.Text1);

            viewModel.TestViewModel2.Name = Name1;
            Assert.AreEqual(Name1, view.Text1);

            viewModel.TestViewModel2.Name = Name2;
            Assert.AreEqual(Name1, view.Text1);
        }

        [TestMethod]
        public void Test_WeakPropertyBinding_OneWayToSource()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            new WeakPropertyBinding(view, "Text1", viewModel, "Name")
                .Initialize<WeakPropertyBinding>()
                .SetMode(BindMode.OneWayToSource)
                .AttachTargetEvent("TestViewEvent");

            view.Text1 = Name2;
            view.RaiseTestViewEvent();

            Assert.AreEqual(viewModel.Name, Name2);
        }

        [TestMethod]
        public void Test_WeakPropertyBinding_TwoWay()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            new WeakPropertyBinding(view, "Text1", viewModel, "Name")
                .Initialize<WeakPropertyBinding>().OfType<WeakPropertyBinding>()
                .SetMode(BindMode.TwoWay)
                .AttachTargetEvent("TestViewEvent")
                .AttachSourceEvent("TestViewModelEvent");

            view.Text1 = Name2;
            view.RaiseTestViewEvent();

            Assert.AreEqual(viewModel.Name, Name2);

            viewModel.Name = Name1;
            viewModel.RaiseTestViewModelEvent();

            Assert.AreEqual(view.Text1, Name1);
        }

        [TestMethod]
        public void Test_WeakPropertyBinding_Index_OneWay()
        {
            var view = new TestView();
            var viewModel = new TestViewModel();

            // 1. OneWay binding.
            new WeakPropertyBinding(view, "Text1", viewModel, "TestViewModel2.TestViewModel3.TestViewModels[1].Name")
                .Initialize<WeakPropertyBinding>().OfType<WeakPropertyBinding>()
                .SetMode(BindMode.TwoWay)
                .AttachTargetEvent("TestViewEvent");


            var viewModel2 = new TestViewModel2();
            viewModel.TestViewModel2 = viewModel2;

            var viewModel3 = new TestViewModel3();
            viewModel2.TestViewModel3 = viewModel3;

            var testViewModels = new ObservableCollection<TestViewModel4>();
            viewModel3.TestViewModels = testViewModels;

            var t4 = new TestViewModel4();
            var t41 = new TestViewModel4();
            testViewModels.Add(t4);
            testViewModels.Add(t41);
            t41.Name = Name1;

            Assert.AreEqual(Name1, view.Text1);

            view.Text1 = Name2;
            view.RaiseTestViewEvent();
            Assert.AreEqual(Name2, t41.Name);

            var t42 = new TestViewModel4();
            testViewModels[1] = t42;
            Assert.AreEqual(null, view.Text1);
        }

        [TestMethod]
        public void Test_WeakPropertyBinding_Index_Indexer()
        {
            var view = new TestViewModel();
            var viewModel = new TestViewModel();

            // 1. OneWay binding.
            new WeakPropertyBinding(view, "TestViewModel2.TestViewModel3.TestViewModels[0]", viewModel, "TestViewModel2.TestViewModel3.TestViewModels[1]")
                .Initialize<WeakPropertyBinding>().OfType<WeakPropertyBinding>()
                .SetMode(BindMode.OneWay)
                .AttachTargetEvent("TestViewEvent");


            var viewModel2 = new TestViewModel2();
            viewModel.TestViewModel2 = viewModel2;

            var viewModel3 = new TestViewModel3();
            viewModel2.TestViewModel3 = viewModel3;

            var testViewModels = new ObservableCollection<TestViewModel4>();
            viewModel3.TestViewModels = testViewModels;

            var t4 = new TestViewModel4();
            var t41 = new TestViewModel4();
            testViewModels.Add(t4);
            t41.Name = Name1;

            viewModel2 = new TestViewModel2();
            view.TestViewModel2 = viewModel2;

            viewModel3 = new TestViewModel3();
            viewModel2.TestViewModel3 = viewModel3;

            testViewModels = new ObservableCollection<TestViewModel4>();
            viewModel3.TestViewModels = testViewModels;

            t41 = new TestViewModel4();
            testViewModels.Add(new TestViewModel4());

            Assert.AreEqual(1, view.TestViewModel2.TestViewModel3.TestViewModels.Count);
            viewModel.TestViewModel2.TestViewModel3.TestViewModels.Add(t41);

            Assert.AreEqual(1, view.TestViewModel2.TestViewModel3.TestViewModels.Count);
            Assert.AreEqual(view.TestViewModel2.TestViewModel3.TestViewModels[0], t41);
        }

        [TestMethod]
        public void Test_WeakPropertyBinding_Index_Expression_Int()
        {
            var view = new TestView();
            var viewModel = new TestViewModel();

            // 1. OneWay binding.
            BindingEngine.SetPropertyBinding(view, v => v.Text1, viewModel, vm => vm.TestViewModel2.TestViewModel3.TestViewModels[1].Name)
                .Initialize<WeakPropertyBinding>().OfType<WeakPropertyBinding>()
                .SetMode(BindMode.TwoWay)
                .AttachTargetEvent("TestViewEvent");

            var viewModel2 = new TestViewModel2();
            viewModel.TestViewModel2 = viewModel2;

            var viewModel3 = new TestViewModel3();
            viewModel2.TestViewModel3 = viewModel3;

            var testViewModels = new ObservableCollection<TestViewModel4>();
            viewModel3.TestViewModels = testViewModels;

            var t4 = new TestViewModel4();
            var t41 = new TestViewModel4();
            testViewModels.Add(t4);
            testViewModels.Add(t41);
            t41.Name = Name1;

            Assert.AreEqual(Name1, view.Text1);

            view.Text1 = Name2;
            view.RaiseTestViewEvent();
            Assert.AreEqual(Name2, t41.Name);

            var t42 = new TestViewModel4();
            testViewModels[1] = t42;
            Assert.AreEqual(null, view.Text1);
        }

        [TestMethod]
        public void Test_WeakPropertyBinding_Index_Expression_String()
        {
            var view = new TestView();
            var viewModel = new TestViewModel();

            // 1. OneWay binding.
            BindingEngine.SetPropertyBinding(view, v => v.Text1, viewModel, vm => vm.TestViewModel2.TestViewModel3.TestViewModel4s["12"].Name)
                .Initialize<WeakPropertyBinding>().OfType<WeakPropertyBinding>()
                .SetMode(BindMode.TwoWay)
                .AttachTargetEvent("TestViewEvent");

            var viewModel2 = new TestViewModel2();
            viewModel.TestViewModel2 = viewModel2;

            var viewModel3 = new TestViewModel3();
            viewModel2.TestViewModel3 = viewModel3;

            var testViewModels = new ObservableDictionary<string, TestViewModel4>();
            viewModel3.TestViewModel4s = testViewModels;

            var t4 = new TestViewModel4();
            testViewModels.Add("12", t4);
            t4.Name = Name1;

            Assert.AreEqual(Name1, view.Text1);

            view.Text1 = Name2;
            view.RaiseTestViewEvent();
            Assert.AreEqual(Name2, t4.Name);

            var t42 = new TestViewModel4();
            testViewModels["12"] = t42;
            Assert.AreEqual(null, view.Text1);
        }

        [TestMethod]
        public void Test_WeakPropertyBinding_Index_Observable()
        {
            var view = new TestView();
            var viewModel = new TestViewModel();

            // 1. OneWay binding.
            new WeakPropertyBinding(view, "Text1", viewModel, "TestViewModel2.TestViewModel3.TestViewModels[1].Name")
                .Initialize<WeakPropertyBinding>().OfType<WeakPropertyBinding>()
                .SetMode(BindMode.TwoWay)
                .AttachTargetEvent("TestViewEvent");


            var viewModel2 = new TestViewModel2();
            viewModel.TestViewModel2 = viewModel2;

            var viewModel3 = new TestViewModel3();
            viewModel2.TestViewModel3 = viewModel3;

            var testViewModels = new ObservableCollection<TestViewModel4>();
            viewModel3.TestViewModels = testViewModels;

            var t4 = new TestViewModel4();
            var t41 = new TestViewModel4();
            testViewModels.Add(t4);
            testViewModels.Add(t41);
            t41.Name = Name1;

            Assert.AreEqual(Name1, view.Text1);

            view.Text1 = Name2;
            view.RaiseTestViewEvent();
            Assert.AreEqual(Name2, t41.Name);

            var t42 = new TestViewModel4();
            testViewModels[1] = t42;
            Assert.AreEqual(null, view.Text1);
        }

        [TestMethod]
        public void Test_WeakPropertyBinding_Index_TwoWay()
        {
            var view = new TestViewModel3();
            var viewModel = new TestViewModel();

            // 1. OneWay binding.
            new WeakPropertyBinding(view, "TestViewModels[1].Name", viewModel, "TestViewModel2.TestViewModel3.TestViewModels[1].Name")
                .Initialize<WeakPropertyBinding>().OfType<WeakPropertyBinding>()
                .SetMode(BindMode.TwoWay)
                .AttachTargetEvent("TestViewEvent");


            var viewModel2 = new TestViewModel2();
            viewModel.TestViewModel2 = viewModel2;

            var viewModel3 = new TestViewModel3();
            viewModel2.TestViewModel3 = viewModel3;

            var testViewModels = new ObservableCollection<TestViewModel4>();
            viewModel3.TestViewModels = testViewModels;

            var t4 = new TestViewModel4();
            var t41 = new TestViewModel4();
            testViewModels.Add(t4);
            testViewModels.Add(t41);

            view.TestViewModels = new ObservableCollection<TestViewModel4>();
            view.TestViewModels.Add(new TestViewModel4());
            view.TestViewModels.Add(new TestViewModel4());
            t41.Name = Name1;

            Assert.AreEqual(Name1, view.TestViewModels[1].Name);
        }

        [TestMethod]
        public void Test_WeakPropertyBinding_PropertySetter()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            WeakPropertyBinding propertyBinding = new WeakPropertyBinding(view, null, viewModel, "Name")
                .Initialize<WeakPropertyBinding>()
                .SetMode<WeakPropertyBinding>(BindMode.OneWay)
                .SetTargetPropertySetter(
                    (data, value) => { ((TestView)data.Target.Source).Text1 = value.ToStringWithoutException(); });

            viewModel.Name = Name1;

            Assert.AreEqual(view.Text1, Name1);
            propertyBinding.Clear();


            new WeakPropertyBinding(viewModel, "Name", view, null)
                .Initialize<WeakPropertyBinding>()
                .SetMode<WeakPropertyBinding>(BindMode.OneWayToSource)
                .SetSourcePropertySetter(
                    (data, value) => { ((TestView)data.Source.Source).Text1 = value.ToStringWithoutException(); });

            viewModel.Name = Name2;

            Assert.AreEqual(view.Text1, Name2);
        }

        [TestMethod]
        public void Test_WeakPropertyBinding_PropertyGetter()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            WeakPropertyBinding propertyBinding = new WeakPropertyBinding(view, "Text1", viewModel, "Name")
                .Initialize<WeakPropertyBinding>()
                .SetMode<WeakPropertyBinding>(BindMode.OneWay)
                .SetSourcePropertyGetter(data => ((TestViewModel)data.Source.Source).Name + "1");

            viewModel.Name = Name1;

            Assert.AreEqual(view.Text1, Name1 + "1");

            propertyBinding.Clear();

            propertyBinding = new WeakPropertyBinding(viewModel, "Name", view, "Text1")
                .Initialize<WeakPropertyBinding>()
                .SetMode<WeakPropertyBinding>(BindMode.OneWayToSource)
                .SetTargetPropertyGetter(data => ((TestViewModel)data.Target.Source).Name + "2");

            viewModel.Name = Name1;

            Assert.AreEqual(view.Text1, Name1 + "2");
        }

        [TestMethod]
        public void Test_WeakPropertyBinding_ImplicitConvert()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            //Implicit convert from int to string
            new WeakPropertyBinding(view, "Text1", viewModel, "Age")
                .Initialize<WeakPropertyBinding>();

            viewModel.Age = 2;

            Assert.AreEqual(view.Text1, "2");
        }

        [TestMethod]
        public void Test_WeakPropertyBinding_DataConverter()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            const int number = 6;

            // Implicit convert from int to string
            new WeakPropertyBinding(view, "Text1", viewModel, "Age")
                .Initialize<WeakPropertyBinding>()
                .SetMode<WeakPropertyBinding>(BindMode.TwoWay)
                .SetConverter<WeakPropertyBinding>(new TestDataConverter())
                .SetParameter<WeakPropertyBinding>(number)
                .AttachTargetEvent("TestViewEvent");

            viewModel.Age = 2;

            Assert.AreEqual(view.Text1, "2" + number);

            view.Text1 = "7" + number;
            view.RaiseTestViewEvent();

            Assert.AreEqual(viewModel.Age, 7);
        }

        [TestMethod]
        public void Test_WeakPropertyBinding_DataConverter_WithParameter()
        {
            var viewModel = new TestViewModel();
            var view = new TestView();

            new WeakPropertyBinding(view, "Text1", viewModel, "Age")
                .Initialize<WeakPropertyBinding>()
                .SetMode<WeakPropertyBinding>(BindMode.TwoWay)
                .SetConverter(new TestDataParameterConverter())
                .SetParameter(9)
                .AttachTargetEvent("TestViewEvent");


            viewModel.Age = 2;

            Assert.AreEqual(view.Text1, TestDataParameterConverter.Prefix + 11);

            view.Text1 = TestDataParameterConverter.Prefix + "13";
            view.RaiseTestViewEvent();

            Assert.AreEqual(viewModel.Age, 4);
        }
    }

    [Serializable]
    public class ObservableDictionary<TKey, TValue> :
        IDictionary<TKey, TValue>,
        IDictionary,
        ISerializable,
        IDeserializationCallback,
        INotifyCollectionChanged,
        INotifyPropertyChanged
    {
        #region Constructors

        #region public

        public ObservableDictionary()
        {
            _keyedEntryCollection = new KeyedDictionaryEntryCollection<TKey>();
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _keyedEntryCollection = new KeyedDictionaryEntryCollection<TKey>();

            foreach (KeyValuePair<TKey, TValue> entry in dictionary)
                DoAddEntry((TKey)entry.Key, (TValue)entry.Value);
        }

        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            _keyedEntryCollection = new KeyedDictionaryEntryCollection<TKey>(comparer);
        }

        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            _keyedEntryCollection = new KeyedDictionaryEntryCollection<TKey>(comparer);

            foreach (KeyValuePair<TKey, TValue> entry in dictionary)
                DoAddEntry((TKey)entry.Key, (TValue)entry.Value);
        }

        #endregion public

        #region protected

        protected ObservableDictionary(SerializationInfo info, StreamingContext context)
        {
            _siInfo = info;
        }

        #endregion protected

        #endregion constructors

        #region Properties

        #region public

        public IEqualityComparer<TKey> Comparer
        {
            get { return _keyedEntryCollection.Comparer; }
        }

        public int Count
        {
            get { return _keyedEntryCollection.Count; }
        }

        public Dictionary<TKey, TValue>.KeyCollection Keys
        {
            get { return TrueDictionary.Keys; }
        }

        public TValue this[TKey key]
        {
            get { return (TValue)_keyedEntryCollection[key].Value; }
            set { DoSetEntry(key, value); }
        }

        public Dictionary<TKey, TValue>.ValueCollection Values
        {
            get { return TrueDictionary.Values; }
        }

        #endregion public

        #region private

        private Dictionary<TKey, TValue> TrueDictionary
        {
            get
            {
                if (_dictionaryCacheVersion != _version)
                {
                    _dictionaryCache.Clear();
                    foreach (DictionaryEntry entry in _keyedEntryCollection)
                        _dictionaryCache.Add((TKey)entry.Key, (TValue)entry.Value);
                    _dictionaryCacheVersion = _version;
                }
                return _dictionaryCache;
            }
        }

        #endregion private

        #endregion properties

        #region Methods

        #region public

        public void Add(TKey key, TValue value)
        {
            DoAddEntry(key, value);
        }

        public void Clear()
        {
            DoClearEntries();
        }

        public bool ContainsKey(TKey key)
        {
            return _keyedEntryCollection.Contains(key);
        }

        public bool ContainsValue(TValue value)
        {
            return TrueDictionary.ContainsValue(value);
        }

        public IEnumerator GetEnumerator()
        {
            return new Enumerator<TKey, TValue>(this, false);
        }

        public bool Remove(TKey key)
        {
            return DoRemoveEntry(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            bool result = _keyedEntryCollection.Contains(key);
            value = result ? (TValue)_keyedEntryCollection[key].Value : default(TValue);
            return result;
        }

        #endregion public

        #region protected

        protected virtual bool AddEntry(TKey key, TValue value)
        {
            _keyedEntryCollection.Add(new DictionaryEntry(key, value));
            return true;
        }

        protected virtual bool ClearEntries()
        {
            // check whether there are entries to clear
            bool result = (Count > 0);
            if (result)
            {
                // if so, clear the dictionary
                _keyedEntryCollection.Clear();
            }
            return result;
        }

        protected int GetIndexAndEntryForKey(TKey key, out DictionaryEntry entry)
        {
            entry = new DictionaryEntry();
            int index = -1;
            if (_keyedEntryCollection.Contains(key))
            {
                entry = _keyedEntryCollection[key];
                index = _keyedEntryCollection.IndexOf(entry);
            }
            return index;
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, args);
        }

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        protected virtual bool RemoveEntry(TKey key)
        {
            // remove the entry
            return _keyedEntryCollection.Remove(key);
        }

        protected virtual bool SetEntry(TKey key, TValue value)
        {
            bool keyExists = _keyedEntryCollection.Contains(key);

            // if identical key/value pair already exists, nothing to do
            if (keyExists && object.Equals(value, _keyedEntryCollection[key].Value))
                return false;

            // otherwise, remove the existing entry
            if (keyExists)
                _keyedEntryCollection.Remove(key);

            // add the new entry
            _keyedEntryCollection.Add(new DictionaryEntry(key, value));

            return true;
        }

        #endregion protected

        #region private

        private void DoAddEntry(TKey key, TValue value)
        {
            if (AddEntry(key, value))
            {
                _version++;

                DictionaryEntry entry;
                int index = GetIndexAndEntryForKey(key, out entry);
                FireEntryAddedNotifications(entry, index);
            }
        }

        private void DoClearEntries()
        {
            if (ClearEntries())
            {
                _version++;
                FireResetNotifications();
            }
        }

        private bool DoRemoveEntry(TKey key)
        {
            DictionaryEntry entry;
            int index = GetIndexAndEntryForKey(key, out entry);

            bool result = RemoveEntry(key);
            if (result)
            {
                _version++;
                if (index > -1)
                    FireEntryRemovedNotifications(entry, index);
            }

            return result;
        }

        private void DoSetEntry(TKey key, TValue value)
        {
            DictionaryEntry entry;
            int index = GetIndexAndEntryForKey(key, out entry);

            if (SetEntry(key, value))
            {
                _version++;

                // if prior entry existed for this key, fire the removed notifications
                if (index > -1)
                {
                    FireEntryRemovedNotifications(entry, index);

                    // force the property change notifications to fire for the modified entry
                    _countCache--;
                }

                // then fire the added notifications
                index = GetIndexAndEntryForKey(key, out entry);
                FireEntryAddedNotifications(entry, index);
            }
        }

        private void FireEntryAddedNotifications(DictionaryEntry entry, int index)
        {
            // fire the relevant PropertyChanged notifications
            FirePropertyChangedNotifications();

            // fire CollectionChanged notification
            if (index > -1)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>((TKey)entry.Key, (TValue)entry.Value), index));
            else
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void FireEntryRemovedNotifications(DictionaryEntry entry, int index)
        {
            // fire the relevant PropertyChanged notifications
            FirePropertyChangedNotifications();

            // fire CollectionChanged notification
            if (index > -1)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>((TKey)entry.Key, (TValue)entry.Value), index));
            else
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void FirePropertyChangedNotifications()
        {
            if (Count != _countCache)
            {
                _countCache = Count;
                OnPropertyChanged("Count");
                OnPropertyChanged("Item[]");
                OnPropertyChanged("Keys");
                OnPropertyChanged("Values");
            }
        }

        private void FireResetNotifications()
        {
            // fire the relevant PropertyChanged notifications
            FirePropertyChangedNotifications();

            // fire CollectionChanged notification
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #endregion private

        #endregion methods

        #region Interfaces

        #region IDictionary<TKey, TValue>

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            DoAddEntry(key, value);
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            return DoRemoveEntry(key);
        }

        bool IDictionary<TKey, TValue>.ContainsKey(TKey key)
        {
            return _keyedEntryCollection.Contains(key);
        }

        bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
        {
            return TryGetValue(key, out value);
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get { return Keys; }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get { return Values; }
        }

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get { return (TValue)_keyedEntryCollection[key].Value; }
            set { DoSetEntry(key, value); }
        }

        #endregion IDictionary<TKey, TValue>

        #region IDictionary

        void IDictionary.Add(object key, object value)
        {
            DoAddEntry((TKey)key, (TValue)value);
        }

        void IDictionary.Clear()
        {
            DoClearEntries();
        }

        bool IDictionary.Contains(object key)
        {
            return _keyedEntryCollection.Contains((TKey)key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new Enumerator<TKey, TValue>(this, true);
        }

        bool IDictionary.IsFixedSize
        {
            get { return false; }
        }

        bool IDictionary.IsReadOnly
        {
            get { return false; }
        }

        object IDictionary.this[object key]
        {
            get { return _keyedEntryCollection[(TKey)key].Value; }
            set { DoSetEntry((TKey)key, (TValue)value); }
        }

        ICollection IDictionary.Keys
        {
            get { return Keys; }
        }

        void IDictionary.Remove(object key)
        {
            DoRemoveEntry((TKey)key);
        }

        ICollection IDictionary.Values
        {
            get { return Values; }
        }

        #endregion IDictionary

        #region ICollection<KeyValuePair<TKey, TValue>>

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> kvp)
        {
            DoAddEntry(kvp.Key, kvp.Value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            DoClearEntries();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> kvp)
        {
            return _keyedEntryCollection.Contains(kvp.Key);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("CopyTo() failed:  array parameter was null");
            }
            if ((index < 0) || (index > array.Length))
            {
                throw new ArgumentOutOfRangeException("CopyTo() failed:  index parameter was outside the bounds of the supplied array");
            }
            if ((array.Length - index) < _keyedEntryCollection.Count)
            {
                throw new ArgumentException("CopyTo() failed:  supplied array was too small");
            }

            foreach (DictionaryEntry entry in _keyedEntryCollection)
                array[index++] = new KeyValuePair<TKey, TValue>((TKey)entry.Key, (TValue)entry.Value);
        }

        int ICollection<KeyValuePair<TKey, TValue>>.Count
        {
            get { return _keyedEntryCollection.Count; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> kvp)
        {
            return DoRemoveEntry(kvp.Key);
        }

        #endregion ICollection<KeyValuePair<TKey, TValue>>

        #region ICollection

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_keyedEntryCollection).CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return _keyedEntryCollection.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return ((ICollection)_keyedEntryCollection).IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection)_keyedEntryCollection).SyncRoot; }
        }

        #endregion ICollection

        #region IEnumerable<KeyValuePair<TKey, TValue>>

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return new Enumerator<TKey, TValue>(this, false);
        }

        #endregion IEnumerable<KeyValuePair<TKey, TValue>>

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable

        #region ISerializable

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            Collection<DictionaryEntry> entries = new Collection<DictionaryEntry>();
            foreach (DictionaryEntry entry in _keyedEntryCollection)
                entries.Add(entry);
            info.AddValue("entries", entries);
        }

        #endregion ISerializable

        #region IDeserializationCallback

        public virtual void OnDeserialization(object sender)
        {
            if (_siInfo != null)
            {
                Collection<DictionaryEntry> entries = (Collection<DictionaryEntry>)
                    _siInfo.GetValue("entries", typeof(Collection<DictionaryEntry>));
                foreach (DictionaryEntry entry in entries)
                    AddEntry((TKey)entry.Key, (TValue)entry.Value);
            }
        }

        #endregion IDeserializationCallback

        #region INotifyCollectionChanged

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add { CollectionChanged += value; }
            remove { CollectionChanged -= value; }
        }

        protected virtual event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion INotifyCollectionChanged

        #region INotifyPropertyChanged

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { PropertyChanged += value; }
            remove { PropertyChanged -= value; }
        }

        protected virtual event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged

        #endregion interfaces

        #region Protected classes

        #region KeyedDictionaryEntryCollection<T>

        protected class KeyedDictionaryEntryCollection<T> : KeyedCollection<T, DictionaryEntry>
        {
            #region constructors

            #region public

            public KeyedDictionaryEntryCollection() : base() { }

            public KeyedDictionaryEntryCollection(IEqualityComparer<T> comparer) : base(comparer) { }

            #endregion public

            #endregion constructors

            #region methods

            #region protected

            protected override T GetKeyForItem(DictionaryEntry entry)
            {
                return (T)entry.Key;
            }

            #endregion protected

            #endregion methods
        }

        #endregion KeyedDictionaryEntryCollection<TKey>

        #endregion protected classes

        #region Public Structures

        #region Enumerator

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct Enumerator<TKey1, TValue1> : IEnumerator<KeyValuePair<TKey1, TValue1>>, IDisposable, IDictionaryEnumerator, IEnumerator
        {
            #region constructors

            internal Enumerator(ObservableDictionary<TKey1, TValue1> dictionary, bool isDictionaryEntryEnumerator)
            {
                _dictionary = dictionary;
                _version = dictionary._version;
                _index = -1;
                _isDictionaryEntryEnumerator = isDictionaryEntryEnumerator;
                _current = new KeyValuePair<TKey1, TValue1>();
            }

            #endregion constructors

            #region properties

            #region public

            public KeyValuePair<TKey1, TValue1> Current
            {
                get
                {
                    ValidateCurrent();
                    return _current;
                }
            }

            #endregion public

            #endregion properties

            #region methods

            #region public

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                ValidateVersion();
                _index++;
                if (_index < _dictionary._keyedEntryCollection.Count)
                {
                    _current = new KeyValuePair<TKey1, TValue1>((TKey1)_dictionary._keyedEntryCollection[_index].Key, (TValue1)_dictionary._keyedEntryCollection[_index].Value);
                    return true;
                }
                _index = -2;
                _current = new KeyValuePair<TKey1, TValue1>();
                return false;
            }

            #endregion public

            #region private

            private void ValidateCurrent()
            {
                if (_index == -1)
                {
                    throw new InvalidOperationException("The enumerator has not been started.");
                }
                else if (_index == -2)
                {
                    throw new InvalidOperationException("The enumerator has reached the end of the collection.");
                }
            }

            private void ValidateVersion()
            {
                if (_version != _dictionary._version)
                {
                    throw new InvalidOperationException("The enumerator is not valid because the dictionary changed.");
                }
            }

            #endregion private

            #endregion methods

            #region IEnumerator implementation

            object IEnumerator.Current
            {
                get
                {
                    ValidateCurrent();
                    if (_isDictionaryEntryEnumerator)
                    {
                        return new DictionaryEntry(_current.Key, _current.Value);
                    }
                    return new KeyValuePair<TKey1, TValue1>(_current.Key, _current.Value);
                }
            }

            void IEnumerator.Reset()
            {
                ValidateVersion();
                _index = -1;
                _current = new KeyValuePair<TKey1, TValue1>();
            }

            #endregion IEnumerator implemenation

            #region IDictionaryEnumerator implemenation

            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    ValidateCurrent();
                    return new DictionaryEntry(_current.Key, _current.Value);
                }
            }
            object IDictionaryEnumerator.Key
            {
                get
                {
                    ValidateCurrent();
                    return _current.Key;
                }
            }
            object IDictionaryEnumerator.Value
            {
                get
                {
                    ValidateCurrent();
                    return _current.Value;
                }
            }

            #endregion

            #region fields

            private ObservableDictionary<TKey1, TValue1> _dictionary;
            private int _version;
            private int _index;
            private KeyValuePair<TKey1, TValue1> _current;
            private bool _isDictionaryEntryEnumerator;

            #endregion fields
        }

        #endregion Enumerator

        #endregion public structures

        #region Fields

        protected KeyedDictionaryEntryCollection<TKey> _keyedEntryCollection;

        private int _countCache = 0;
        private Dictionary<TKey, TValue> _dictionaryCache = new Dictionary<TKey, TValue>();
        private int _dictionaryCacheVersion = 0;
        private int _version = 0;

        [NonSerialized]
        private SerializationInfo _siInfo = null;

        #endregion fields
    }
}