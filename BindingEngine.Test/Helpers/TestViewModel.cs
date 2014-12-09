using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace BindingEngine.Test.Helpers
{
    using Illusion.Utility.Tests;

    public class TestViewModel : ViewModelBase
    {
        private ICommand _addAgeCommand;
        private int _age;
        private string _name;
        private ICommand _setAgeCommand;
        private TestViewModel2 _testViewModel2;
        private ObservableCollection<TestViewModel2> _testViewModelCollection;

        public int Age
        {
            get { return _age; }
            set
            {
                _age = value;
                NotifyPropertyChanged("Age");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public TestViewModel2 TestViewModel2
        {
            get { return _testViewModel2; }
            set
            {
                _testViewModel2 = value;
                NotifyPropertyChanged("TestViewModel2");
            }
        }

        public ObservableCollection<TestViewModel2> TestViewModelCollection
        {
            get { return _testViewModelCollection; }
            set
            {
                _testViewModelCollection = value;
                NotifyPropertyChanged("TestViewModelCollection");
            }
        }

        public ICommand AddAgeCommand
        {
            get
            {
                if(_addAgeCommand == null)
                {
                    _addAgeCommand = new DelegateCommand(AddAge, CanAddAge);
                }
                return _addAgeCommand;
            }
        }

        public ICommand SetAgeCommand
        {
            get
            {
                if(_setAgeCommand == null)
                {
                    _setAgeCommand = new DelegateCommand<int>(SetAge, CanSetAge);
                }
                return _setAgeCommand;
            }
        }

        public void AddAge()
        {
            Age++;
        }

        public bool CanAddAge()
        {
            return Age < 2;
        }

        public void SetAge(int age)
        {
            Age = age;
        }

        public bool CanSetAge(int age)
        {
            return age < 3;
        }

        public event TestViewModelEventHandler TestViewModelEvent;

        public void RaiseTestViewModelEvent()
        {
            TestViewModelEventHandler handler = TestViewModelEvent;
            if(handler != null)
                handler(this, new TestViewModelEventArgs());
        }

        public int GetTestViewModelEventInvocationCount()
        {
            TestViewModelEventHandler handler = TestViewModelEvent;
            return handler != null ? handler.GetInvocationList().Length : 0;
        }
    }

    public class TestViewModel2 : ViewModelBase
    {
        private int _age;

        private string _name;
        private TestViewModel3 _testViewModel3;

        private Dictionary<string, string> _stringValues;
        private Dictionary<int, string> _intValues; 

        public int Age
        {
            get { return _age; }
            set
            {
                _age = value;
                NotifyPropertyChanged("Age");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }
        public Dictionary<string, string> StringValues
        {
            get { return _stringValues; }
            set
            {
                _stringValues = value;
                NotifyPropertyChanged("StringValues");
            }
        }

        public Dictionary<int, string> IntValues
        {
            get { return _intValues; }
            set
            {
                _intValues = value;
                NotifyPropertyChanged("IntValues");
            }
        }

        public TestViewModel3 TestViewModel3
        {
            get { return _testViewModel3; }
            set
            {
                _testViewModel3 = value;
                NotifyPropertyChanged("TestViewModel3");
            }
        }

        public int SetAge(int age)
        {
            Age = age;
            return Age;
        }

        public event TestViewModelEventHandler TestViewModelEvent;

        public void RaiseTestViewModelEvent()
        {
            TestViewModelEventHandler handler = TestViewModelEvent;
            if(handler != null)
                handler(this, new TestViewModelEventArgs());
        }
    }

    public class TestViewModel3 : ViewModelBase
    {
        private readonly ICommand _changedNameCommand;
        private int _age;

        private IList<int> _intValues;
        private string _name;
        private IList<string> _stringValues = new ObservableCollection<string>();
        private ObservableCollection<TestViewModel2> _testViewModelCollection;
        private IList<TestViewModel4> _testViewModels;

        private ObservableDictionary<string, TestViewModel4> _testViewModel4s; 

        public TestViewModel3()
        {
            _changedNameCommand = new DelegateCommand<string>(o => { Name = o; }, o => true);
        }

        public int Age
        {
            get { return _age; }
            set
            {
                _age = value;
                NotifyPropertyChanged("Age");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public ObservableCollection<TestViewModel2> TestViewModelCollection
        {
            get { return _testViewModelCollection; }
            set
            {
                _testViewModelCollection = value;
                NotifyPropertyChanged("TestViewModelCollection");
            }
        }

        public IList<int> IntValues
        {
            get { return _intValues; }
            set
            {
                _intValues = value;
                NotifyPropertyChanged("IntValues");
            }
        }

        public IList<string> StringValues
        {
            get { return _stringValues; }
            set
            {
                _stringValues = value;
                NotifyPropertyChanged("StringValues");
            }
        }


        public IList<TestViewModel4> TestViewModels
        {
            get { return _testViewModels; }
            set
            {
                _testViewModels = value;
                NotifyPropertyChanged("TestViewModels");
            }
        }

        public ObservableDictionary<string, TestViewModel4> TestViewModel4s
        {
            get { return _testViewModel4s; }
            set
            {
                _testViewModel4s = value;
                NotifyPropertyChanged("TestViewModel4s");
            }
        }

        public ICommand ChageNameCommand
        {
            get { return _changedNameCommand; }
        }

        public event TestViewModelEventHandler TestViewModelEvent;

        public void RaiseTestViewModelEvent()
        {
            TestViewModelEventHandler handler = TestViewModelEvent;
            if(handler != null)
                handler(this, new TestViewModelEventArgs());
        }
    }

    public class TestViewModel4 : ViewModelBase
    {
        private int _age;

        private string _name;

        public int Age
        {
            get { return _age; }
            set
            {
                _age = value;
                NotifyPropertyChanged("Age");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public event TestViewModelEventHandler TestViewModelEvent;

        public void RaiseTestViewModelEvent()
        {
            TestViewModelEventHandler handler = TestViewModelEvent;
            if(handler != null)
                handler(this, new TestViewModelEventArgs());
        }
    }

    public class TestViewModel5 : ViewModelBase
    {
        private int _age;

        private string _name;
        private TestViewModel _testViewModel;

        public int Age
        {
            get { return _age; }
            set
            {
                _age = value;
                NotifyPropertyChanged("Age");
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public TestViewModel TestViewModel
        {
            get { return _testViewModel; }
            set
            {
                _testViewModel = value;
                NotifyPropertyChanged("TestViewModel");
            }
        }


        public event TestViewModelEventHandler TestViewModelEvent;

        public void RaiseTestViewModelEvent()
        {
            TestViewModelEventHandler handler = TestViewModelEvent;
            if(handler != null)
                handler(this, new TestViewModelEventArgs());
        }
    }

    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class TestViewModelEventArgs : EventArgs
    {
    }

    public delegate void TestViewModelEventHandler(object sender, TestViewModelEventArgs e);
}