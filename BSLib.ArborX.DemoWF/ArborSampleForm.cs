// Copyright 2015 by Serg V. Zhdanovskih

using System.Windows.Forms;
using BSLib.ArborGVT;

namespace BSLib.ArborX.DemoWF
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

            ArborSystem.CreateSample(arborViewer1.Sys.Graph);
            arborViewer1.Start();
        }

        private void ArborViewer1MouseMove(object sender, MouseEventArgs e)
        {
            ArborNode resNode = arborViewer1.GetNodeByCoords(e.X, e.Y);

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
