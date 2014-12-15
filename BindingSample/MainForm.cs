namespace Illusion.Sample
{
    using System;
    using System.Windows;
    using System.Windows.Forms;

    using Illusion.Utility;

    using Binding = System.Windows.Data.Binding;
    using Button = System.Windows.Forms.Button;
    using ListBox = System.Windows.Forms.ListBox;

    /// <summary>
    ///     Class MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        internal static BindingManager WinformBinding = new BindingManager();

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainForm" /> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();

            this.personListBox.DisplayMember = "Name";

            this.dataGrid.Columns[0].ValueType = typeof(string);
            this.dataGrid.Columns[1].ValueType = typeof(int);
            this.dataGrid.Columns[2].ValueType = typeof(string);

            DynamicEngine.SetBindingManager(new EmitManager());

            this.SetGlobalBinding();
        }

        public System.Windows.Controls.TabControl WPFTabControl
        {
            get
            {
                return ((WPFControl)this.elementHost2.Child).TabControl;
            }
        }

        #endregion

        #region Methods

        public void SetGlobalBinding()
        {
            BindingEngine.SetPropertyBinding(this.winformBindingCbx, i => i.Checked, this, null, false)
                         .SetMode(BindMode.OneWayToSource)
                         .AttachTargetEvent("CheckedChanged")
                         .OfType<WeakPropertyBinding>()
                         .SetSourcePropertySetter((data, o) => ((MainForm)data.Source.Source).UpdateWinformBinding((bool)o))
                         .Activate();

            BindingEngine.SetPropertyBinding(this.wpfBindingCbx, i => i.Checked, this, null, false)
                         .SetMode(BindMode.OneWayToSource)
                         .AttachTargetEvent("CheckedChanged")
                         .OfType<WeakPropertyBinding>()
                         .SetSourcePropertySetter((data, o) => ((MainForm)data.Source.Source).UpdateWpfBinding((bool)o))
                         .Activate();

            BindingEngine.SetPropertyBinding(this.winformTabControl, i => i.SelectedIndex, DataWarehouse.Instance, o => o.SelectedIndex)
                          .SetMode(BindMode.TwoWay)
                          .AttachTargetEvent("SelectedIndexChanged");

            BindingEngine.SetPropertyBinding(this.winformLabel, i => i.Text, DataWarehouse.Instance, o => o.SelectedIndex)
                         .SetSourceBindMode(SourceMode.Property)
                         .OfType<WeakPropertyBinding>()
                         .SetTargetPropertySetter(this.UpdateViewModelLabelText);

            BindingEngine.SetPropertyBinding(this.wpfLabel, i => i.Text, DataWarehouse.Instance, o => o.SelectedIndex)
                         .SetSourceBindMode(SourceMode.Property)
                         .OfType<WeakPropertyBinding>()
                         .SetTargetPropertySetter(this.UpdateViewModelLabelText);
        }

        public void UpdateWinformBinding(bool bind)
        {
            if (bind)
            {
                this.SetWinformBinding1();
                this.SetWinformBinding2();
                this.SetWinformBinding3();
                this.SetWinformBinding4();
            }
            else
            {
                this.ClearWinformBinding();
            }
        }

        public void UpdateWpfBinding(bool bind)
        {
            if (bind)
            {
                this.SetWpfBinding();
            }
            else
            {
                this.ClearWpfBinding();
            }
        }

        private void UpdateViewModelLabelText(BindData data, object o)
        {
            var index = (int)o;
            switch (index)
            {
                case 0:
                    data.Target.Value = ((DataWarehouse)data.Source.Source).ControlViewModel1.GetHashCode().ToStringWithoutException();
                    break;
                case 1:
                    data.Target.Value = ((DataWarehouse)data.Source.Source).ControlViewModel2.GetHashCode().ToStringWithoutException();
                    break;
                default:
                    data.Target.Value = "null";
                    break;
            }
        }

        private void SetWpfBinding()
        {
            ((FrameworkElement)this.WPFTabControl.Items[0]).SetBinding(
                FrameworkElement.DataContextProperty,
                new Binding("ControlViewModel1") { Source = DataWarehouse.Instance });

            ((FrameworkElement)this.WPFTabControl.Items[1]).SetBinding(
                FrameworkElement.DataContextProperty,
                new Binding("ControlViewModel2") { Source = DataWarehouse.Instance });

            ((FrameworkElement)this.WPFTabControl.Items[2]).SetBinding(
                FrameworkElement.DataContextProperty,
                new Binding("ControlViewModel3") { Source = DataWarehouse.Instance });

            ((FrameworkElement)this.WPFTabControl.Items[3]).SetBinding(
                FrameworkElement.DataContextProperty,
                new Binding("ControlViewModel4") { Source = DataWarehouse.Instance });
        }

        private void SetWinformBinding1()
        {
            // CheckBox
            WinformBinding.SetPropertyBinding(this.checkBox1, i => i.Checked, DataWarehouse.Instance, o => o.ControlViewModel1.CheckBoxValue)
                          .SetMode(BindMode.TwoWay)
                          .AttachTargetEvent("CheckedChanged");

            // ComboBox
            WinformBinding.SetCollectionBinding(this.comboBox1, i => i.Items, DataWarehouse.Instance, o => o.ControlViewModel1.ComboBoxValues);
            WinformBinding.SetPropertyBinding(this.comboBox1, i => i.SelectedIndex, DataWarehouse.Instance, o => o.ControlViewModel1.ComboBoxSelectedIndex)
                          .SetMode(BindMode.TwoWay)
                          .AttachTargetEvent("SelectedIndexChanged");

            // RadioButton
            WinformBinding.SetPropertyBinding(this.radioButton1, i => i.Checked, DataWarehouse.Instance, o => o.ControlViewModel1.IsRadioButton1Selected)
                          .SetMode(BindMode.TwoWay)
                          .AttachTargetEvent("CheckedChanged");
            WinformBinding.SetPropertyBinding(this.radioButton2, i => i.Checked, DataWarehouse.Instance, o => o.ControlViewModel1.IsRadioButton2Selected)
                          .SetMode(BindMode.TwoWay)
                          .AttachTargetEvent("CheckedChanged");

            // TextBox
            WinformBinding.SetPropertyBinding(this.textBox1, i => i.Text, DataWarehouse.Instance, o => o.ControlViewModel1.TextBoxText)
                          .SetMode(BindMode.TwoWay)
                          .AttachTargetEvent("TextChanged");

            // TrackBar
            WinformBinding.SetPropertyBinding(this.trackBar1, i => i.Minimum, DataWarehouse.Instance, o => o.ControlViewModel1.SliderMinValue);
            WinformBinding.SetPropertyBinding(this.trackBar1, i => i.Maximum, DataWarehouse.Instance, o => o.ControlViewModel1.SliderMaxValue);

            WinformBinding.SetPropertyBinding(
                this.trackBar1,
                i => i.Value,
                DataWarehouse.Instance,
                o => o.ControlViewModel1.SliderValue)
                          .SetMode(BindMode.TwoWay)
                          .AttachTargetEvent("ValueChanged");

            // TabControl
            WinformBinding.SetCollectionBinding(this.tabControl1, i => i.TabPages, DataWarehouse.Instance, o => o.ControlViewModel1.Persons, false)
                          .SetTargetCollectionHandler(new TabControlCollectionHanlder())
                          .Activate();
            WinformBinding.SetPropertyBinding(this.tabControl1, i => i.SelectedIndex, DataWarehouse.Instance, o => o.ControlViewModel1.TabControlSelectedIndex)
                          .SetMode(BindMode.TwoWay)
                          .AttachTargetEvent("SelectedIndexChanged");

            // Button
            WinformBinding.SetCommandBinding(this.addBtn1, null, DataWarehouse.Instance, i => i.ControlViewModel1.AddCommand)
                          .AddEnableProperty<Button>(button => button.Enabled)
                          .AttachTargetEvent("Click");

            WinformBinding.SetCommandBinding(this.removeBtn1, null, DataWarehouse.Instance, i => i.ControlViewModel1.RemoveCommand)
                          .AddEnableProperty<Button>(button => button.Enabled)
                          .Watch<ControlViewModel2>(i => i.CurrentPerson)
                          .AttachTargetEvent("Click");

            // ScrollBar
            WinformBinding.SetPropertyBinding(this.hScrollBar1, i => i.Minimum, DataWarehouse.Instance, o => o.ControlViewModel1.SliderMinValue);
            WinformBinding.SetPropertyBinding(this.hScrollBar1, i => i.Maximum, DataWarehouse.Instance, o => o.ControlViewModel1.ScrollBarMaxValue);
            WinformBinding.SetPropertyBinding(this.hScrollBar1, i => i.Value, DataWarehouse.Instance, o => o.ControlViewModel1.ScrollBarValue)
                          .SetMode(BindMode.TwoWay)
                          .AttachTargetEvent("ValueChanged");

        }

        private void SetWinformBinding2()
        {
            WinformBinding.SetPropertyBinding(this.keyTextBox, i => i.Text, DataWarehouse.Instance, o => o.ControlViewModel2.Key)
                          .SetMode(BindMode.TwoWay)
                          .AttachTargetEvent("TextChanged");

            WinformBinding.SetPropertyBinding(this.nameTextbox, i => i.Text, DataWarehouse.Instance, o => o.ControlViewModel2.CurrentPerson.Name)
                          .SetMode(BindMode.TwoWay)
                          .AttachTargetEvent("TextChanged");

            WinformBinding.SetPropertyBinding(this.personListBox, o => o.SelectedItem, DataWarehouse.Instance, i => i.ControlViewModel2.CurrentPerson)
                          .SetMode(BindMode.TwoWay)
                          .AttachTargetEvent("SelectedValueChanged");

            WinformBinding.SetPropertyBinding(this.countText, i => i.Text, DataWarehouse.Instance, o => o.ControlViewModel2.Persons.Count);

            WinformBinding.SetCollectionBinding(this.personListBox, i => i.Items, DataWarehouse.Instance, o => o.ControlViewModel2.Persons)
                          .TargetHandler = new ListBoxCollectionHanlder();

            WinformBinding.SetCommandBinding(this.addBtn, null, DataWarehouse.Instance, i => i.ControlViewModel2.AddCommand)
                          .AddEnableProperty<Button>(button => button.Enabled)
                          .AttachTargetEvent("Click");

            WinformBinding.SetCommandBinding(this.removeBtn, null, DataWarehouse.Instance, i => i.ControlViewModel2.RemoveCommand)
                          .AddEnableProperty<Button>(button => button.Enabled)
                          .Watch<ControlViewModel2>(i => i.CurrentPerson)
                          .AttachTargetEvent("Click");

            WinformBinding.SetCommandBinding(this.clearBtn, null, DataWarehouse.Instance, i => i.ControlViewModel2.ClearCommand)
                          .AddEnableProperty<Button>(button => button.Enabled)
                          .AttachTargetEvent("Click");
        }

        private void SetWinformBinding3()
        {
            WinformBinding.SetCollectionBinding(this.treeView1, i => i.Nodes, DataWarehouse.Instance, o => o.ControlViewModel3.Groups, false)
                          .SetTargetDataGenerator(this.SetTreeViewGroupBinding)
                          .Activate();

            WinformBinding.SetPropertyBinding(this.treeView1, o => o.SelectedNode, DataWarehouse.Instance, i => i.ControlViewModel3.SelectedItem, false)
                          .SetMode(BindMode.OneWayToSource)
                          .AttachTargetEvent("AfterSelect")
                          .Activate();

            WinformBinding.SetPropertyBinding(this.groupTbx, i => i.Text, DataWarehouse.Instance, o => o.ControlViewModel3.CurrentGroup.Name)
                          .SetMode(BindMode.TwoWay)
                          .AttachTargetEvent("TextChanged");

            WinformBinding.SetPropertyBinding(this.personTbx, i => i.Text, DataWarehouse.Instance, o => o.ControlViewModel3.CurrentPerson.Name)
                          .SetMode(BindMode.TwoWay)
                          .AttachTargetEvent("TextChanged");


            WinformBinding.SetCommandBinding(this.addGroupBtn, null, DataWarehouse.Instance, i => i.ControlViewModel3.AddGroupCommand)
                          .AddEnableProperty<Button>(button => button.Enabled)
                          .AttachTargetEvent("Click");

            WinformBinding.SetCommandBinding(this.removeGroupBtn, null, DataWarehouse.Instance, i => i.ControlViewModel3.RemoveGroupCommand)
                          .AddEnableProperty<Button>(button => button.Enabled)
                          .Watch<ControlViewModel3>(i => i.CurrentGroup)
                          .AttachTargetEvent("Click");

            WinformBinding.SetCommandBinding(this.addPersonBtn, null, DataWarehouse.Instance, i => i.ControlViewModel3.AddPersonCommand)
                          .AddEnableProperty<Button>(button => button.Enabled)
                          .Watch<ControlViewModel3>(i => i.CurrentGroup)
                          .AttachTargetEvent("Click");

            WinformBinding.SetCommandBinding(this.removePersonBtn, null, DataWarehouse.Instance, i => i.ControlViewModel3.RemovePersonCommand)
                          .AddEnableProperty<Button>(button => button.Enabled)
                          .Watch<ControlViewModel3>(i => i.CurrentPerson)
                          .AttachTargetEvent("Click");

        }

        private void SetWinformBinding4()
        {
            // DataGridView
            WinformBinding.SetCollectionBinding(this.dataGrid, i => i.Rows, DataWarehouse.Instance, o => o.ControlViewModel4.Persons, false)
                          .SetTargetCollectionHandler(new DataGridViewCollectionHandler())
                          .Activate();

            // Button
            WinformBinding.SetCommandBinding(this.addRowBtn, null, DataWarehouse.Instance, i => i.ControlViewModel4.AddPersonCommand)
                          .AddEnableProperty<Button>(button => button.Enabled)
                          .AttachTargetEvent("Click");

            WinformBinding.SetCommandBinding(this.removeRowBtn, null, DataWarehouse.Instance, i => i.ControlViewModel4.RemovePersonCommand)
                          .AddEnableProperty<Button>(button => button.Enabled)
                          .Watch<ControlViewModel4>(i => i.CurrentPerson)
                          .AttachTargetEvent("Click");
        }

        private class DataGridViewCollectionHandler : ICollectionHandler
        {
            public bool AddItem(int index, object item, object source, object sourceProperty)
            {
                var person = (Person)item;
                var datagrid = (DataGridView)source;
                DataGridViewRow row = datagrid.Rows[datagrid.Rows.Add()];

                WinformBinding.SetPropertyBinding(row, i => i.Cells["NameColumn"].Value, person, j => j.Name)
                              .SetMode(BindMode.TwoWay)
                              .AttachTargetEvent(row.DataGridView, "CellValueChanged",
                                  (sender, args) =>
                                  {
                                      var e = (DataGridViewCellEventArgs)args;
                                      return e.ColumnIndex == 0;
                                  });

                WinformBinding.SetPropertyBinding(row, i => i.Cells["AgeColumn"].Value, person, j => j.Age)
                              .SetMode(BindMode.TwoWay)
                              .AttachTargetEvent(row.DataGridView, "CellValueChanged",
                                  (sender, args) =>
                                  {
                                      var e = (DataGridViewCellEventArgs)args;
                                      return e.ColumnIndex == 1;
                                  });
                              
                WinformBinding.SetPropertyBinding(row, i => i.Cells["HashCodeColumn"].Value, person, null)
                              .SetSourcePropertyGetter(data => data.Source.Source.GetHashCode());

                return true;
            }

            public bool Clear(object source, object sourceProperty)
            {
                var datagrid = (DataGridView)source;
                datagrid.Rows.Clear();
                return true;
            }

            public bool RemoveItem(int index, object item, object source, object sourceProperty)
            {
                var datagrid = (DataGridView)source;
                datagrid.Rows.RemoveAt(index);
                return true;
            }
        }

        private object SetTreeViewGroupBinding(object o, object o1)
        {
            var node = new TreeNode();
            var group = (Group)o;

            WinformBinding.SetPropertyBinding(node, i => i.Text, group, j => j.Name)
                          .SetMode(BindMode.TwoWay)
                          .AttachTargetEvent("TextChanged");

            WinformBinding.SetCollectionBinding(node, i => i.Nodes, group, j => j.Persons)
                          .SetTargetDataGenerator(this.SetTreeviewItemBinding);

            return node;
        }

        private object SetTreeviewItemBinding(object o, object o1)
        {
            var node = new TreeNode();
            var person = (Person)o;

            WinformBinding.SetPropertyBinding(node, i => i.Text, person, j => j.Name)
                          .SetMode(BindMode.TwoWay)
                          .AttachTargetEvent("TextChanged");

            return node;
        }

        private void ClearWinformBinding()
        {
            WinformBinding.ClearAllBindings();
        }

        private void ClearWpfBinding()
        {
            ((FrameworkElement)this.WPFTabControl.Items[0]).ClearValue(FrameworkElement.DataContextProperty);
            ((FrameworkElement)this.WPFTabControl.Items[1]).ClearValue(FrameworkElement.DataContextProperty);
            ((FrameworkElement)this.WPFTabControl.Items[2]).ClearValue(FrameworkElement.DataContextProperty);
            ((FrameworkElement)this.WPFTabControl.Items[3]).ClearValue(FrameworkElement.DataContextProperty);
        }

        private void UpdateMainViewModel()
        {
            DataWarehouse.Instance.ControlViewModel1 = new ControlViewModel1();
            DataWarehouse.Instance.ControlViewModel2 = new ControlViewModel2();
        }

        private void ChangeViewModel_Click(object sender, EventArgs e)
        {
            this.UpdateMainViewModel();
        }

        private void CollectBtn_Click(object sender, EventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        #endregion

        #region Private Classes

        private class ListBoxCollectionHanlder : ICollectionHandler
        {
            #region Public Methods and Operators

            public bool AddItem(int index, object item, object source, object sourceProperty)
            {
                MainForm.WinformBinding.SetNotifyBinding(source, null, item, o => (o as Person).Name).SetSourceChanged(
                        (o, args) =>
                        ((RefreshListBox)args.Data.Target.Source).RefreshListBoxItem(args.Data.Source.Source));

                // Return false to let the default collection work, ListBox will add the Person as the ListBoxItem.
                return false;
            }

            public bool Clear(object source, object sourceProperty)
            {
                var listbox = source as ListBox;
                if (listbox != null)
                {
                    listbox.ClearSelected();
                }

                return false;
            }

            public bool RemoveItem(int index, object item, object source, object sourceProperty)
            {
                return false;
            }

            #endregion
        }

        private class TabControlCollectionHanlder : ICollectionHandler
        {
            #region Public Methods and Operators

            public bool AddItem(int index, object item, object source, object sourceProperty)
            {
                var tabItem = new TabPage();
                var textbox = new System.Windows.Forms.TextBox();
                MainForm.WinformBinding.SetPropertyBinding(textbox, t => t.Text, item, o => (o as Person).Name)
                             .SetMode(BindMode.TwoWay)
                             .AttachTargetEvent("TextChanged");
                tabItem.Controls.Add(textbox);
                MainForm.WinformBinding.SetPropertyBinding(tabItem, t => t.Text, item, o => (o as Person).Name);

                ((System.Windows.Forms.TabControl)source).TabPages.Add(tabItem);

                // Return true to indicate already create the default item.
                return true;
            }

            public bool RemoveItem(int index, object item, object source, object sourceProperty)
            {
                return false;
            }

            public bool Clear(object source, object sourceProperty)
            {
                return false;
            }

            #endregion
        }

        #endregion
    }

    public class RefreshListBox : ListBox
    {
        #region Public Methods and Operators

        public void RefreshListBoxItem(object item)
        {
            int index = this.Items.IndexOf(item);
            if (index != -1)
            {
                this.RefreshItem(index);
            }
        }

        #endregion
    }
}