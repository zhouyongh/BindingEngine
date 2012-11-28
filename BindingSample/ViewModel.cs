using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;

namespace BindingSample
{
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

    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            Commands.Add("AddCommand", new Command(o => Persons.Add(new Person() { Name = string.Format("yohan " + number++) })));
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

        private static int number = 0;
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
