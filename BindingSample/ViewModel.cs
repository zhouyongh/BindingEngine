namespace Illusion.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Forms;
    using System.Windows.Input;

    public class Command : ICommand
    {
        #region Fields

        private readonly Func<object, bool> canExecute;

        private readonly Action<object> execute;

        #endregion

        #region Constructors and Destructors

        public Command(Action<object> execute)
        {
            this.execute = execute;
        }

        public Command(Action<object> execute, Func<object, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        #endregion

        #region Public Events

        public event EventHandler CanExecuteChanged;

        #endregion

        #region Public Methods and Operators

        public bool CanExecute(object parameter)
        {
            if (this.canExecute != null)
            {
                return this.canExecute(parameter);
            }

            return true;
        }

        public void Execute(object parameter)
        {
            if (this.execute != null)
            {
                this.execute(parameter);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            EventHandler handler = this.CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion
    }

    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }

    public class DataWarehouse : ViewModelBase
    {
        #region Static Fields

        public static DataWarehouse Instance = new DataWarehouse();

        #endregion

        #region Fields

        private ControlViewModel1 viewModel1 = new ControlViewModel1();

        private ControlViewModel2 viewModel2 = new ControlViewModel2();

        private ControlViewModel3 viewModel3 = new ControlViewModel3();

        private ControlViewModel4 viewModel4 = new ControlViewModel4();

        private int selectedIndex;

        #endregion

        #region Public Properties

        public int SelectedIndex
        {
            get
            {
                return this.selectedIndex;
            }

            set
            {
                this.selectedIndex = value;
                this.NotifyPropertyChanged("SelectedIndex");
            }
        }

        public ControlViewModel1 ControlViewModel1
        {
            get
            {
                return this.viewModel1;
            }

            set
            {
                this.viewModel1 = value;
                this.NotifyPropertyChanged("ControlViewModel1");
            }
        }

        public ControlViewModel2 ControlViewModel2
        {
            get
            {
                return this.viewModel2;
            }

            set
            {
                this.viewModel2 = value;
                this.NotifyPropertyChanged("ControlViewModel2");
            }
        }

        public ControlViewModel3 ControlViewModel3
        {
            get
            {
                return this.viewModel3;
            }

            set
            {
                this.viewModel3 = value;
                this.NotifyPropertyChanged("ControlViewModel3");
            }
        }

        public ControlViewModel4 ControlViewModel4
        {
            get
            {
                return this.viewModel4;
            }

            set
            {
                this.viewModel4 = value;
                this.NotifyPropertyChanged("ControlViewModel4");
            }
        }

        #endregion
    }

    public class ControlViewModel1 : ViewModelBase
    {
        #region Fields

        private readonly Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();

        private int number;

        private Person currentPerson;

        private ObservableCollection<Person> persons = new ObservableCollection<Person>();

        private bool checkBoxValue;

        private int comboBoxSelectedIndex;

        private IList<string> comboBoxValues;

        private bool isRadioButton1Selected = true;

        private bool isRadioButton2Selected;

        private double sliderValue = 1;

        private double sliderMinValue = 0;

        private double sliderMaxValue = 10;

        private double scrollBarValue;

        private double scrollBarMaxValue = 1000;

        private string textBoxText = "Hello World";

        private int tabControlSelectedIndex;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlViewModel1"/> class.
        /// </summary>
        public ControlViewModel1()
        {
            this.comboBoxValues = new[] { "comboBoxItem1", "comboBoxItem2", "comboBoxItem3", "comboBoxItem4" };

            this.commands.Add(
                "AddPageCommand",
                new Command(o => this.Persons.Add(new Person { Name = string.Format("page " + number++) })));
            this.commands.Add(
                "RemovePageCommand",
                new Command(o => this.Persons.Remove(this.CurrentPerson), o => this.CurrentPerson != null));

            this.AddCommand.Execute(null);
        }

        #endregion

        #region Public Properties

        public ICommand AddCommand
        {
            get
            {
                return this.commands["AddPageCommand"];
            }
        }

        public ICommand RemoveCommand
        {
            get
            {
                return this.commands["RemovePageCommand"];
            }
        }

        public Dictionary<string, ICommand> Commands
        {
            get
            {
                return this.commands;
            }
        }

        public Person CurrentPerson
        {
            get
            {
                return this.currentPerson;
            }

            set
            {
                this.currentPerson = value;
                this.NotifyPropertyChanged("CurrentPerson");
            }
        }

        public ObservableCollection<Person> Persons
        {
            get
            {
                return this.persons;
            }

            set
            {
                this.persons = value;
                this.NotifyPropertyChanged("Persons");
            }
        }

        public bool CheckBoxValue
        {
            get
            {
                return this.checkBoxValue;
            }

            set
            {
                this.checkBoxValue = value;
                this.NotifyPropertyChanged("CheckBoxValue");
            }
        }

        public IList<string> ComboBoxValues
        {
            get
            {
                return this.comboBoxValues;
            }

            set
            {
                this.comboBoxValues = value;
                this.NotifyPropertyChanged("ComboBoxValues");
            }
        }

        public int ComboBoxSelectedIndex
        {
            get
            {
                return this.comboBoxSelectedIndex;
            }

            set
            {
                this.comboBoxSelectedIndex = value;
                this.NotifyPropertyChanged("ComboBoxSelectedIndex");
            }
        }
        public int TabControlSelectedIndex
        {
            get
            {
                return this.tabControlSelectedIndex;
            }

            set
            {
                this.tabControlSelectedIndex = value;
                this.NotifyPropertyChanged("TabControlSelectedIndex");
            }
        }

        public bool IsRadioButton1Selected
        {
            get
            {
                return this.isRadioButton1Selected;
            }

            set
            {
                this.isRadioButton1Selected = value;
                this.NotifyPropertyChanged("IsRadioButton1Selected");
            }
        }

        public bool IsRadioButton2Selected
        {
            get
            {
                return this.isRadioButton2Selected;
            }

            set
            {
                this.isRadioButton2Selected = value;
                this.NotifyPropertyChanged("IsRadioButton2Selected");
            }
        }
        public string TextBoxText
        {
            get
            {
                return this.textBoxText;
            }

            set
            {
                this.textBoxText = value;
                this.NotifyPropertyChanged("TextBoxText");
            }
        }

        public double SliderValue
        {
            get
            {
                return this.sliderValue;
            }

            set
            {
                this.sliderValue = value;
                this.NotifyPropertyChanged("SliderValue");
            }
        }

        public double SliderMinValue
        {
            get
            {
                return this.sliderMinValue;
            }

            set
            {
                this.sliderMinValue = value;
                this.NotifyPropertyChanged("SliderMaxValue");
            }
        }

        public double SliderMaxValue
        {
            get
            {
                return this.sliderMaxValue;
            }

            set
            {
                this.sliderMaxValue = value;
                this.NotifyPropertyChanged("SliderMaxValue");
            }
        }

        public double ScrollBarValue
        {
            get
            {
                return this.scrollBarValue;
            }

            set
            {
                this.scrollBarValue = value;
                this.NotifyPropertyChanged("ScrollBarValue");
            }
        }

        public double ScrollBarMaxValue
        {
            get
            {
                return this.scrollBarMaxValue;
            }

            set
            {
                this.scrollBarMaxValue = value;
                this.NotifyPropertyChanged("ScrollBarMaxValue");
            }
        }

        #endregion
    }

    public class ControlViewModel2 : ViewModelBase
    {
        #region Fields

        private readonly Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();

        private int number;

        private Person currentPerson;

        private int? key = 2;

        private ObservableCollection<Person> persons = new ObservableCollection<Person>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlViewModel2"/> class.
        /// </summary>
        public ControlViewModel2()
        {
            this.commands.Add(
                "AddCommand",
                new Command(o => this.Persons.Add(new Person { Name = string.Format("yohan " + number++) })));
            this.commands.Add(
                "RemoveCommand",
                new Command(o => this.Persons.Remove(this.CurrentPerson), o => this.CurrentPerson != null));
            this.commands.Add("ClearCommand", new Command(o => this.Persons.Clear()));
        }

        #endregion

        #region Public Properties

        public ICommand AddCommand
        {
            get
            {
                return this.commands["AddCommand"];
            }
        }

        public ICommand ClearCommand
        {
            get
            {
                return this.commands["ClearCommand"];
            }
        }

        public Dictionary<string, ICommand> Commands
        {
            get
            {
                return this.commands;
            }
        }

        public Person CurrentPerson
        {
            get
            {
                return this.currentPerson;
            }

            set
            {
                this.currentPerson = value;
                this.NotifyPropertyChanged("CurrentPerson");
            }
        }

        public int? Key
        {
            get
            {
                return this.key;
            }

            set
            {
                this.key = value;
                this.NotifyPropertyChanged("Key");
            }
        }

        public ObservableCollection<Person> Persons
        {
            get
            {
                return this.persons;
            }

            set
            {
                this.persons = value;
                this.NotifyPropertyChanged("Persons");
            }
        }

        public ICommand RemoveCommand
        {
            get
            {
                return this.commands["RemoveCommand"];
            }
        }

        #endregion
    }

    public class ControlViewModel3 : ViewModelBase
    {
        #region Fields

        private readonly Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();

        private int groupNumber;

        private int personNumber;

        private Person currentPerson;

        private int? key = 2;

        private ObservableCollection<Group> groups = new ObservableCollection<Group>();

        private Group currentGroup;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlViewModel3"/> class.
        /// </summary>
        public ControlViewModel3()
        {
            this.commands.Add(
                "AddGroupCommand",
                new Command(
                    o => this.Groups.Add(new Group { Name = string.Format("Group " + this.groupNumber++) })));
            this.commands.Add(
                "RemoveGroupCommand",
                new Command(
                    o => this.Groups.Remove(this.CurrentGroup), 
                    o => this.CurrentGroup != null));

            this.commands.Add(
                "AddPersonCommand",
                new Command(
                    o => this.CurrentGroup.Persons.Add(new Person { Name = string.Format("yohan " + this.personNumber++) }),
                    o => this.CurrentGroup != null));
            this.commands.Add(
                "RemovePersonCommand",
                new Command(
                    o => this.CurrentGroup.Persons.Remove(this.CurrentPerson), 
                    o => this.CurrentGroup != null && this.CurrentPerson != null));
        }

        #endregion

        #region Public Properties

        public ICommand AddGroupCommand
        {
            get
            {
                return this.commands["AddGroupCommand"];
            }
        }

        public ICommand RemoveGroupCommand
        {
            get
            {
                return this.commands["RemoveGroupCommand"];
            }
        }

        public ICommand AddPersonCommand
        {
            get
            {
                return this.commands["AddPersonCommand"];
            }
        }

        public ICommand RemovePersonCommand
        {
            get
            {
                return this.commands["RemovePersonCommand"];
            }
        }

        public Dictionary<string, ICommand> Commands
        {
            get
            {
                return this.commands;
            }
        }

        public Group CurrentGroup
        {
            get
            {
                return this.currentGroup;
            }

            set
            {
                this.currentGroup = value;
                this.NotifyPropertyChanged("CurrentGroup");
            }
        }

        public Person CurrentPerson
        {
            get
            {
                return this.currentPerson;
            }

            set
            {
                this.currentPerson = value;
                this.NotifyPropertyChanged("CurrentPerson");
            }
        }

        public object SelectedItem
        {
            get
            {
                return (object)this.CurrentGroup ?? this.currentPerson;
            }

            set
            {
                var node = value as TreeNode;
                if (node != null)
                {
                    if (node.Level == 0)
                    {
                        var index = node.TreeView.Nodes.IndexOf(node);
                        this.CurrentGroup = this.Groups[index];
                    }
                    else
                    {
                        var groupIndex = node.TreeView.Nodes.IndexOf(node.Parent);
                        var group = this.Groups[groupIndex];
                        var index = node.Parent.Nodes.IndexOf(node);
                        this.CurrentPerson = group.Persons[index];
                    }
                }
                this.NotifyPropertyChanged("SelectedItem");
            }
        }

        public int? Key
        {
            get
            {
                return this.key;
            }

            set
            {
                this.key = value;
                this.NotifyPropertyChanged("Key");
            }
        }

        public ObservableCollection<Group> Groups
        {
            get
            {
                return this.groups;
            }

            set
            {
                this.groups = value;
                this.NotifyPropertyChanged("Groups");
            }
        }

        #endregion
    }

    public class ControlViewModel4 : ViewModelBase
    {
        #region Fields

        private readonly Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();

        private int personNumber;

        private Person currentPerson;

        private int? key = 2;

        private ObservableCollection<Person> persons = new ObservableCollection<Person>();

        private ObservableCollection<Person> selectPersons = new ObservableCollection<Person>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlViewModel4"/> class.
        /// </summary>
        public ControlViewModel4()
        {
            this.commands.Add(
                "AddPersonCommand",
                new Command(
                    o => this.Persons.Add(new Person { Name = string.Format("yohan " + this.personNumber++) })));
            this.commands.Add(
                "RemovePersonCommand",
                new Command(
                    o =>
                        {
                            if (CurrentPerson != null)
                            {
                                this.Persons.Remove(this.CurrentPerson);
                            }

                            foreach (var selectedPerson in SelectedPersons)
                            {
                                this.Persons.Remove(selectedPerson);
                            }
                        },
                    o => this.SelectedPersons.Any() || this.CurrentPerson != null));
        }

        #endregion

        #region Public Properties

        public ICommand AddPersonCommand
        {
            get
            {
                return this.commands["AddPersonCommand"];
            }
        }

        public ICommand RemovePersonCommand
        {
            get
            {
                return this.commands["RemovePersonCommand"];
            }
        }

        public Dictionary<string, ICommand> Commands
        {
            get
            {
                return this.commands;
            }
        }

        public Person CurrentPerson
        {
            get
            {
                return this.currentPerson;
            }

            set
            {
                this.currentPerson = value;
                this.NotifyPropertyChanged("CurrentPerson");
            }
        }

        public int? Key
        {
            get
            {
                return this.key;
            }

            set
            {
                this.key = value;
                this.NotifyPropertyChanged("Key");
            }
        }

        public ObservableCollection<Person> Persons
        {
            get
            {
                return this.persons;
            }

            set
            {
                this.persons = value;
                this.NotifyPropertyChanged("Persons");
            }
        }

        public ObservableCollection<Person> SelectedPersons
        {
            get
            {
                return this.selectPersons;
            }

            set
            {
                this.selectPersons = value;
                this.NotifyPropertyChanged("SelectedPersons");
            }
        }

        #endregion
    }

    public class Person : ViewModelBase
    {
        #region Fields

        private int age;

        private string name;

        #endregion

        #region Public Properties

        public int Age
        {
            get
            {
                return this.age;
            }

            set
            {
                this.age = value;
                this.NotifyPropertyChanged("Age");
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
                this.NotifyPropertyChanged("Name");
            }
        }

        #endregion
    }

    public class Group : ViewModelBase
    {
        #region Fields

        private string name;

        private ObservableCollection<Person> persons = new ObservableCollection<Person>();

        private bool isSelected;

        #endregion

        #region Public Properties

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
                this.NotifyPropertyChanged("Name");
            }
        }

        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                this.isSelected = value;
                this.NotifyPropertyChanged("IsSelected");
            }
        }

        public ObservableCollection<Person> Persons
        {
            get
            {
                return this.persons;
            }
        }

        #endregion
    }
}