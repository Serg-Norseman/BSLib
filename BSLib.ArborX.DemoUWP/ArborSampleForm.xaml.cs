// Copyright 2015 by Serg V. Zhdanovskih

using BSLib.DataViz.ArborGVT;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace BSLib.ArborX.DemoUWP
{
    public sealed partial class ArborSampleForm : Page
    {
        private readonly ToolTip fTip;
        private bool fTipShow;

        public ArborSampleForm()
        {
            InitializeComponent();

            fTip = new ToolTip();
            fTip.Placement = PlacementMode.Right;
            fTipShow = false;

            arborViewer1.EnergyDebug = true;
            arborViewer1.NodesDragging = true;

            ArborSystem.CreateSample(arborViewer1.System.Graph);
            arborViewer1.System.Start();
        }

        private void arborViewer1_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Point pt = e.GetCurrentPoint(arborViewer1).Position;
            ArborNode resNode = arborViewer1.System.GetNearestNode((int)pt.X, (int)pt.Y);

            if (resNode == null) {
                if (fTipShow) {
                    //fTip.Hide(arborViewer1);
                    ToolTipService.SetToolTip(arborViewer1, null);
                    fTipShow = false;
                }
            } else {
                if (!fTipShow) {
                    fTip.Content = resNode.Sign;
                    ToolTipService.SetToolTip(arborViewer1, fTip);
                    //fTip.Show(resNode.Sign, arborViewer1, e.X + 24, e.Y);
                    fTipShow = true;
                }
            }
        }
    }
}
