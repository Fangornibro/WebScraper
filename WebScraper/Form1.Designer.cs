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
            this.mainDataGridView = new System.Windows.Forms.DataGridView();
            this.allDatabasesTabControl = new System.Windows.Forms.TabControl();
            this.brandCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button1 = new System.Windows.Forms.Button();
            this.reloadButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.categoryCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            ((System.ComponentModel.ISupportInitialize)(this.mainDataGridView)).BeginInit();
            this.allDatabasesTabControl.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // scrapButton
            // 
            this.scrapButton.Location = new System.Drawing.Point(31, 609);
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
            this.tabPage1.Size = new System.Drawing.Size(803, 0);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // mainDataGridView
            // 
            this.mainDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mainDataGridView.Location = new System.Drawing.Point(31, 92);
            this.mainDataGridView.Name = "mainDataGridView";
            this.mainDataGridView.RowHeadersVisible = false;
            this.mainDataGridView.Size = new System.Drawing.Size(781, 500);
            this.mainDataGridView.TabIndex = 1;
            // 
            // allDatabasesTabControl
            // 
            this.allDatabasesTabControl.Controls.Add(this.tabPage1);
            this.allDatabasesTabControl.Location = new System.Drawing.Point(1, 2);
            this.allDatabasesTabControl.Name = "allDatabasesTabControl";
            this.allDatabasesTabControl.SelectedIndex = 0;
            this.allDatabasesTabControl.Size = new System.Drawing.Size(811, 20);
            this.allDatabasesTabControl.TabIndex = 2;
            this.allDatabasesTabControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.allDatabasesTabControl_MouseUp);
            // 
            // brandCheckedListBox
            // 
            this.brandCheckedListBox.BackColor = System.Drawing.SystemColors.Control;
            this.brandCheckedListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.brandCheckedListBox.CheckOnClick = true;
            this.brandCheckedListBox.FormattingEnabled = true;
            this.brandCheckedListBox.Location = new System.Drawing.Point(12, 26);
            this.brandCheckedListBox.Name = "brandCheckedListBox";
            this.brandCheckedListBox.Size = new System.Drawing.Size(227, 480);
            this.brandCheckedListBox.TabIndex = 4;
            this.brandCheckedListBox.SelectedIndexChanged += new System.EventHandler(this.categoryCheckedListBox_SelectedIndexChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(3, 3);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(37, 17);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "All";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.checkBox1_MouseUp);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(123, 609);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(583, 23);
            this.progressBar1.TabIndex = 7;
            this.progressBar1.Visible = false;
            // 
            // button1
            // 
            this.button1.BackgroundImage = global::WebScraper.Properties.Resources.TrashCanIcon;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.button1.Location = new System.Drawing.Point(738, 52);
            this.button1.Name = "button1";
            this.button1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.button1.Size = new System.Drawing.Size(34, 34);
            this.button1.TabIndex = 8;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
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
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(123, 635);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(583, 23);
            this.label1.TabIndex = 9;
            this.label1.Text = "label1";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.Visible = false;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(3, 3);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(48, 17);
            this.checkBox2.TabIndex = 11;
            this.checkBox2.Text = "Grills";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.checkBox2_MouseUp);
            // 
            // categoryCheckedListBox
            // 
            this.categoryCheckedListBox.BackColor = System.Drawing.SystemColors.Control;
            this.categoryCheckedListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.categoryCheckedListBox.CheckOnClick = true;
            this.categoryCheckedListBox.FormattingEnabled = true;
            this.categoryCheckedListBox.Location = new System.Drawing.Point(12, 26);
            this.categoryCheckedListBox.Name = "categoryCheckedListBox";
            this.categoryCheckedListBox.Size = new System.Drawing.Size(227, 480);
            this.categoryCheckedListBox.TabIndex = 10;
            this.categoryCheckedListBox.SelectedIndexChanged += new System.EventHandler(this.categoryCheckedListBox_SelectedIndexChanged_1);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(818, 92);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(277, 500);
            this.tabControl1.TabIndex = 12;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.checkBox1);
            this.tabPage2.Controls.Add(this.brandCheckedListBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(269, 584);
            this.tabPage2.TabIndex = 0;
            this.tabPage2.Text = "Brand";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.checkBox2);
            this.tabPage3.Controls.Add(this.categoryCheckedListBox);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(269, 474);
            this.tabPage3.TabIndex = 1;
            this.tabPage3.Text = "Category";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // WebScraper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1132, 674);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.reloadButton);
            this.Controls.Add(this.mainDataGridView);
            this.Controls.Add(this.allDatabasesTabControl);
            this.Controls.Add(this.scrapButton);
            this.Name = "WebScraper";
            this.Text = "WebScraper";
            ((System.ComponentModel.ISupportInitialize)(this.mainDataGridView)).EndInit();
            this.allDatabasesTabControl.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button scrapButton;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView mainDataGridView;
        private System.Windows.Forms.TabControl allDatabasesTabControl;
        private System.Windows.Forms.Button reloadButton;
        private System.Windows.Forms.CheckedListBox brandCheckedListBox;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckedListBox categoryCheckedListBox;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
    }
}

