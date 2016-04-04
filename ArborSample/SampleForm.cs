using System.Windows.Forms;
using ArborGVT;

namespace ArborSample
{
    public partial class SampleForm : Form
    {
        private readonly ToolTip fTip;
        private bool fTipShow;

        public SampleForm()
        {
            InitializeComponent();

            this.fTip = new ToolTip();
            this.fTipShow = false;

            arborViewer1.EnergyDebug = true;
            arborViewer1.NodesDragging = true;
            arborViewer1.doSample();
            arborViewer1.start();
        }

        private void ArborViewer1MouseMove(object sender, MouseEventArgs e)
        {
            ArborNode resNode = arborViewer1.getNodeByCoord(e.X, e.Y);

            if (resNode == null) {
                if (fTipShow) {
                    fTip.Hide(arborViewer1);
                    fTipShow = false;
                }
            } else {
                if (!fTipShow) {
                    fTip.Show(resNode.Sign, arborViewer1, e.X + 24, e.Y);
                    fTipShow = true;
                }
            }
        }
    }
}
