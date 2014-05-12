namespace BindingSample
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.nameTextbox = new System.Windows.Forms.TextBox();
            this.clearBtn = new System.Windows.Forms.Button();
            this.countText = new System.Windows.Forms.Label();
            this.removeBtn = new System.Windows.Forms.Button();
            this.addBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.personListBox = new BindingSample.RefreshListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.wpfHost = new System.Windows.Forms.Integration.ElementHost();
            this.wpfControl1 = new BindingSample.WPFControl();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.viewModelLabel = new System.Windows.Forms.Label();
            this.setBindingBtn = new System.Windows.Forms.Button();
            this.clearBindingbtn = new System.Windows.Forms.Button();
            this.gcCollectbtn = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.bindStatusLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.keyTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.nameTextbox);
            this.groupBox1.Controls.Add(this.clearBtn);
            this.groupBox1.Controls.Add(this.countText);
            this.groupBox1.Controls.Add(this.removeBtn);
            this.groupBox1.Controls.Add(this.addBtn);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.personListBox);
            this.groupBox1.Location = new System.Drawing.Point(36, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(250, 254);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "WinFrom";
            // 
            // nameTextbox
            // 
            this.nameTextbox.Location = new System.Drawing.Point(87, 145);
            this.nameTextbox.Name = "nameTextbox";
            this.nameTextbox.Size = new System.Drawing.Size(100, 21);
            this.nameTextbox.TabIndex = 3;
            // 
            // clearBtn
            // 
            this.clearBtn.Location = new System.Drawing.Point(169, 210);
            this.clearBtn.Name = "clearBtn";
            this.clearBtn.Size = new System.Drawing.Size(75, 21);
            this.clearBtn.TabIndex = 1;
            this.clearBtn.Text = "Clear";
            this.clearBtn.UseVisualStyleBackColor = true;
            // 
            // countText
            // 
            this.countText.AutoSize = true;
            this.countText.Location = new System.Drawing.Point(95, 172);
            this.countText.Name = "countText";
            this.countText.Size = new System.Drawing.Size(41, 12);
            this.countText.TabIndex = 2;
            this.countText.Text = "label2";
            // 
            // removeBtn
            // 
            this.removeBtn.Location = new System.Drawing.Point(88, 210);
            this.removeBtn.Name = "removeBtn";
            this.removeBtn.Size = new System.Drawing.Size(75, 21);
            this.removeBtn.TabIndex = 1;
            this.removeBtn.Text = "Remove";
            this.removeBtn.UseVisualStyleBackColor = true;
            // 
            // addBtn
            // 
            this.addBtn.Location = new System.Drawing.Point(7, 210);
            this.addBtn.Name = "addBtn";
            this.addBtn.Size = new System.Drawing.Size(75, 21);
            this.addBtn.TabIndex = 1;
            this.addBtn.Text = "Add";
            this.addBtn.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 172);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Count :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 145);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Current :";
            // 
            // personListBox
            // 
            this.personListBox.FormattingEnabled = true;
            this.personListBox.ItemHeight = 12;
            this.personListBox.Location = new System.Drawing.Point(19, 18);
            this.personListBox.Name = "personListBox";
            this.personListBox.Size = new System.Drawing.Size(204, 100);
            this.personListBox.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.wpfHost);
            this.groupBox2.Location = new System.Drawing.Point(326, 19);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(250, 254);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "WPF";
            // 
            // wpfHost
            // 
            this.wpfHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wpfHost.Location = new System.Drawing.Point(3, 17);
            this.wpfHost.Name = "wpfHost";
            this.wpfHost.Size = new System.Drawing.Size(244, 234);
            this.wpfHost.TabIndex = 0;
            this.wpfHost.Child = this.wpfControl1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(207, 302);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(113, 21);
            this.button1.TabIndex = 1;
            this.button1.Text = "Change ViewModel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.changeViewModel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(40, 358);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(125, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "ViewModel hashcode :";
            // 
            // viewModelLabel
            // 
            this.viewModelLabel.AutoSize = true;
            this.viewModelLabel.Location = new System.Drawing.Point(166, 358);
            this.viewModelLabel.Name = "viewModelLabel";
            this.viewModelLabel.Size = new System.Drawing.Size(0, 12);
            this.viewModelLabel.TabIndex = 3;
            // 
            // setBindingBtn
            // 
            this.setBindingBtn.Location = new System.Drawing.Point(36, 302);
            this.setBindingBtn.Name = "setBindingBtn";
            this.setBindingBtn.Size = new System.Drawing.Size(82, 21);
            this.setBindingBtn.TabIndex = 4;
            this.setBindingBtn.Text = "Set Binding";
            this.setBindingBtn.UseVisualStyleBackColor = true;
            this.setBindingBtn.Click += new System.EventHandler(this.setBindingBtn_Click);
            // 
            // clearBindingbtn
            // 
            this.clearBindingbtn.Location = new System.Drawing.Point(124, 302);
            this.clearBindingbtn.Name = "clearBindingbtn";
            this.clearBindingbtn.Size = new System.Drawing.Size(77, 21);
            this.clearBindingbtn.TabIndex = 4;
            this.clearBindingbtn.Text = "Clear Binding";
            this.clearBindingbtn.UseVisualStyleBackColor = true;
            this.clearBindingbtn.Click += new System.EventHandler(this.clearBindingBtn_Click);
            // 
            // gcCollectbtn
            // 
            this.gcCollectbtn.Location = new System.Drawing.Point(329, 302);
            this.gcCollectbtn.Name = "gcCollectbtn";
            this.gcCollectbtn.Size = new System.Drawing.Size(90, 21);
            this.gcCollectbtn.TabIndex = 5;
            this.gcCollectbtn.Text = "GC Collect";
            this.gcCollectbtn.UseVisualStyleBackColor = true;
            this.gcCollectbtn.Click += new System.EventHandler(this.gcCollectbtn_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(88, 335);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "Bind Status :";
            // 
            // bindStatusLabel
            // 
            this.bindStatusLabel.AutoSize = true;
            this.bindStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.bindStatusLabel.Location = new System.Drawing.Point(166, 335);
            this.bindStatusLabel.Name = "bindStatusLabel";
            this.bindStatusLabel.Size = new System.Drawing.Size(0, 13);
            this.bindStatusLabel.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(266, 335);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(257, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "Current Key (Nullabe binding, default 2) :";
            // 
            // keyTextBox
            // 
            this.keyTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.keyTextBox.Location = new System.Drawing.Point(529, 332);
            this.keyTextBox.Name = "keyTextBox";
            this.keyTextBox.Size = new System.Drawing.Size(48, 20);
            this.keyTextBox.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 393);
            this.Controls.Add(this.clearBindingbtn);
            this.Controls.Add(this.gcCollectbtn);
            this.Controls.Add(this.setBindingBtn);
            this.Controls.Add(this.keyTextBox);
            this.Controls.Add(this.bindStatusLabel);
            this.Controls.Add(this.viewModelLabel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "MainForm";
            this.Text = "Binding Sample";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private RefreshListBox personListBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button addBtn;
        private System.Windows.Forms.Button removeBtn;
        private System.Windows.Forms.Button clearBtn;
        private System.Windows.Forms.Label countText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Integration.ElementHost wpfHost;
        private WPFControl wpfControl1;
        private System.Windows.Forms.TextBox nameTextbox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label viewModelLabel;
        private System.Windows.Forms.Button setBindingBtn;
        private System.Windows.Forms.Button clearBindingbtn;
        private System.Windows.Forms.Button gcCollectbtn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label bindStatusLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox keyTextBox;
    }
}