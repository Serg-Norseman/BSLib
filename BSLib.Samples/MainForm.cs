using System;
using System.Windows.Forms;

namespace BSLib.Samples
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainFormLoad(object sender, EventArgs e)
        {
        }

        private void ComboBox1SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void btnArborViewer_Click(object sender, EventArgs e)
        {
            using (var form = new ArborSampleForm()) {
                form.ShowDialog();
            }
        }
    }
}
