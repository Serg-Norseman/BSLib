namespace BSLib.Samples
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private BSLib.Controls.RadioGroup radioGroup21;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button btnArborViewer;
        
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        
        private void InitializeComponent()
        {
            this.radioGroup21 = new BSLib.Controls.RadioGroup();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.btnArborViewer = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // radioGroup21
            // 
            this.radioGroup21.Columns = 2;
            this.radioGroup21.Items = new string[] {
        "1",
        "2",
        "3",
        "4",
        "5",
        "6",
        "7",
        "8",
        "9",
        "10",
        "11",
        "12",
        "13",
        "14",
        "15"};
            this.radioGroup21.Location = new System.Drawing.Point(13, 14);
            this.radioGroup21.Margin = new System.Windows.Forms.Padding(4);
            this.radioGroup21.Name = "radioGroup21";
            this.radioGroup21.Padding = new System.Windows.Forms.Padding(4);
            this.radioGroup21.SelectedIndex = 0;
            this.radioGroup21.Size = new System.Drawing.Size(363, 251);
            this.radioGroup21.TabIndex = 15;
            this.radioGroup21.TabStop = false;
            this.radioGroup21.Text = "radioGroup21";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(651, 113);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 24);
            this.comboBox1.TabIndex = 16;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1SelectedIndexChanged);
            // 
            // btnArborViewer
            // 
            this.btnArborViewer.Location = new System.Drawing.Point(918, 31);
            this.btnArborViewer.Name = "btnArborViewer";
            this.btnArborViewer.Size = new System.Drawing.Size(142, 31);
            this.btnArborViewer.TabIndex = 17;
            this.btnArborViewer.Text = "ArborViewer";
            this.btnArborViewer.UseVisualStyleBackColor = true;
            this.btnArborViewer.Click += new System.EventHandler(this.btnArborViewer_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1141, 425);
            this.Controls.Add(this.btnArborViewer);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.radioGroup21);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "Test";
            this.Load += new System.EventHandler(this.MainFormLoad);
            this.ResumeLayout(false);

        }
    }
}
