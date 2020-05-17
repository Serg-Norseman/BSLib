/*
 *  ArborGVT - a graph vizualization toolkit
 *
 *  Physics code derived from springy.js, copyright (c) 2010 Dennis Hotson
 *  JavaScript library, copyright (c) 2011 Samizdat Drafting Co.
 *
 *  Fork and C# implementation, copyright (c) 2012,2016 by Serg V. Zhdanovskih.
 */

using System;
using System.Diagnostics;
using System.Timers;
using BSLib.DataViz.ArborGVT;
using BSLib.DataViz.SmartGraph;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace BSLib.ArborX.DemoUWP
{
    public sealed class ArborNodeEx : ArborNode
    {
        public Color Color;
        public Rect Box;

        public ArborNodeEx(string sign) : base(sign)
        {
            Color = Colors.Gray;
        }
    }

    public sealed class ArborSystemEx : ArborSystem
    {
        private System.Timers.Timer fTimer;

        public ArborSystemEx(double repulsion, double stiffness, double friction, IArborRenderer renderer)
            : base(repulsion, stiffness, friction, renderer)
        {
            fTimer = null;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            TickTimer();
        }

        protected override void StartTimer()
        {
            fTimer = new System.Timers.Timer();
            fTimer.AutoReset = true;
            fTimer.Interval = TimerInterval;
            fTimer.Elapsed += TimerElapsed;
            fTimer.Start();
        }

        protected override void StopTimer()
        {
            if (fTimer != null) {
                fTimer.Stop();
                fTimer.Dispose();
                fTimer = null;
            }
        }

        protected override ArborNode CreateNode(string sign)
        {
            return new ArborNodeEx(sign);
        }

        protected override ArborEdge CreateEdge(ArborNode source, ArborNode target, double length, double stiffness,
                                                bool directed = false)
        {
            return new ArborEdge(source, target, length, stiffness, directed);
        }
    }

    public sealed partial class ArborViewer : UserControl, IArborRenderer
    {
        private ArborNode fDragged;
        private bool fEnergyDebug;
        private bool fNodesDragging;

        //private readonly Font fDrawFont;
        //private readonly Pen fLinePen;
        private readonly CanvasTextFormat fStrFormat;
        private readonly ArborSystem fSystem;
        private readonly SolidColorBrush fBlackBrush;
        private readonly SolidColorBrush fWhiteBrush;

        public bool EnergyDebug
        {
            get { return fEnergyDebug; }
            set { fEnergyDebug = value; }
        }

        public bool NodesDragging
        {
            get { return fNodesDragging; }
            set { fNodesDragging = value; }
        }

        public ArborSystem System
        {
            get { return fSystem; }
        }

        public ArborViewer()
        {
            InitializeComponent();


            // repulsion - отталкивание, stiffness - тугоподвижность, friction - сила трения
            fSystem = new ArborSystemEx(10000.0f, 500.0f /*1000.0f*/, 0.1f, this);
            fSystem.SetViewSize((int)Width, (int)Height);
            fSystem.AutoStop = true;
            fSystem.Graph = new Graph();

            fDragged = null;
            fEnergyDebug = false;
            fNodesDragging = false;

            //fDrawFont = new Font("Calibri", 9);

            //fLinePen = new Pen(Color.Gray, 1);
            //fLinePen.StartCap = LineCap.NoAnchor;
            //fLinePen.EndCap = LineCap.ArrowAnchor;

            fStrFormat = new CanvasTextFormat();
            fStrFormat.HorizontalAlignment = CanvasHorizontalAlignment.Center;
            fStrFormat.VerticalAlignment = CanvasVerticalAlignment.Center;
            fStrFormat.FontSize = 9;
            fStrFormat.WordWrapping = CanvasWordWrapping.NoWrap;

            fBlackBrush = new SolidColorBrush(Colors.Black);
            fWhiteBrush = new SolidColorBrush(Colors.White);
        }

       /* protected override void Dispose(bool disposing)
        {
            if (disposing) {
                fSys.Dispose();
                fDrawFont.Dispose();
                fLinePen.Dispose();
                fWhiteBrush.Dispose();
                fBlackBrush.Dispose();
                fStrFormat.Dispose();
            }
            base.Dispose(disposing);
        }*/

        public void Invalidate()
        {
            canv.Invalidate();
        }

        private void AV_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            fSystem.SetViewSize((int)e.NewSize.Width, (int)e.NewSize.Height);
            Invalidate();
        }

        private void AV_Unloaded(object sender, RoutedEventArgs e)
        {
            canv.RemoveFromVisualTree();
            canv = null;
        }

        private void AV_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            try {
                var gfx = args.DrawingSession;
                gfx.Antialiasing = CanvasAntialiasing.Antialiased;

                for (int i = 0, edgesCount = fSystem.Edges.Count; i < edgesCount; i++) {
                    ArborEdge edge = fSystem.Edges[i];

                    var sourceNode = (ArborNodeEx)edge.Source;
                    var targetNode = (ArborNodeEx)edge.Target;

                    ArborPoint pt1 = fSystem.GetViewCoords(sourceNode.Pt);
                    ArborPoint pt2 = fSystem.GetViewCoords(targetNode.Pt);

                    ArborPoint tail = IntersectLineBox(pt1, pt2, sourceNode.Box);
                    ArborPoint head = (tail.IsNull()) ? ArborPoint.Null : IntersectLineBox(tail, pt2, targetNode.Box);

                    if (!head.IsNull() && !tail.IsNull()) {
                        gfx.DrawLine((float)tail.X, (float)tail.Y, (float)head.X, (float)head.Y, Colors.Gray, 1);
                    }
                }

                for (int i = 0, nodesCount = fSystem.Nodes.Count; i < nodesCount; i++) {
                    ArborNodeEx node = (ArborNodeEx)fSystem.Nodes[i];

                    node.Box = GetNodeRect(gfx, node);
                    gfx.FillRectangle(node.Box, node.Color);
                    gfx.DrawText(node.Sign, node.Box, Colors.White, fStrFormat);
                }

                if (fEnergyDebug) {
                    string energy = "max=" + fSystem.EnergyMax.ToString("0.00000") + ", mean=" + fSystem.EnergyMean.ToString("0.00000");
                    gfx.DrawText(energy, 10, 10, Colors.Black);
                }
            } catch (Exception ex) {
                Debug.WriteLine("ArborViewer.OnPaint(): " + ex.Message);
            }
        }

        public static ArborPoint IntersectLineLine(ArborPoint p1, ArborPoint p2, ArborPoint p3, ArborPoint p4)
        {
            double denom = ((p4.Y - p3.Y) * (p2.X - p1.X) - (p4.X - p3.X) * (p2.Y - p1.Y));
            if (denom == 0.0f) return ArborPoint.Null; // lines are parallel

            double ua = ((p4.X - p3.X) * (p1.Y - p3.Y) - (p4.Y - p3.Y) * (p1.X - p3.X)) / denom;
            double ub = ((p2.X - p1.X) * (p1.Y - p3.Y) - (p2.Y - p1.Y) * (p1.X - p3.X)) / denom;

            if (ua < 0.0f || ua > 1.0f || ub < 0.0f || ub > 1.0f) return ArborPoint.Null;

            return new ArborPoint(p1.X + ua * (p2.X - p1.X), p1.Y + ua * (p2.Y - p1.Y));
        }

        public ArborPoint IntersectLineBox(ArborPoint p1, ArborPoint p2, Rect boxTuple)
        {
            double bx = boxTuple.X;
            double by = boxTuple.Y;
            double bw = boxTuple.Width;
            double bh = boxTuple.Height;

            ArborPoint tl = new ArborPoint(bx, by);
            ArborPoint tr = new ArborPoint(bx + bw, by);
            ArborPoint bl = new ArborPoint(bx, by + bh);
            ArborPoint br = new ArborPoint(bx + bw, by + bh);

            ArborPoint pt;

            pt = IntersectLineLine(p1, p2, tl, tr);
            if (!pt.IsNull()) return pt;

            pt = IntersectLineLine(p1, p2, tr, br);
            if (!pt.IsNull()) return pt;

            pt = IntersectLineLine(p1, p2, br, bl);
            if (!pt.IsNull()) return pt;

            pt = IntersectLineLine(p1, p2, bl, tl);
            if (!pt.IsNull()) return pt;

            return ArborPoint.Null;
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            //if (!Focused) base.Focus();

            if (fNodesDragging) {
                Point pt = e.GetCurrentPoint(this).Position;
                fDragged = fSystem.GetNearestNode((int)pt.X, (int)pt.Y);

                if (fDragged != null) {
                    fDragged.Fixed = true;
                }
            }
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);

            if (fNodesDragging && fDragged != null) {
                fDragged.Fixed = false;
                fDragged = null;
            }
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            base.OnPointerMoved(e);

            if (fNodesDragging && fDragged != null) {
                Point pt = e.GetCurrentPoint(this).Position;
                fDragged.Pt = fSystem.GetModelCoords(pt.X, pt.Y);
            }
        }

        public Rect GetNodeRect(CanvasDrawingSession gfx, ArborNode node)
        {
            CanvasTextLayout textLayout = new CanvasTextLayout(gfx, node.Sign, fStrFormat, 0.0f, 0.0f);
            double w = textLayout.DrawBounds.Width + 10;
            double h = textLayout.DrawBounds.Height + 4;
            ArborPoint pt = fSystem.GetViewCoords(node.Pt);
            return new Rect(pt.X - w / 2, pt.Y - h / 2, w, h);
        }
    }
}
