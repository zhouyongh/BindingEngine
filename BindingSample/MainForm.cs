using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using Binding = System.Windows.Data.Binding;

namespace BindingSample
{
    public partial class MainForm : Form
    {
        public MainViewModel MainViewModel { get; set; }

        public MainForm()
        {
            InitializeComponent();

            SetBinding();

            ((System.Windows.Controls.UserControl)wpfHost.Child).SetBinding(
                FrameworkElement.DataContextProperty, new Binding("MainViewModel") { Source = DataWarehouse.Instance });
        }

        private void UpdateMainViewModel()
        {
            DataWarehouse.Instance.MainViewModel = new MainViewModel();
        }

        private void SetBinding()
        {
            personListBox.DisplayMember = "Name";

            BindingEngine.SetMethodBinding(viewModelLabel, i => i.Text, DataWarehouse.Instance, o => o.MainViewModel).AttachTargetMethod("MainViewModel", "GetHashCode");
            BindingEngine.SetPropertyBinding(nameTextbox, i => i.Text, DataWarehouse.Instance, o => o.MainViewModel.CurrentPerson.Name).SetMode(BindMode.TwoWay).AttachSourceEvent("TextChanged");

            BindingEngine.SetPropertyBinding(personListBox, o => o.SelectedItem, DataWarehouse.Instance, i => i.MainViewModel.CurrentPerson).SetMode(BindMode.TwoWay).AttachSourceEvent("SelectedValueChanged");
            BindingEngine.SetPropertyBinding(countText, i => i.Text, DataWarehouse.Instance, o => o.MainViewModel.Persons.Count);

            BindingEngine.SetCollectionBinding(personListBox, i => i.Items, DataWarehouse.Instance, o => o.MainViewModel.Persons).Generator = (listbox, o1) =>
            {
                BindingEngine.SetMethodBinding(listbox, null, o1, o => (o as Person).Name)
                             .AttachSourceMethod("RefreshListBoxItem", new BindMethodParameter(BindObjectMode.Target))
                             .SetMode(BindMode.OneWayToTarget)
                             .AttachSourceEvent(BindObjectMode.Target, null, "PropertyChanged");
                return o1;
            };
              
            BindingEngine.SetCommandBinding(addBtn, "Click", DataWarehouse.Instance, i => i.MainViewModel.AddCommand).AddEnableProperty<Button>(button => button.Enabled);
            BindingEngine.SetCommandBinding(removeBtn, "Click", DataWarehouse.Instance, i => i.MainViewModel.RemoveCommand).AddEnableProperty<Button>(button => button.Enabled).Watch<MainViewModel>(o => o.CurrentPerson);
            BindingEngine.SetCommandBinding(clearBtn, "Click", DataWarehouse.Instance, i => i.MainViewModel.ClearCommand).AddEnableProperty<Button>(button => button.Enabled);

            bindStatusLabel.Text = "Binded";
        }

        private void changeViewModel_Click(object sender, EventArgs e)
        {
            UpdateMainViewModel();
        }

        private void setBindingBtn_Click(object sender, EventArgs e)
        {
            SetBinding();
        }

        private void clearBindingBtn_Click(object sender, EventArgs e)
        {
            BindingEngine.ClearAllBindings();
            bindStatusLabel.Text = "UnBinded";
        }

        private void gcCollectbtn_Click(object sender, EventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }

    public class RefreshListBox : ListBox
    {
        public void RefreshListBoxItem(object item)
        {
            int index = Items.IndexOf(item);
            if (index != -1)
            {
                RefreshItem(index);
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
