/*
 *  ArborGVT - a graph vizualization toolkit
 *
 *  Physics code derived from springy.js, copyright (c) 2010 Dennis Hotson
 *  JavaScript library, copyright (c) 2011 Samizdat Drafting Co.
 *
 *  Fork and C# implementation, copyright (c) 2012,2016 by Serg V. Zhdanovskih.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using BSLib.SmartGraph;

namespace BSLib.ArborGVT
{
    internal class PSBounds
    {
        public ArborPoint LeftTop = ArborPoint.Null;
        public ArborPoint RightBottom = ArborPoint.Null;

        public PSBounds(ArborPoint leftTop, ArborPoint rightBottom)
        {
            LeftTop = leftTop;
            RightBottom = rightBottom;
        }
    }

    public abstract class ArborSystem : BaseObject
    {
        private static readonly Random _random = new Random();

        private readonly int[] margins = new int[4] { 20, 20, 20, 20 };
        private const double Mag = 0.04;

        private bool fAutoStop;
        private bool fBusy;
        private readonly List<ArborEdge> fEdges;
        private Graph fGraph;
        private PSBounds fGraphBounds;
        private readonly Hashtable fNames;
        private readonly List<ArborNode> fNodes;
        private EventHandler fOnStart;
        private EventHandler fOnStop;
        private readonly IArborRenderer fRenderer;
        private DateTime fPrevTime;
        private int fScreenHeight;
        private int fScreenWidth;
        private double fStopThreshold;
        private PSBounds fViewBounds;

        public double EnergySum = 0;
        public double EnergyMax = 0;
        public double EnergyMean = 0;

        public double ParamRepulsion = 1000; // отражение, отвращение, отталкивание
        public double ParamStiffness = 600; // церемонность, тугоподвижность
        public double ParamFriction = 0.5; // трение
        public double ParamDt = 0.01; // 0.02;
        public bool ParamGravity = false;
        public double ParamPrecision = 0.6;
        public double ParamTimeout = 1000 / 100;
        public double ParamTheta = 0.4;

        #region Properties

        public bool AutoStop
        {
            get { return fAutoStop; }
            set { fAutoStop = value; }
        }

        public Graph Graph
        {
            get {
                return fGraph;
            }
            set {
                if (fGraph != value)
                {
                    if (fGraph != null)
                    {
                        fGraph.OnChange -= notifyEventHandler;
                    }

                    fGraph = value;

                    if (fGraph != null)
                    {
                        fGraph.OnChange += notifyEventHandler;
                        syncGraph();
                    }
                }
            }
        }

        public List<ArborNode> Nodes
        {
            get { return fNodes; }
        }

        public List<ArborEdge> Edges
        {
            get { return fEdges; }
        }

        public event EventHandler OnStart
        {
            add
            {
                fOnStart = value;
            }
            remove
            {
                if (fOnStart == value)
                {
                    fOnStart = null;
                }
            }
        }

        public event EventHandler OnStop
        {
            add
            {
                fOnStop = value;
            }
            remove
            {
                if (fOnStop == value)
                {
                    fOnStop = null;
                }
            }
        }

        public double StopThreshold
        {
            get { return fStopThreshold; }
            set { fStopThreshold = value; }
        }

        #endregion

        public ArborSystem(double repulsion, double stiffness, double friction, IArborRenderer renderer)
        {
            fAutoStop = true;
            fBusy = false;
            fNames = new Hashtable();
            fNodes = new List<ArborNode>();
            fEdges = new List<ArborEdge>();
            fRenderer = renderer;
            fStopThreshold = /*0.05*/ 0.7;

            ParamRepulsion = repulsion;
            ParamStiffness = stiffness;
            ParamFriction = friction;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                stop();
            }
            base.Dispose(disposing);
        }

        protected abstract void StartTimer();

        protected abstract void StopTimer();

        private void syncGraph()
        {
            fEdges.Clear();
            fNodes.Clear();

            foreach (Vertex vertex in fGraph.Vertices)
            {
                ArborNode node = CreateNode(vertex.Sign);
                vertex.Extensions.Add(node);

                resetCoords(node);
                fNodes.Add(node);
            }

            foreach (Edge edge in fGraph.Edges)
            {
                ArborNode anSrc = edge.Source.Extensions.Find<ArborNode>();
                ArborNode anTgt = edge.Target.Extensions.Find<ArborNode>();

                ArborEdge arbEdge = CreateEdge(anSrc, anTgt, 1, ParamStiffness);
                edge.Extensions.Add(arbEdge);
                fEdges.Add(arbEdge);
            }
        }

        private void notifyEventHandler(object sender, NotifyEventArgs e)
        {
            syncGraph();
        }

        public void start()
        {
            //syncGraph();

            if (fOnStart != null) fOnStart(this, new EventArgs());

            fPrevTime = DateTime.FromBinary(0);

            StartTimer();
        }

        public void stop()
        {
            StopTimer();

            if (fOnStop != null) fOnStop(this, new EventArgs());
        }

        protected virtual ArborNode CreateNode(string sign)
        {
            return new ArborNode(sign);
        }

        protected virtual ArborEdge CreateEdge(ArborNode src, ArborNode tgt, double len, double stiffness, bool directed = false)
        {
            return new ArborEdge(src, tgt, len, stiffness, directed);
        }

        public ArborNode addNode(string sign, double x, double y)
        {
            ArborNode node = getNode(sign);
            if (node != null) return node;

            node = CreateNode(sign);
            node.Pt = new ArborPoint(x, y);

            fNames.Add(sign, node);
            fNodes.Add(node);

            return node;
        }

        public ArborNode addNode(string sign)
        {
            ArborPoint lt = fGraphBounds.LeftTop;
            ArborPoint rb = fGraphBounds.RightBottom;
            double xx = lt.X + (rb.X - lt.X) * ArborSystem.getRndDouble();
            double yy = lt.Y + (rb.Y - lt.Y) * ArborSystem.getRndDouble();

            return addNode(sign, xx, yy);
        }

        private void resetCoords(ArborNode node)
        {
            ArborPoint lt = fGraphBounds.LeftTop;
            ArborPoint rb = fGraphBounds.RightBottom;
            double xx = lt.X + (rb.X - lt.X) * ArborSystem.getRndDouble();
            double yy = lt.Y + (rb.Y - lt.Y) * ArborSystem.getRndDouble();

            node.Pt = new ArborPoint(xx, yy);
        }
        
        public ArborNode getNode(string sign)
        {
            return (ArborNode)fNames[sign];
        }

        public ArborEdge addEdge(string srcSign, string tgtSign, double len = 1.0)
        {
            ArborNode src = getNode(srcSign);
            src = (src != null) ? src : addNode(srcSign);

            ArborNode tgt = getNode(tgtSign);
            tgt = (tgt != null) ? tgt : addNode(tgtSign);

            ArborEdge x = null;
            if (src != null && tgt != null)
            {
                foreach (ArborEdge edge in fEdges)
                {
                    if (edge.Source == src && edge.Target == tgt)
                    {
                        x = edge;
                        break;
                    }
                }
            }

            if (x == null)
            {
                x = CreateEdge(src, tgt, len, ParamStiffness);
                fEdges.Add(x);
            }

            return x;
        }

        public void setScreenSize(int width, int height)
        {
            fScreenWidth = width;
            fScreenHeight = height;
            updateViewBounds();
        }

        public ArborPoint toScreen(ArborPoint pt)
        {
            if (fViewBounds == null) return ArborPoint.Null;

            ArborPoint vd = fViewBounds.RightBottom.sub(fViewBounds.LeftTop);
            double sx = margins[3] + pt.sub(fViewBounds.LeftTop).div(vd.X).X * (fScreenWidth - (margins[1] + margins[3]));
            double sy = margins[0] + pt.sub(fViewBounds.LeftTop).div(vd.Y).Y * (fScreenHeight - (margins[0] + margins[2]));
            return new ArborPoint(sx, sy);
        }

        public ArborPoint fromScreen(double sx, double sy)
        {
            if (fViewBounds == null) return ArborPoint.Null;

            ArborPoint vd = fViewBounds.RightBottom.sub(fViewBounds.LeftTop);
            double x = (sx - margins[3]) / (fScreenWidth - (margins[1] + margins[3])) * vd.X + fViewBounds.LeftTop.X;
            double y = (sy - margins[0]) / (fScreenHeight - (margins[0] + margins[2])) * vd.Y + fViewBounds.LeftTop.Y;
            return new ArborPoint(x, y);
        }

        public ArborNode getNearest(int sx, int sy)
        {
            ArborPoint x = fromScreen(sx, sy);

            ArborNode resNode = null;
            double minDist = +1.0;

            for (int i = 0, nodesCount = fNodes.Count; i < nodesCount; i++)
            {
                ArborNode node = fNodes[i];
                ArborPoint z = node.Pt;
                if (z.exploded())
                {
                    continue;
                }

                double dist = z.sub(x).magnitude();
                if (dist < minDist)
                {
                    resNode = node;
                    minDist = dist;
                }
            }

            return resNode;
        }

        private void updateGraphBounds()
        {
            ArborPoint lt = new ArborPoint(-1, -1);
            ArborPoint rb = new ArborPoint(1, 1);

            for (int i = 0, nodesCount = fNodes.Count; i < nodesCount; i++)
            {
                ArborNode node = fNodes[i];
                ArborPoint pt = node.Pt;
                if (pt.exploded()) continue;

                if (pt.X < lt.X) lt.X = pt.X;
                if (pt.Y < lt.Y) lt.Y = pt.Y;
                if (pt.X > rb.X) rb.X = pt.X;
                if (pt.Y > rb.Y) rb.Y = pt.Y;
            }

            lt.X -= 1.2;
            lt.Y -= 1.2;
            rb.X += 1.2;
            rb.Y += 1.2;

            ArborPoint sz = rb.sub(lt);
            ArborPoint cent = lt.add(sz.div(2));
            ArborPoint d = new ArborPoint(Math.Max(sz.X, 4.0), Math.Max(sz.Y, 4.0)).div(2);

            fGraphBounds = new PSBounds(cent.sub(d), cent.add(d));
        }

        private void updateViewBounds()
        {
            try
            {
                updateGraphBounds();

                if (fViewBounds == null)
                {
                    fViewBounds = fGraphBounds;
                    return;
                }

                ArborPoint vLT = fGraphBounds.LeftTop.sub(fViewBounds.LeftTop).mul(Mag);
                ArborPoint vRB = fGraphBounds.RightBottom.sub(fViewBounds.RightBottom).mul(Mag);

                double aX = vLT.magnitude() * fScreenWidth;
                double aY = vRB.magnitude() * fScreenHeight;

                if (aX > 1 || aY > 1)
                {
                    ArborPoint nbLT = fViewBounds.LeftTop.add(vLT);
                    ArborPoint nbRB = fViewBounds.RightBottom.add(vRB);

                    fViewBounds = new PSBounds(nbLT, nbRB);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ArborSystem.updateViewBounds(): " + ex.Message);
            }
        }

        protected void TickTimer()
        {
            if (fBusy) return;
            fBusy = true;
            try
            {
                updatePhysics();
                updateViewBounds();

                if (fRenderer != null)
                {
                    fRenderer.Invalidate();
                }

                if (fAutoStop)
                {
                    if (EnergyMean <= fStopThreshold)
                    {
                        if (fPrevTime == DateTime.FromBinary(0))
                        {
                            fPrevTime = DateTime.Now;
                        }
                        TimeSpan ts = DateTime.Now - fPrevTime;
                        if (ts.TotalMilliseconds > 1000)
                        {
                            stop();
                        }
                    }
                    else
                    {
                        fPrevTime = DateTime.FromBinary(0);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ArborSystem.tickTimer(): " + ex.Message);
            }
            fBusy = false;
        }

        private void updatePhysics()
        {
            try
            {
                // tend particles
                for (int i = 0, nodesCount = fNodes.Count; i < nodesCount; i++)
                {
                    ArborPoint pV = fNodes[i].V;
                    pV.X = 0;
                    pV.Y = 0;
                }

                if (ParamStiffness > 0)
                {
                    applySprings();
                }

                // euler integrator
                if (ParamRepulsion > 0)
                {
                    applyBarnesHutRepulsion();
                }

                updateVelocityAndPosition(ParamDt);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ArborSystem.updatePhysics(): " + ex.Message);
            }
        }

        private void applyBarnesHutRepulsion()
        {
            BarnesHutTree bht = new BarnesHutTree(fGraphBounds.LeftTop, fGraphBounds.RightBottom, ParamTheta);

            int nodesCount = fNodes.Count;
            for (int i = 0; i < nodesCount; i++)
            {
                ArborNode node = fNodes[i];
                bht.insert(node);
            }

            for (int i = 0; i < nodesCount; i++)
            {
                ArborNode node = fNodes[i];
                bht.applyForces(node, ParamRepulsion);
            }
        }

        private void applySprings()
        {
            for (int i = 0, edgesCount = fEdges.Count; i < edgesCount; i++)
            {
                ArborEdge edge = fEdges[i];

                ArborPoint s = edge.Target.Pt.sub(edge.Source.Pt);
                double sMag = s.magnitude();

                ArborPoint r = ((sMag > 0) ? s : ArborPoint.newRnd(1)).normalize();
                double q = edge.Stiffness * (edge.Length - sMag);

                edge.Source.applyForce(r.mul(q * -0.5));
                edge.Target.applyForce(r.mul(q * 0.5));
            }
        }

        private void updateVelocityAndPosition(double dt)
        {
            int size = fNodes.Count;
            if (size == 0)
            {
                EnergySum = 0;
                EnergyMax = 0;
                EnergyMean = 0;
                return;
            }

            double eSum = 0;
            double eMax = 0;

            // calc center drift
            ArborPoint rr = ArborPoint.Zero;
            for (int i = 0; i < size; i++)
            {
                ArborNode node = fNodes[i];
                rr = rr.sub(node.Pt);
            }
            ArborPoint drift = rr.div(size);

            // main updates loop
            for (int i = 0; i < size; i++)
            {
                ArborNode node = fNodes[i];

                // apply center drift
                node.applyForce(drift);

                // apply center gravity
                if (ParamGravity)
                {
                    ArborPoint q = node.Pt.mul(-1);
                    node.applyForce(q.mul(ParamRepulsion / 100));
                }

                // update velocities
                if (node.Fixed)
                {
                    node.V = ArborPoint.Zero;
                }
                else
                {
                    node.V = node.V.add(node.F.mul(dt));
                    node.V = node.V.mul(1 - ParamFriction);

                    double r = node.V.magnitudeSquare();
                    if (r > 1000000)
                    {
                        node.V = node.V.div(r);
                    }
                }

                node.F.X = node.F.Y = 0;

                // update positions
                node.Pt = node.Pt.add(node.V.mul(dt));

                // update energy
                double z = node.V.magnitudeSquare();
                eSum += z;
                eMax = Math.Max(z, eMax);
            }

            EnergySum = eSum;
            EnergyMax = eMax;
            EnergyMean = eSum / size;
        }

        internal static double getRndDouble()
        {
            return _random.NextDouble();
        }

        public static void CreateSample(Graph graph)
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
