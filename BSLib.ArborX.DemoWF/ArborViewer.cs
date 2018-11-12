/*
 *  ArborGVT - a graph vizualization toolkit
 *
 *  Physics code derived from springy.js, copyright (c) 2010 Dennis Hotson
 *  JavaScript library, copyright (c) 2011 Samizdat Drafting Co.
 *
 *  Fork and C# implementation, copyright (c) 2012,2016 by Serg V. Zhdanovskih.
 */

#if !NETSTANDARD

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Timers;
using System.Windows.Forms;

using BSLib.ArborGVT;
using BSLib.SmartGraph;

namespace BSLib.ArborX.DemoWF
{
    public sealed class ArborNodeEx : ArborNode
    {
        public Color Color;
        public RectangleF Box;

        public ArborNodeEx(string sign) : base(sign)
        {
            Color = Color.Gray;
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
            fTimer.Interval = ParamTimeout;
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

        protected override ArborEdge CreateEdge(ArborNode src, ArborNode tgt, double len, double stiffness, bool directed = false)
        {
            return new ArborEdge(src, tgt, len, stiffness, directed);
        }
    }

    public sealed class ArborViewer : Panel, IArborRenderer
    {
        private ArborNode fDragged;
        private bool fEnergyDebug;
        private Graph fGraph;
        private bool fNodesDragging;

        private readonly Font fDrawFont;
        private readonly Pen fLinePen;
        private readonly StringFormat fStrFormat;
        private readonly ArborSystem fSys;
        private readonly SolidBrush fBlackBrush;
        private readonly SolidBrush fWhiteBrush;

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

        public ArborSystem Sys
        {
            get { return fSys; }
        }

        public ArborViewer()
        {
            base.BackColor = Color.White;
            base.BorderStyle = BorderStyle.Fixed3D;
            base.DoubleBuffered = true;
            base.TabStop = true;
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            fGraph = new Graph();

            // repulsion - отталкивание, stiffness - тугоподвижность, friction - сила трения
            fSys = new ArborSystemEx(10000.0f, 500.0f/*1000.0f*/, 0.1f, this);
            fSys.setScreenSize(Width, Height);
            fSys.AutoStop = true;
            fSys.Graph = fGraph;

            fDragged = null;
            fEnergyDebug = false;
            fNodesDragging = false;

            fDrawFont = new Font("Calibri", 9);

            fLinePen = new Pen(Color.Gray, 1);
            fLinePen.StartCap = LineCap.NoAnchor;
            fLinePen.EndCap = LineCap.ArrowAnchor;

            fStrFormat = new StringFormat();
            fStrFormat.Alignment = StringAlignment.Center;
            fStrFormat.LineAlignment = StringAlignment.Center;

            fBlackBrush = new SolidBrush(Color.Black);
            fWhiteBrush = new SolidBrush(Color.White);
        }

        protected override void Dispose(bool disposing)
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
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            fSys.setScreenSize(Width, Height);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try {
                Graphics gfx = e.Graphics;
                gfx.SmoothingMode = SmoothingMode.AntiAlias;

                for (int i = 0, edgesCount = fSys.Edges.Count; i < edgesCount; i++) {
                    ArborEdge edge = fSys.Edges[i];

                    var srcNode = edge.Source as ArborNodeEx;
                    var tgtNode = edge.Target as ArborNodeEx;

                    ArborPoint pt1 = fSys.toScreen(srcNode.Pt);
                    ArborPoint pt2 = fSys.toScreen(tgtNode.Pt);

                    ArborPoint tail = IntersectLineBox(pt1, pt2, srcNode.Box);
                    ArborPoint head = (tail.isNull()) ? ArborPoint.Null : IntersectLineBox(tail, pt2, tgtNode.Box);

                    if (!head.isNull() && !tail.isNull()) {
                        gfx.DrawLine(fLinePen, (int)tail.X, (int)tail.Y, (int)head.X, (int)head.Y);
                    }
                }

                for (int i = 0, nodesCount = fSys.Nodes.Count; i < nodesCount; i++) {
                    ArborNode node = fSys.Nodes[i];
                    var xnode = node as ArborNodeEx;

                    xnode.Box = GetNodeRect(gfx, node);
                    gfx.FillRectangle(new SolidBrush(xnode.Color), xnode.Box);
                    gfx.DrawString(node.Sign, fDrawFont, fWhiteBrush, xnode.Box, fStrFormat);
                }

                if (fEnergyDebug) {
                    string energy = "max=" + fSys.EnergyMax.ToString("0.00000") + ", mean=" + fSys.EnergyMean.ToString("0.00000");
                    gfx.DrawString(energy, fDrawFont, fBlackBrush, 10, 10);
                }
            } catch (Exception ex) {
                Debug.WriteLine("ArborViewer.OnPaint(): " + ex.Message);
            }
        }

        public static ArborPoint IntersectLineLine(ArborPoint p1, ArborPoint p2, ArborPoint p3, ArborPoint p4)
        {
            double denom = ((p4.Y - p3.Y) * (p2.X - p1.X) - (p4.X - p3.X) * (p2.Y - p1.Y));
            if (denom == 0f) return ArborPoint.Null; // lines are parallel

            double ua = ((p4.X - p3.X) * (p1.Y - p3.Y) - (p4.Y - p3.Y) * (p1.X - p3.X)) / denom;
            double ub = ((p2.X - p1.X) * (p1.Y - p3.Y) - (p2.Y - p1.Y) * (p1.X - p3.X)) / denom;

            if (ua < 0f || ua > 1f || ub < 0f || ub > 1f) return ArborPoint.Null;

            return new ArborPoint(p1.X + ua * (p2.X - p1.X), p1.Y + ua * (p2.Y - p1.Y));
        }

        public ArborPoint IntersectLineBox(ArborPoint p1, ArborPoint p2, RectangleF boxTuple)
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
            if (!pt.isNull()) return pt;

            pt = IntersectLineLine(p1, p2, tr, br);
            if (!pt.isNull()) return pt;

            pt = IntersectLineLine(p1, p2, br, bl);
            if (!pt.isNull()) return pt;

            pt = IntersectLineLine(p1, p2, bl, tl);
            if (!pt.isNull()) return pt;

            return ArborPoint.Null;
        }

        public void Start()
        {
            fSys.start();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (!Focused) base.Focus();

            if (fNodesDragging) {
                fDragged = GetNodeByCoords(e.X, e.Y);

                if (fDragged != null) {
                    fDragged.Fixed = true;
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (fNodesDragging && fDragged != null) {
                fDragged.Fixed = false;
                //fDragged.Mass = 1000;
                fDragged = null;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (fNodesDragging && fDragged != null) {
                fDragged.Pt = fSys.fromScreen(e.X, e.Y);
            }
        }

        public RectangleF GetNodeRect(Graphics gfx, ArborNode node)
        {
            SizeF tsz = gfx.MeasureString(node.Sign, fDrawFont);
            float w = tsz.Width + 10;
            float h = tsz.Height + 4;
            ArborPoint pt = fSys.toScreen(node.Pt);
            pt.X = Math.Floor(pt.X);
            pt.Y = Math.Floor(pt.Y);

            return new RectangleF((float)pt.X - w / 2, (float)pt.Y - h / 2, w, h);
        }

        public ArborNode GetNodeByCoords(int x, int y)
        {
            return fSys.getNearest(x, y);

            /*foreach (ArborNode node in fSys.Nodes) {
                if (node.Box.Contains(x, y)) {
                    return node;
                }
            }
            return null;*/
        }
    }
}

#endif
