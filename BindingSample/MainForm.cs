using System;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Binding = System.Windows.Data.Binding;
using Button = System.Windows.Forms.Button;
using ListBox = System.Windows.Forms.ListBox;

namespace BindingSample
{
    public partial class MainForm : Form
    {
        public MainViewModel MainViewModel { get; set; }

        public MainForm()
        {
            InitializeComponent();
            personListBox.DisplayMember = "Name";

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
            BindingEngine.SetPropertyBinding(keyTextBox, i => i.Text, DataWarehouse.Instance, o => o.MainViewModel.Key)
                         .SetMode(BindMode.TwoWay)
                         .AttachSourceEvent("TextChanged"); ;
            BindingEngine.SetMethodBinding(viewModelLabel, i => i.Text, DataWarehouse.Instance, o => o.MainViewModel)
                         .AttachTargetMethod<DataWarehouse>(o => o.MainViewModel, "GetHashCode");
            BindingEngine.SetPropertyBinding(nameTextbox, i => i.Text, DataWarehouse.Instance, o => o.MainViewModel.CurrentPerson.Name)
                         .SetMode(BindMode.TwoWay)
                         .AttachSourceEvent("TextChanged");

            BindingEngine.SetPropertyBinding(personListBox, o => o.SelectedItem, DataWarehouse.Instance, i => i.MainViewModel.CurrentPerson)
                         .SetMode(BindMode.TwoWay)
                         .AttachSourceEvent("SelectedValueChanged");
            BindingEngine.SetPropertyBinding(countText, i => i.Text, DataWarehouse.Instance, o => o.MainViewModel.Persons.Count);

            BindingEngine.SetCollectionBinding(personListBox, i => i.Items, DataWarehouse.Instance, o => o.MainViewModel.Persons).Handler = new ListBoxCollectionHanlder();

            BindingEngine.SetCommandBinding(addBtn, null, DataWarehouse.Instance, i => i.MainViewModel.AddCommand)
                         .AddEnableProperty<Button>(button => button.Enabled).AttachSourceEvent("Click");
            BindingEngine.SetCommandBinding(removeBtn, null, DataWarehouse.Instance, i => i.MainViewModel.RemoveCommand)
                         .AddEnableProperty<Button>(button => button.Enabled).Watch<MainViewModel>(o => o.CurrentPerson)
                         .AttachSourceEvent("Click");
            BindingEngine.SetCommandBinding(clearBtn, null, DataWarehouse.Instance, i => i.MainViewModel.ClearCommand)
                         .AddEnableProperty<Button>(button => button.Enabled).AttachSourceEvent("Click");

            bindStatusLabel.Text = "Binded";
        }

        private class ListBoxCollectionHanlder : ICollectionHandler
        {
            public bool AddItem(int index, object item, object source, object sourceProperty)
            {
                BindingEngine.SetMethodBinding(source, null, item, o => (o as Person).Name)
                             .AttachSourceMethod("RefreshListBoxItem", new BindMethodParameter(BindObjectMode.Target))
                             .SetMode(BindMode.OneWayToTarget)
                             .AttachSourceEvent(BindObjectMode.Target, null, "PropertyChanged");

                //Return false to let the default collection work, ListBox will and the Person as the ListBoxItem.
                return false;
            }

            public bool RemoveItem(int index, object item, object source, object sourceProperty)
            {
                return false;
            }

            public bool Clear(object source, object sourceProperty)
            {
                return false;
            }
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
}
