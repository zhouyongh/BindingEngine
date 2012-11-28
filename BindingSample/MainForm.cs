using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;

namespace BindingSample
{
    public partial class MainForm : Form
    {
        public MainViewModel MainViewModel { get; set; }

        public MainForm()
        {
            InitializeComponent();

            MainViewModel = new MainViewModel();

            BindingEngine.SetPropertyBinding(MainViewModel, i => i.CurrentPerson, personListBox, o => o.SelectedItem).SetMode(BindMode.TwoWay).AttachEvent("SelectedValueChanged");
            BindingEngine.SetPropertyBinding(countText, i => i.Text, MainViewModel, o => o.Persons.Count);
            BindingEngine.SetCollectionBinding(personListBox, i => i.Items, MainViewModel.Persons).Generator = data =>
                {
                    Label item = new Label();
                    BindingEngine.SetPropertyBinding(item, i => i.Text, data, o => ((Person) o).Name);
                    return item;
                };

            BindingEngine.SetPropertyBinding(nameTextbox, i => i.Text, MainViewModel, o => o.CurrentPerson.Name).SetMode(BindMode.TwoWay);

            BindingEngine.SetCommandBinding(MainViewModel, i => i.AddCommand, addBtn, "Click").AddEnableProperty<Button>(button => button.Enabled);
            BindingEngine.SetCommandBinding(MainViewModel, i => i.RemoveCommand, removeBtn, "Click").AddEnableProperty<Button>(button => button.Enabled).Watch<MainViewModel>(o => o.CurrentPerson);
            BindingEngine.SetCommandBinding(MainViewModel, i => i.ClearCommand, clearBtn, "Click").AddEnableProperty<Button>(button => button.Enabled);

            ((System.Windows.Controls.UserControl)wpfHost.Child).DataContext = MainViewModel;
        }
    }

    public class DataGenerator<T> : IDataGenerator where T : class
    {
        public object Generate(object value, object parameter)
        {
            return EmitEngine.CreateInstance<T>();
        }
    }

    public class Command : ICommand
    {
        private Action<object> _execute;
        private Func<object, bool> _canExecute;
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
}
