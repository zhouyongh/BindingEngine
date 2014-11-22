namespace Illusion.Sample
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.nameTextbox = new System.Windows.Forms.TextBox();
            this.clearBtn = new System.Windows.Forms.Button();
            this.countText = new System.Windows.Forms.Label();
            this.removeBtn = new System.Windows.Forms.Button();
            this.addBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.personListBox = new Illusion.Sample.RefreshListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.winformLabel = new System.Windows.Forms.Label();
            this.gcCollectbtn = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.keyTextBox = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.winformTabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.removeBtn1 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.addBtn1 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.personTbx = new System.Windows.Forms.TextBox();
            this.groupTbx = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.removeGroupBtn = new System.Windows.Forms.Button();
            this.removePersonBtn = new System.Windows.Forms.Button();
            this.addPersonBtn = new System.Windows.Forms.Button();
            this.addGroupBtn = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.removeRowBtn = new System.Windows.Forms.Button();
            this.addRowBtn = new System.Windows.Forms.Button();
            this.dataGrid = new System.Windows.Forms.DataGridView();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.elementHost2 = new System.Windows.Forms.Integration.ElementHost();
            this.wpfControl21 = new Illusion.Sample.WPFControl();
            this.winformBindingCbx = new System.Windows.Forms.CheckBox();
            this.wpfBindingCbx = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.wpfLabel = new System.Windows.Forms.Label();
            this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AgeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HashCodeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox3.SuspendLayout();
            this.winformTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // nameTextbox
            // 
            this.nameTextbox.Location = new System.Drawing.Point(88, 162);
            this.nameTextbox.Name = "nameTextbox";
            this.nameTextbox.Size = new System.Drawing.Size(100, 21);
            this.nameTextbox.TabIndex = 3;
            // 
            // clearBtn
            // 
            this.clearBtn.Location = new System.Drawing.Point(170, 227);
            this.clearBtn.Name = "clearBtn";
            this.clearBtn.Size = new System.Drawing.Size(75, 21);
            this.clearBtn.TabIndex = 1;
            this.clearBtn.Text = "Clear";
            this.clearBtn.UseVisualStyleBackColor = true;
            // 
            // countText
            // 
            this.countText.AutoSize = true;
            this.countText.Location = new System.Drawing.Point(96, 189);
            this.countText.Name = "countText";
            this.countText.Size = new System.Drawing.Size(41, 12);
            this.countText.TabIndex = 2;
            this.countText.Text = "label2";
            // 
            // removeBtn
            // 
            this.removeBtn.Location = new System.Drawing.Point(89, 227);
            this.removeBtn.Name = "removeBtn";
            this.removeBtn.Size = new System.Drawing.Size(75, 21);
            this.removeBtn.TabIndex = 1;
            this.removeBtn.Text = "Remove";
            this.removeBtn.UseVisualStyleBackColor = true;
            // 
            // addBtn
            // 
            this.addBtn.Location = new System.Drawing.Point(8, 227);
            this.addBtn.Name = "addBtn";
            this.addBtn.Size = new System.Drawing.Size(75, 21);
            this.addBtn.TabIndex = 1;
            this.addBtn.Text = "Add";
            this.addBtn.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 189);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Count :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 162);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Current :";
            // 
            // personListBox
            // 
            this.personListBox.FormattingEnabled = true;
            this.personListBox.ItemHeight = 12;
            this.personListBox.Location = new System.Drawing.Point(20, 35);
            this.personListBox.Name = "personListBox";
            this.personListBox.Size = new System.Drawing.Size(204, 100);
            this.personListBox.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(44, 456);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(113, 22);
            this.button1.TabIndex = 1;
            this.button1.Text = "Change ViewModel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.ChangeViewModel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(42, 428);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(125, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "ViewModel hashcode :";
            // 
            // winformLabel
            // 
            this.winformLabel.AutoSize = true;
            this.winformLabel.Location = new System.Drawing.Point(176, 428);
            this.winformLabel.Name = "winformLabel";
            this.winformLabel.Size = new System.Drawing.Size(0, 12);
            this.winformLabel.TabIndex = 3;
            // 
            // gcCollectbtn
            // 
            this.gcCollectbtn.Location = new System.Drawing.Point(163, 456);
            this.gcCollectbtn.Name = "gcCollectbtn";
            this.gcCollectbtn.Size = new System.Drawing.Size(90, 22);
            this.gcCollectbtn.TabIndex = 5;
            this.gcCollectbtn.Text = "GC Collect";
            this.gcCollectbtn.UseVisualStyleBackColor = true;
            this.gcCollectbtn.Click += new System.EventHandler(this.CollectBtn_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(280, 456);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(257, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "Current Key (Nullabe binding, default 2) :";
            // 
            // keyTextBox
            // 
            this.keyTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.keyTextBox.Location = new System.Drawing.Point(547, 452);
            this.keyTextBox.Name = "keyTextBox";
            this.keyTextBox.Size = new System.Drawing.Size(48, 20);
            this.keyTextBox.TabIndex = 3;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.winformTabControl);
            this.groupBox3.Location = new System.Drawing.Point(44, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(296, 378);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "WinForm Controls";
            // 
            // winformTabControl
            // 
            this.winformTabControl.Controls.Add(this.tabPage1);
            this.winformTabControl.Controls.Add(this.tabPage2);
            this.winformTabControl.Controls.Add(this.tabPage3);
            this.winformTabControl.Controls.Add(this.tabPage4);
            this.winformTabControl.Location = new System.Drawing.Point(0, 14);
            this.winformTabControl.Name = "winformTabControl";
            this.winformTabControl.SelectedIndex = 0;
            this.winformTabControl.Size = new System.Drawing.Size(296, 364);
            this.winformTabControl.TabIndex = 10;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.White;
            this.tabPage1.Controls.Add(this.removeBtn1);
            this.tabPage1.Controls.Add(this.comboBox1);
            this.tabPage1.Controls.Add(this.addBtn1);
            this.tabPage1.Controls.Add(this.checkBox1);
            this.tabPage1.Controls.Add(this.hScrollBar1);
            this.tabPage1.Controls.Add(this.radioButton1);
            this.tabPage1.Controls.Add(this.tabControl1);
            this.tabPage1.Controls.Add(this.radioButton2);
            this.tabPage1.Controls.Add(this.trackBar1);
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(288, 338);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Common Controls";
            // 
            // removeBtn1
            // 
            this.removeBtn1.Location = new System.Drawing.Point(151, 276);
            this.removeBtn1.Name = "removeBtn1";
            this.removeBtn1.Size = new System.Drawing.Size(75, 23);
            this.removeBtn1.TabIndex = 9;
            this.removeBtn1.Text = "Remove";
            this.removeBtn1.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(30, 26);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 20);
            this.comboBox1.TabIndex = 2;
            // 
            // addBtn1
            // 
            this.addBtn1.Location = new System.Drawing.Point(30, 276);
            this.addBtn1.Name = "addBtn1";
            this.addBtn1.Size = new System.Drawing.Size(75, 23);
            this.addBtn1.TabIndex = 9;
            this.addBtn1.Text = "Add";
            this.addBtn1.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(30, 5);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(72, 16);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Text = "CheckBox";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.LargeChange = 1;
            this.hScrollBar1.Location = new System.Drawing.Point(26, 309);
            this.hScrollBar1.Maximum = 10;
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(200, 17);
            this.hScrollBar1.TabIndex = 8;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(30, 54);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(95, 16);
            this.radioButton1.TabIndex = 3;
            this.radioButton1.Text = "RadioButton1";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Location = new System.Drawing.Point(30, 166);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(200, 100);
            this.tabControl1.TabIndex = 7;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(30, 77);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(95, 16);
            this.radioButton2.TabIndex = 4;
            this.radioButton2.Text = "RadioButton2";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(30, 132);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(121, 45);
            this.trackBar1.TabIndex = 6;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(30, 103);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(121, 21);
            this.textBox1.TabIndex = 5;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.White;
            this.tabPage2.Controls.Add(this.nameTextbox);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.clearBtn);
            this.tabPage2.Controls.Add(this.personListBox);
            this.tabPage2.Controls.Add(this.countText);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.removeBtn);
            this.tabPage2.Controls.Add(this.addBtn);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(288, 338);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "ListBox";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.personTbx);
            this.tabPage3.Controls.Add(this.groupTbx);
            this.tabPage3.Controls.Add(this.label7);
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Controls.Add(this.removeGroupBtn);
            this.tabPage3.Controls.Add(this.removePersonBtn);
            this.tabPage3.Controls.Add(this.addPersonBtn);
            this.tabPage3.Controls.Add(this.addGroupBtn);
            this.tabPage3.Controls.Add(this.treeView1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(288, 338);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "TreeView";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // personTbx
            // 
            this.personTbx.Location = new System.Drawing.Point(118, 233);
            this.personTbx.Name = "personTbx";
            this.personTbx.Size = new System.Drawing.Size(100, 21);
            this.personTbx.TabIndex = 3;
            // 
            // groupTbx
            // 
            this.groupTbx.Location = new System.Drawing.Point(119, 205);
            this.groupTbx.Name = "groupTbx";
            this.groupTbx.Size = new System.Drawing.Size(100, 21);
            this.groupTbx.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 236);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(101, 12);
            this.label7.TabIndex = 2;
            this.label7.Text = "Current People :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 208);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "Current Group :";
            // 
            // removeGroupBtn
            // 
            this.removeGroupBtn.Location = new System.Drawing.Point(120, 261);
            this.removeGroupBtn.Name = "removeGroupBtn";
            this.removeGroupBtn.Size = new System.Drawing.Size(91, 28);
            this.removeGroupBtn.TabIndex = 1;
            this.removeGroupBtn.Text = "Remove Group";
            this.removeGroupBtn.UseVisualStyleBackColor = true;
            // 
            // removePersonBtn
            // 
            this.removePersonBtn.Location = new System.Drawing.Point(120, 293);
            this.removePersonBtn.Name = "removePersonBtn";
            this.removePersonBtn.Size = new System.Drawing.Size(91, 28);
            this.removePersonBtn.TabIndex = 1;
            this.removePersonBtn.Text = "Remove People";
            this.removePersonBtn.UseVisualStyleBackColor = true;
            // 
            // addPersonBtn
            // 
            this.addPersonBtn.Location = new System.Drawing.Point(13, 293);
            this.addPersonBtn.Name = "addPersonBtn";
            this.addPersonBtn.Size = new System.Drawing.Size(75, 28);
            this.addPersonBtn.TabIndex = 1;
            this.addPersonBtn.Text = "Add People";
            this.addPersonBtn.UseVisualStyleBackColor = true;
            // 
            // addGroupBtn
            // 
            this.addGroupBtn.Location = new System.Drawing.Point(13, 261);
            this.addGroupBtn.Name = "addGroupBtn";
            this.addGroupBtn.Size = new System.Drawing.Size(75, 28);
            this.addGroupBtn.TabIndex = 1;
            this.addGroupBtn.Text = "Add Group";
            this.addGroupBtn.UseVisualStyleBackColor = true;
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(6, 6);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(243, 187);
            this.treeView1.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.removeRowBtn);
            this.tabPage4.Controls.Add(this.addRowBtn);
            this.tabPage4.Controls.Add(this.dataGrid);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(288, 338);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "DataGrid";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // removeRowBtn
            // 
            this.removeRowBtn.Location = new System.Drawing.Point(155, 283);
            this.removeRowBtn.Name = "removeRowBtn";
            this.removeRowBtn.Size = new System.Drawing.Size(75, 23);
            this.removeRowBtn.TabIndex = 10;
            this.removeRowBtn.Text = "Remove";
            this.removeRowBtn.UseVisualStyleBackColor = true;
            // 
            // addRowBtn
            // 
            this.addRowBtn.Location = new System.Drawing.Point(34, 283);
            this.addRowBtn.Name = "addRowBtn";
            this.addRowBtn.Size = new System.Drawing.Size(75, 23);
            this.addRowBtn.TabIndex = 11;
            this.addRowBtn.Text = "Add";
            this.addRowBtn.UseVisualStyleBackColor = true;
            // 
            // dataGrid
            // 
            this.dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameColumn,
            this.AgeColumn,
            this.HashCodeColumn});
            this.dataGrid.Location = new System.Drawing.Point(0, 0);
            this.dataGrid.Name = "dataGrid";
            this.dataGrid.RowTemplate.Height = 23;
            this.dataGrid.Size = new System.Drawing.Size(288, 248);
            this.dataGrid.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.elementHost2);
            this.groupBox4.Location = new System.Drawing.Point(380, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(290, 378);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "WPF Controls";
            // 
            // elementHost2
            // 
            this.elementHost2.Location = new System.Drawing.Point(0, 14);
            this.elementHost2.Name = "elementHost2";
            this.elementHost2.Size = new System.Drawing.Size(290, 364);
            this.elementHost2.TabIndex = 1;
            this.elementHost2.Text = "elementHost2";
            this.elementHost2.Child = this.wpfControl21;
            // 
            // winformBindingCbx
            // 
            this.winformBindingCbx.AutoSize = true;
            this.winformBindingCbx.Checked = true;
            this.winformBindingCbx.CheckState = System.Windows.Forms.CheckState.Checked;
            this.winformBindingCbx.Location = new System.Drawing.Point(44, 406);
            this.winformBindingCbx.Name = "winformBindingCbx";
            this.winformBindingCbx.Size = new System.Drawing.Size(84, 16);
            this.winformBindingCbx.TabIndex = 1;
            this.winformBindingCbx.Text = "SetBinding";
            this.winformBindingCbx.UseVisualStyleBackColor = true;
            // 
            // wpfBindingCbx
            // 
            this.wpfBindingCbx.AutoSize = true;
            this.wpfBindingCbx.Checked = true;
            this.wpfBindingCbx.CheckState = System.Windows.Forms.CheckState.Checked;
            this.wpfBindingCbx.Location = new System.Drawing.Point(380, 404);
            this.wpfBindingCbx.Name = "wpfBindingCbx";
            this.wpfBindingCbx.Size = new System.Drawing.Size(84, 16);
            this.wpfBindingCbx.TabIndex = 1;
            this.wpfBindingCbx.Text = "SetBinding";
            this.wpfBindingCbx.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(378, 425);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(125, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "ViewModel hashcode :";
            // 
            // wpfLabel
            // 
            this.wpfLabel.AutoSize = true;
            this.wpfLabel.Location = new System.Drawing.Point(516, 425);
            this.wpfLabel.Name = "wpfLabel";
            this.wpfLabel.Size = new System.Drawing.Size(0, 12);
            this.wpfLabel.TabIndex = 3;
            // 
            // NameColumn
            // 
            this.NameColumn.HeaderText = "Name";
            this.NameColumn.Name = "NameColumn";
            // 
            // AgeColumn
            // 
            this.AgeColumn.HeaderText = "Age";
            this.AgeColumn.Name = "AgeColumn";
            this.AgeColumn.Width = 60;
            // 
            // HashCodeColumn
            // 
            this.HashCodeColumn.HeaderText = "HashCode";
            this.HashCodeColumn.Name = "HashCodeColumn";
            this.HashCodeColumn.ReadOnly = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(693, 494);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.wpfBindingCbx);
            this.Controls.Add(this.winformBindingCbx);
            this.Controls.Add(this.gcCollectbtn);
            this.Controls.Add(this.keyTextBox);
            this.Controls.Add(this.wpfLabel);
            this.Controls.Add(this.winformLabel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button1);
            this.Name = "MainForm";
            this.Text = "Binding Sample";
            this.groupBox3.ResumeLayout(false);
            this.winformTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private RefreshListBox personListBox;
        private System.Windows.Forms.Button addBtn;
        private System.Windows.Forms.Button removeBtn;
        private System.Windows.Forms.Button clearBtn;
        private System.Windows.Forms.Label countText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox nameTextbox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label winformLabel;
        private System.Windows.Forms.Button gcCollectbtn;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox keyTextBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Integration.ElementHost elementHost2;
        private WPFControl wpfControl21;
        private System.Windows.Forms.Button removeBtn1;
        private System.Windows.Forms.Button addBtn1;
        private System.Windows.Forms.TabControl winformTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox winformBindingCbx;
        private System.Windows.Forms.CheckBox wpfBindingCbx;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label wpfLabel;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button removeGroupBtn;
        private System.Windows.Forms.Button removePersonBtn;
        private System.Windows.Forms.Button addPersonBtn;
        private System.Windows.Forms.Button addGroupBtn;
        private System.Windows.Forms.TextBox personTbx;
        private System.Windows.Forms.TextBox groupTbx;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.DataGridView dataGrid;
        private System.Windows.Forms.Button removeRowBtn;
        private System.Windows.Forms.Button addRowBtn;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn AgeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn HashCodeColumn;
    }
}