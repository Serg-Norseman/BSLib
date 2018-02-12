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
using System.Windows.Forms;

using BSLib.SmartGraph;

namespace BSLib.ArborGVT
{
    public sealed class ArborViewer : Panel, IArborRenderer
    {
    	private Graph fGraph;
        private bool fEnergyDebug;
        private ArborNode fDragged;
        private readonly Font fDrawFont;
        private bool fNodesDragging;
        private readonly StringFormat fStrFormat;
        private readonly ArborSystem fSys;
        private readonly SolidBrush fBlackBrush;
        private readonly SolidBrush fWhiteBrush;

        public bool EnergyDebug
        {
            get { return this.fEnergyDebug; }
            set { this.fEnergyDebug = value; }
        }

        public bool NodesDragging
        {
            get { return this.fNodesDragging; }
            set { this.fNodesDragging = value; }
        }

        public ArborSystem Sys
        {
            get { return this.fSys; }
        }

        public ArborViewer()
        {
            base.BorderStyle = BorderStyle.Fixed3D;
            base.TabStop = true;
            base.BackColor = Color.White;

            base.DoubleBuffered = true;
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.fGraph = new Graph();

            // repulsion - отталкивание, stiffness - тугоподвижность, friction - сила трения
            this.fSys = new ArborSystem(10000, 500/*1000*/, 0.1, this);
            this.fSys.setScreenSize(this.Width, this.Height);
            this.fSys.AutoStop = false;
            this.fSys.Graph = this.fGraph;

            this.fEnergyDebug = false;
            this.fDrawFont = new Font("Calibri", 9);

            this.fStrFormat = new StringFormat();
            this.fStrFormat.Alignment = StringAlignment.Center;
            this.fStrFormat.LineAlignment = StringAlignment.Center;

            this.fBlackBrush = new SolidBrush(Color.Black);
            this.fWhiteBrush = new SolidBrush(Color.White);
            this.fDragged = null;
            this.fNodesDragging = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.fSys.Dispose();
                this.fDrawFont.Dispose();
                this.fWhiteBrush.Dispose();
                this.fBlackBrush.Dispose();
                this.fStrFormat.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            this.fSys.setScreenSize(this.Width, this.Height);
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics gfx = pe.Graphics;

            try
            {
                gfx.SmoothingMode = SmoothingMode.AntiAlias;

                foreach (ArborNode node in fSys.Nodes)
                {
                    node.Box = this.getNodeRect(gfx, node);
                    gfx.FillRectangle(new SolidBrush(node.Color), node.Box);
                    gfx.DrawString(node.Sign, fDrawFont, this.fWhiteBrush, node.Box, this.fStrFormat);
                }

                using (Pen grayPen = new Pen(Color.Gray, 1))
                {
                    grayPen.StartCap = LineCap.NoAnchor;
                    grayPen.EndCap = LineCap.ArrowAnchor;

                    foreach (ArborEdge edge in fSys.Edges)
                    {
                        ArborNode srcNode = edge.Source;
                        ArborNode tgtNode = edge.Target;

                        ArborPoint pt1 = fSys.toScreen(srcNode.Pt);
                        ArborPoint pt2 = fSys.toScreen(tgtNode.Pt);

                        ArborPoint tail = intersect_line_box(pt1, pt2, srcNode.Box);
                        ArborPoint head = (tail.isNull()) ? ArborPoint.Null : intersect_line_box(tail, pt2, tgtNode.Box);

                        if (!head.isNull() && !tail.isNull())
                        {
                            gfx.DrawLine(grayPen, (int)tail.X, (int)tail.Y, (int)head.X, (int)head.Y);
                        }
                    }
                }

                if (this.fEnergyDebug)
                {
                    string energy = "max=" + fSys.EnergyMax.ToString("0.00000") + ", mean=" + fSys.EnergyMean.ToString("0.00000");
                    gfx.DrawString(energy, fDrawFont, this.fBlackBrush, 10, 10);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ArborViewer.OnPaint(): " + ex.Message);
            }
        }

        public static ArborPoint intersect_line_line(ArborPoint p1, ArborPoint p2, ArborPoint p3, ArborPoint p4)
        {
            double denom = ((p4.Y - p3.Y) * (p2.X - p1.X) - (p4.X - p3.X) * (p2.Y - p1.Y));
            if (denom == 0) return ArborPoint.Null; // lines are parallel

            double ua = ((p4.X - p3.X) * (p1.Y - p3.Y) - (p4.Y - p3.Y) * (p1.X - p3.X)) / denom;
            double ub = ((p2.X - p1.X) * (p1.Y - p3.Y) - (p2.Y - p1.Y) * (p1.X - p3.X)) / denom;

            if (ua < 0 || ua > 1 || ub < 0 || ub > 1) return ArborPoint.Null;

            return new ArborPoint(p1.X + ua * (p2.X - p1.X), p1.Y + ua * (p2.Y - p1.Y));
        }

        public ArborPoint intersect_line_box(ArborPoint p1, ArborPoint p2, RectangleF boxTuple)
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

            pt = intersect_line_line(p1, p2, tl, tr);
            if (!pt.isNull()) return pt;

            pt = intersect_line_line(p1, p2, tr, br);
            if (!pt.isNull()) return pt;

            pt = intersect_line_line(p1, p2, br, bl);
            if (!pt.isNull()) return pt;

            pt = intersect_line_line(p1, p2, bl, tl);
            if (!pt.isNull()) return pt;

            return ArborPoint.Null;
        }

