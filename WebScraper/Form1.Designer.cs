namespace WebScraper
{
    partial class WebScraper
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.scrapButton = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.mainTable = new System.Windows.Forms.DataGridView();
            this.allDatabasesTabControl = new System.Windows.Forms.TabControl();
            this.reloadButton = new System.Windows.Forms.Button();
            this.categoryCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.scrapTextBox = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.mainTable)).BeginInit();
            this.allDatabasesTabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // scrapButton
            // 
            this.scrapButton.Location = new System.Drawing.Point(137, 613);
            this.scrapButton.Name = "scrapButton";
            this.scrapButton.Size = new System.Drawing.Size(75, 23);
            this.scrapButton.TabIndex = 0;
            this.scrapButton.Text = "Scrap";
            this.scrapButton.UseVisualStyleBackColor = true;
            this.scrapButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(934, 0);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // mainTable
            // 
            this.mainTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mainTable.Location = new System.Drawing.Point(31, 92);
            this.mainTable.Name = "mainTable";
            this.mainTable.RowHeadersVisible = false;
            this.mainTable.Size = new System.Drawing.Size(781, 500);
            this.mainTable.TabIndex = 1;
            // 
            // allDatabasesTabControl
            // 
            this.allDatabasesTabControl.Controls.Add(this.tabPage1);
            this.allDatabasesTabControl.Location = new System.Drawing.Point(1, 2);
            this.allDatabasesTabControl.Name = "allDatabasesTabControl";
            this.allDatabasesTabControl.SelectedIndex = 0;
            this.allDatabasesTabControl.Size = new System.Drawing.Size(942, 20);
            this.allDatabasesTabControl.TabIndex = 2;
            this.allDatabasesTabControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.allDatabasesTabControl_MouseUp);
            // 
            // reloadButton
            // 
            this.reloadButton.BackgroundImage = global::WebScraper.Properties.Resources.ReloadIcon1;
            this.reloadButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.reloadButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.reloadButton.Location = new System.Drawing.Point(778, 52);
            this.reloadButton.Name = "reloadButton";
            this.reloadButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.reloadButton.Size = new System.Drawing.Size(34, 34);
            this.reloadButton.TabIndex = 3;
            this.reloadButton.UseVisualStyleBackColor = true;
            this.reloadButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // categoryCheckedListBox
            // 
            this.categoryCheckedListBox.BackColor = System.Drawing.SystemColors.Control;
            this.categoryCheckedListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.categoryCheckedListBox.CheckOnClick = true;
            this.categoryCheckedListBox.FormattingEnabled = true;
            this.categoryCheckedListBox.Location = new System.Drawing.Point(827, 115);
            this.categoryCheckedListBox.Name = "categoryCheckedListBox";
            this.categoryCheckedListBox.Size = new System.Drawing.Size(227, 480);
            this.categoryCheckedListBox.TabIndex = 4;
            this.categoryCheckedListBox.SelectedIndexChanged += new System.EventHandler(this.categoryCheckedListBox_SelectedIndexChanged);
            // 
            // scrapTextBox
            // 
            this.scrapTextBox.Location = new System.Drawing.Point(31, 613);
            this.scrapTextBox.Name = "scrapTextBox";
            this.scrapTextBox.Size = new System.Drawing.Size(100, 20);
            this.scrapTextBox.TabIndex = 5;
            this.scrapTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.scrapTextBox_KeyPress);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(818, 92);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(37, 17);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "All";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.checkBox1_MouseUp);
            // 
            // WebScraper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1081, 674);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.scrapTextBox);
            this.Controls.Add(this.categoryCheckedListBox);
            this.Controls.Add(this.reloadButton);
            this.Controls.Add(this.mainTable);
            this.Controls.Add(this.allDatabasesTabControl);
            this.Controls.Add(this.scrapButton);
            this.Name = "WebScraper";
            this.Text = "WebScraper";
            ((System.ComponentModel.ISupportInitialize)(this.mainTable)).EndInit();
            this.allDatabasesTabControl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button scrapButton;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView mainTable;
        private System.Windows.Forms.TabControl allDatabasesTabControl;
        private System.Windows.Forms.Button reloadButton;
        private System.Windows.Forms.CheckedListBox categoryCheckedListBox;
        private System.Windows.Forms.TextBox scrapTextBox;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}

