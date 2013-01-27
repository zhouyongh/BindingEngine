using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BindingSample
{
    public class Command : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;
        public event EventHandler CanExecuteChanged;

        public Command(Action<object> execute)
        {
            _execute = execute;
        }

        public Command(Action<object> execute, Func<object, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute(parameter);
            return true;
        }

        public void Execute(object parameter)
        {
            if (_execute != null)
                _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }

    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class DataWarehouse : ViewModelBase
    {
        private MainViewModel _viewModel = new MainViewModel();
        public MainViewModel MainViewModel
        {
            get { return _viewModel; }
            set
            {
                _viewModel = value;
                NotifyPropertyChanged("MainViewModel");
            }
        }

        public static DataWarehouse Instance = new DataWarehouse();
    }

    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            Commands.Add("AddCommand", new Command(o => Persons.Add(new Person() { Name = string.Format("yohan " + _number++) })));
            Commands.Add("RemoveCommand", new Command(o => Persons.Remove(CurrentPerson), o => CurrentPerson != null));
            Commands.Add("ClearCommand", new Command(o => Persons.Clear()));
        }

        private ObservableCollection<Person> _persons = new ObservableCollection<Person>();
        public ObservableCollection<Person> Persons
        {
            get { return _persons; }
            set
            {
                _persons = value;
                NotifyPropertyChanged("Persons");
            }
        }

        private int? _key = 2;
        public int? Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
                NotifyPropertyChanged("Key");
            }
        }

        private Person _currentPerson = new Person();
        public Person CurrentPerson
        {
            get { return _currentPerson; }
            set
            {
                _currentPerson = value;
                NotifyPropertyChanged("CurrentPerson");
            }
        }

        private static int _number;
        public ICommand AddCommand
        {
            get
            {
                return Commands["AddCommand"];
            }
        }

        public ICommand RemoveCommand
        {
            get
            {
                return Commands["RemoveCommand"];
            }
        }

        public ICommand ClearCommand
        {
            get { return Commands["ClearCommand"]; }
        }

        public Dictionary<string, ICommand> Commands = new Dictionary<string, ICommand>();
    }

    public class Person : ViewModelBase
    {
        private int _age;
        public int Age
        {
            get
            {
                return _age;
            }
            set
            {
                _age = value;
                NotifyPropertyChanged("Age");
            }
        }

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }
    }
}