        public void start()
        {
            this.fSys.start();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (!this.Focused) base.Focus();

            if (this.fNodesDragging)
            {
                this.fDragged = this.getNodeByCoord(e.X, e.Y);

                if (this.fDragged != null)
                {
                    this.fDragged.Fixed = true;
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (this.fNodesDragging && this.fDragged != null)
            {
                this.fDragged.Fixed = false;
                //this.fDragged.Mass = 1000;
                this.fDragged = null;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this.fNodesDragging && this.fDragged != null)
            {
                this.fDragged.Pt = fSys.fromScreen(e.X, e.Y);
            }
        }

        public RectangleF getNodeRect(Graphics gfx, ArborNode node)
        {
            SizeF tsz = gfx.MeasureString(node.Sign, fDrawFont);
            float w = tsz.Width + 10;
            float h = tsz.Height + 4;
            ArborPoint pt = fSys.toScreen(node.Pt);
            pt.X = Math.Floor(pt.X);
            pt.Y = Math.Floor(pt.Y);

            return new RectangleF((float)pt.X - w / 2, (float)pt.Y - h / 2, w, h);
        }

        public ArborNode getNodeByCoord(int x, int y)
        {
            return fSys.getNearest(x, y);

            /*foreach (ArborNode node in fSys.Nodes)
            {
                if (node.Box.Contains(x, y)) {
                    return node;
                }
            }
            return null;*/
        }

        public static void createSample(Graph graph)
        {
        	graph.BeginUpdate();

            graph.AddVertex("1")/*.Mass = 50*/;

            graph.AddDirectedEdge("1", "4"/*, 10*/);
            graph.AddDirectedEdge("1", "12"/*, 10*/);
            graph.AddDirectedEdge("4", "21");
            graph.AddDirectedEdge("4", "23"/*, 20*/);
            graph.AddDirectedEdge("7", "34");
            graph.AddDirectedEdge("7", "13");
            graph.AddDirectedEdge("7", "44");
            graph.AddDirectedEdge("12", "25");
            graph.AddDirectedEdge("12", "24");
            graph.AddDirectedEdge("23", "50");
            graph.AddDirectedEdge("23", "53");
            graph.AddDirectedEdge("24", "6");
            graph.AddDirectedEdge("24", "42"/*, 20*/);
            graph.AddDirectedEdge("25", "94");
            graph.AddDirectedEdge("25", "66");
            graph.AddDirectedEdge("32", "47");
            graph.AddDirectedEdge("32", "84");
            graph.AddDirectedEdge("42", "32");
            graph.AddDirectedEdge("42", "7");
            graph.AddDirectedEdge("50", "72");
            graph.AddDirectedEdge("50", "65");
            graph.AddDirectedEdge("53", "67");
            graph.AddDirectedEdge("53", "68");
            graph.AddDirectedEdge("66", "79");
            graph.AddDirectedEdge("66", "80");
            graph.AddDirectedEdge("67", "88");
            graph.AddDirectedEdge("67", "83");
            graph.AddDirectedEdge("68", "77");
            graph.AddDirectedEdge("68", "91");
            graph.AddDirectedEdge("80", "99");
            graph.AddDirectedEdge("80", "97");
            graph.AddDirectedEdge("88", "110");
            graph.AddDirectedEdge("88", "104");
            graph.AddDirectedEdge("91", "106");
            graph.AddDirectedEdge("91", "100");

            graph.EndUpdate();
        }
    }
}

#endif
