// Copyright 2015 by Serg V. Zhdanovskih

using System.Windows.Forms;
using BSLib.ArborGVT;

namespace ArborSample
{
    public partial class ArborSampleForm : Form
    {
        private readonly ToolTip fTip;
        private bool fTipShow;

        public ArborSampleForm()
        {
            InitializeComponent();

            fTip = new ToolTip();
            fTipShow = false;

            arborViewer1.EnergyDebug = true;
            arborViewer1.NodesDragging = true;

            ArborViewer.createSample(arborViewer1.Sys.Graph);
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
