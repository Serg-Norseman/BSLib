namespace Test
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private BSLib.Controls.RadioGroup radioGroup21;
        private System.Windows.Forms.ComboBox comboBox1;
        
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
            this.radioGroup21.Location = new System.Drawing.Point(10, 11);
            this.radioGroup21.Name = "radioGroup21";
            this.radioGroup21.SelectedIndex = 0;
            this.radioGroup21.Size = new System.Drawing.Size(272, 204);
            this.radioGroup21.TabIndex = 15;
            this.radioGroup21.TabStop = false;
            this.radioGroup21.Text = "radioGroup21";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(488, 92);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(92, 21);
            this.comboBox1.TabIndex = 16;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1SelectedIndexChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(856, 345);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.radioGroup21);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.Text = "Test";
            this.Load += new System.EventHandler(this.MainFormLoad);
            this.ResumeLayout(false);

        }
    }
}
