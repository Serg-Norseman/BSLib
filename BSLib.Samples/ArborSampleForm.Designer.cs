namespace BSLib.Samples
{
    partial class ArborSampleForm
    {
        private System.ComponentModel.IContainer components = null;
        private BSLib.DataViz.ArborGVT.ArborViewer arborViewer1;

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
            this.arborViewer1 = new BSLib.DataViz.ArborGVT.ArborViewer();
            this.SuspendLayout();
            // 
            // arborViewer1
            // 
            this.arborViewer1.BackColor = System.Drawing.Color.White;
            this.arborViewer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.arborViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.arborViewer1.Location = new System.Drawing.Point(0, 0);
            this.arborViewer1.Name = "arborViewer1";
            this.arborViewer1.Size = new System.Drawing.Size(894, 587);
            this.arborViewer1.TabIndex = 0;
            this.arborViewer1.TabStop = true;
            this.arborViewer1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ArborViewer1MouseMove);
            // 
            // ArborSampleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(894, 587);
            this.Controls.Add(this.arborViewer1);
            this.KeyPreview = true;
            this.Name = "ArborSampleForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SampleForm";
            this.ResumeLayout(false);
        }
    }
}
