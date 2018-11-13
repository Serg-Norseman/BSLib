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
        private static readonly int[] Margins = new int[4] { 20, 20, 20, 20 };

        protected const double Dt = 0.01f;
        protected const double Mag = 0.04f;
        protected const double Theta = 0.4f;
        protected const double TimerInterval = 1000.0f / 100.0f;

        public const double DefaultRepulsion = 1000.0f;
        public const double DefaultStiffness = 600.0f;
        public const double DefaultFriction = 0.5f;

        private bool fAutoStop;
        private bool fBusy;
        private readonly List<ArborEdge> fEdges;
        private double fEnergyMax;
        private double fEnergyMean;
        private double fEnergySum;
        private double fFriction;
        private Graph fGraph;
        private PSBounds fGraphBounds;
        private bool fGravity;
        private readonly Hashtable fNames;
        private readonly List<ArborNode> fNodes;
        private EventHandler fOnStart;
        private EventHandler fOnStop;
        private readonly IArborRenderer fRenderer;
        private DateTime fPrevTime;
        private double fRepulsion;
        private double fStiffness;
        private double fStopThreshold;
        private PSBounds fViewBounds;
        private int fViewHeight;
        private int fViewWidth;


        #region Properties

        public bool AutoStop
        {
            get { return fAutoStop; }
            set { fAutoStop = value; }
        }

        public double EnergyMax
        {
            get { return fEnergyMax; }
            set { fEnergyMax = value; }
        }

        public double EnergyMean
        {
            get { return fEnergyMean; }
            set { fEnergyMean = value; }
        }

        public double EnergySum
        {
            get { return fEnergySum; }
            set { fEnergySum = value; }
        }

        public double Friction
        {
            get { return fFriction; }
            set { fFriction = value; }
        }

        public Graph Graph
        {
            get {
                return fGraph;
            }
            set {
                if (fGraph != value) {
                    if (fGraph != null) {
                        fGraph.OnChange -= OnGraphChange;
                    }

                    fGraph = value;

                    if (fGraph != null) {
                        fGraph.OnChange += OnGraphChange;
                        SyncGraph();
                    }
                }
            }
        }

        public bool Gravity
        {
            get { return fGravity; }
            set { fGravity = value; }
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
            add {
                fOnStart = value;
            }
            remove {
                if (fOnStart == value) {
                    fOnStart = null;
                }
            }
        }

        public event EventHandler OnStop
        {
            add {
                fOnStop = value;
            }
            remove {
                if (fOnStop == value) {
                    fOnStop = null;
                }
            }
        }

        public double Repulsion
        {
            get { return fRepulsion; }
            set { fRepulsion = value; }
        }

        public double Stiffness
        {
            get { return fStiffness; }
            set { fStiffness = value; }
        }

        public double StopThreshold
        {
            get { return fStopThreshold; }
            set { fStopThreshold = value; }
        }

        #endregion

        protected ArborSystem(double repulsion, double stiffness, double friction, IArborRenderer renderer)
        {
            fAutoStop = true;
            fBusy = false;
            fGravity = false;
            fNames = new Hashtable();
            fNodes = new List<ArborNode>();
            fEdges = new List<ArborEdge>();
            fRenderer = renderer;
            fStopThreshold = 0.1f;

            fRepulsion = repulsion;
            fStiffness = stiffness;
            fFriction = friction;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                Stop();
            }
            base.Dispose(disposing);
        }

        protected abstract void StartTimer();

        protected abstract void StopTimer();

        private void SyncGraph()
        {
            fEdges.Clear();
            fNodes.Clear();

            foreach (Vertex vertex in fGraph.Vertices) {
                ArborNode node = CreateNode(vertex.Sign);
                vertex.Extensions.Add(node);

                ResetNodeCoords(node);
                fNodes.Add(node);
            }

            foreach (Edge edge in fGraph.Edges) {
                ArborNode anSrc = edge.Source.Extensions.Find<ArborNode>();
                ArborNode anTgt = edge.Target.Extensions.Find<ArborNode>();

                ArborEdge arbEdge = CreateEdge(anSrc, anTgt, 1.0f, fStiffness);
                edge.Extensions.Add(arbEdge);
                fEdges.Add(arbEdge);
            }
        }

        private void OnGraphChange(object sender, NotifyEventArgs e)
        {
            SyncGraph();
        }

        public void Start()
        {
            //SyncGraph();

            if (fOnStart != null) fOnStart(this, new EventArgs());

            fEnergyMax = 0.0f;
            fEnergyMean = 0.0f;
            fEnergySum = 0.0f;
            fPrevTime = DateTime.FromBinary(0);

            StartTimer();
        }

        public void Stop()
        {
            StopTimer();

            if (fOnStop != null) fOnStop(this, new EventArgs());
        }

        protected virtual ArborNode CreateNode(string sign)
        {
            return new ArborNode(sign);
        }

        protected virtual ArborEdge CreateEdge(ArborNode source, ArborNode target, double length, double stiffness,
                                               bool directed = false)
        {
            return new ArborEdge(source, target, length, stiffness, directed);
        }

        public ArborNode AddNode(string sign, double x, double y)
        {
            ArborNode node = GetNode(sign);
            if (node != null) return node;

            node = CreateNode(sign);
            node.Pt = new ArborPoint(x, y);

            fNames.Add(sign, node);
            fNodes.Add(node);

            return node;
        }

        public ArborNode AddNode(string sign)
        {
            ArborPoint lt = fGraphBounds.LeftTop;
            ArborPoint rb = fGraphBounds.RightBottom;
            double xx = lt.X + (rb.X - lt.X) * ArborSystem.GetRndDouble();
            double yy = lt.Y + (rb.Y - lt.Y) * ArborSystem.GetRndDouble();

            return AddNode(sign, xx, yy);
        }

        private void ResetNodeCoords(ArborNode node)
        {
            ArborPoint lt = fGraphBounds.LeftTop;
            ArborPoint rb = fGraphBounds.RightBottom;
            double xx = lt.X + (rb.X - lt.X) * ArborSystem.GetRndDouble();
            double yy = lt.Y + (rb.Y - lt.Y) * ArborSystem.GetRndDouble();

            node.Pt = new ArborPoint(xx, yy);
        }

        public ArborNode GetNode(string sign)
        {
            return (ArborNode)fNames[sign];
        }

        public ArborEdge AddEdge(string sourceSign, string targetSign, double length = 1.0f)
        {
            ArborNode src = GetNode(sourceSign);
            src = (src != null) ? src : AddNode(sourceSign);

            ArborNode tgt = GetNode(targetSign);
            tgt = (tgt != null) ? tgt : AddNode(targetSign);

            ArborEdge result = null;
            if (src != null && tgt != null) {
                foreach (ArborEdge edge in fEdges) {
                    if (edge.Source == src && edge.Target == tgt) {
                        result = edge;
                        break;
                    }
                }
            }

            if (result == null) {
                result = CreateEdge(src, tgt, length, fStiffness);
                fEdges.Add(result);
            }

            return result;
        }

        public void SetViewSize(int width, int height)
        {
            fViewWidth = width;
            fViewHeight = height;
            UpdateViewBounds();
        }

        public ArborPoint GetViewCoords(ArborPoint pt)
        {
            if (fViewBounds == null) return ArborPoint.Null;

            ArborPoint vd = fViewBounds.RightBottom.Sub(fViewBounds.LeftTop);
            double sx = Margins[3] + pt.Sub(fViewBounds.LeftTop).Div(vd.X).X * (fViewWidth - (Margins[1] + Margins[3]));
            double sy = Margins[0] + pt.Sub(fViewBounds.LeftTop).Div(vd.Y).Y * (fViewHeight - (Margins[0] + Margins[2]));
            return new ArborPoint(sx, sy);
        }

        public ArborPoint GetModelCoords(double viewX, double viewY)
        {
            if (fViewBounds == null) return ArborPoint.Null;

            ArborPoint vd = fViewBounds.RightBottom.Sub(fViewBounds.LeftTop);
            double x = (viewX - Margins[3]) / (fViewWidth - (Margins[1] + Margins[3])) * vd.X + fViewBounds.LeftTop.X;
            double y = (viewY - Margins[0]) / (fViewHeight - (Margins[0] + Margins[2])) * vd.Y + fViewBounds.LeftTop.Y;
            return new ArborPoint(x, y);
        }

        public ArborNode GetNearestNode(int viewX, int viewY)
        {
            ArborPoint pt = GetModelCoords(viewX, viewY);

            ArborNode result = null;
            double minDist = +1.0f;

            for (int i = 0, nodesCount = fNodes.Count; i < nodesCount; i++) {
                ArborNode node = fNodes[i];
                ArborPoint nodePt = node.Pt;
                if (nodePt.IsExploded()) continue;

                double dist = nodePt.Sub(pt).Magnitude();
                if (dist < minDist) {
                    result = node;
                    minDist = dist;
                }
            }

            return result;
        }

        private void UpdateGraphBounds()
        {
            ArborPoint lt = new ArborPoint(-1.0f, -1.0f);
            ArborPoint rb = new ArborPoint(+1.0f, +1.0f);

            for (int i = 0, nodesCount = fNodes.Count; i < nodesCount; i++) {
                ArborPoint pt = fNodes[i].Pt;
                if (pt.IsExploded()) continue;

                if (pt.X < lt.X) lt.X = pt.X;
                if (pt.Y < lt.Y) lt.Y = pt.Y;
                if (pt.X > rb.X) rb.X = pt.X;
                if (pt.Y > rb.Y) rb.Y = pt.Y;
            }

            lt.X -= 1.2f;
            lt.Y -= 1.2f;
            rb.X += 1.2f;
            rb.Y += 1.2f;

            ArborPoint sz = rb.Sub(lt);
            ArborPoint cent = lt.Add(sz.Div(2.0f));
            ArborPoint d = new ArborPoint(Math.Max(sz.X, 4.0f), Math.Max(sz.Y, 4.0f)).Div(2.0f);

            fGraphBounds = new PSBounds(cent.Sub(d), cent.Add(d));
        }

        private void UpdateViewBounds()
        {
            try {
                UpdateGraphBounds();

                if (fViewBounds == null) {
                    fViewBounds = fGraphBounds;
                    return;
                }

                ArborPoint vLT = fGraphBounds.LeftTop.Sub(fViewBounds.LeftTop).Mul(Mag);
                ArborPoint vRB = fGraphBounds.RightBottom.Sub(fViewBounds.RightBottom).Mul(Mag);

                double aX = vLT.Magnitude() * fViewWidth;
                double aY = vRB.Magnitude() * fViewHeight;

                if (aX > 1.0f || aY > 1.0f) {
                    ArborPoint nbLT = fViewBounds.LeftTop.Add(vLT);
                    ArborPoint nbRB = fViewBounds.RightBottom.Add(vRB);

                    fViewBounds = new PSBounds(nbLT, nbRB);
                }
            } catch (Exception ex) {
                Debug.WriteLine("ArborSystem.UpdateViewBounds(): " + ex.Message);
            }
        }

        protected void TickTimer()
        {
            if (fBusy) return;
            fBusy = true;

            try {
                UpdatePhysics();
                UpdateViewBounds();

                if (fRenderer != null) {
                    fRenderer.Invalidate();
                }

                if (fAutoStop) {
                    if (fEnergyMean <= fStopThreshold) {
                        if (fPrevTime == DateTime.FromBinary(0)) {
                            fPrevTime = DateTime.Now;
                        }
                        TimeSpan ts = DateTime.Now - fPrevTime;
                        if (ts.TotalMilliseconds > 1000.0f) {
                            Stop();
                        }
                    } else {
                        fPrevTime = DateTime.FromBinary(0);
                    }
                }
            } catch (Exception ex) {
                Debug.WriteLine("ArborSystem.TickTimer(): " + ex.Message);
            }

            fBusy = false;
        }

        private void UpdatePhysics()
        {
            try {
                // tend particles
                for (int i = 0, nodesCount = fNodes.Count; i < nodesCount; i++) {
                    ArborPoint pV = fNodes[i].V;
                    pV.X = 0;
                    pV.Y = 0;
                }

                if (fStiffness > 0.0f) {
                    ApplySprings();
                }

                // euler integrator
                if (fRepulsion > 0.0f) {
                    ApplyBarnesHutRepulsion();
                }

                UpdateVelocityAndPosition(Dt);
            } catch (Exception ex) {
                Debug.WriteLine("ArborSystem.UpdatePhysics(): " + ex.Message);
            }
        }

        private void ApplyBarnesHutRepulsion()
        {
            BarnesHutTree bht = new BarnesHutTree(fGraphBounds.LeftTop, fGraphBounds.RightBottom, Theta);

            int nodesCount = fNodes.Count;
            for (int i = 0; i < nodesCount; i++) {
                ArborNode node = fNodes[i];
                bht.Insert(node);
            }

            for (int i = 0; i < nodesCount; i++) {
                ArborNode node = fNodes[i];
                bht.ApplyForces(node, fRepulsion);
            }
        }

        private void ApplySprings()
        {
            for (int i = 0, edgesCount = fEdges.Count; i < edgesCount; i++) {
                ArborEdge edge = fEdges[i];

                ArborPoint s = edge.Target.Pt.Sub(edge.Source.Pt);
                double sMag = s.Magnitude();

                ArborPoint r = ((sMag > 0.0f) ? s : ArborPoint.NewRandom(1.0f)).Normalize();
                double q = edge.Stiffness * (edge.Length - sMag);

                edge.Source.ApplyForce(r.Mul(q * -0.5f));
                edge.Target.ApplyForce(r.Mul(q * 0.5f));
            }
        }

        private void UpdateVelocityAndPosition(double dt)
        {
            int nodesCount = fNodes.Count;
            if (nodesCount == 0) {
                fEnergyMax = 0.0f;
                fEnergyMean = 0.0f;
                fEnergySum = 0.0f;
                return;
            }

            double eMax = 0.0f;
            double eSum = 0.0f;

            // calc center drift
            ArborPoint rr = ArborPoint.Zero;
            for (int i = 0; i < nodesCount; i++) {
                ArborNode node = fNodes[i];
                rr = rr.Sub(node.Pt);
            }
            ArborPoint drift = rr.Div(nodesCount);

            // main updates loop
            for (int i = 0; i < nodesCount; i++) {
                ArborNode node = fNodes[i];

                // apply center drift
                node.ApplyForce(drift);

                // apply center gravity
                if (fGravity) {
                    ArborPoint q = node.Pt.Mul(-1.0f);
                    node.ApplyForce(q.Mul(fRepulsion / 100.0f));
                }

                // update velocities
                if (node.Fixed) {
                    node.V = ArborPoint.Zero;
                } else {
                    node.V = node.V.Add(node.F.Mul(dt));
                    node.V = node.V.Mul(1.0f - fFriction);

                    double r = node.V.MagnitudeSquare();
                    if (r > 1000000.0f) {
                        node.V = node.V.Div(r);
                    }
                }

                node.F.X = node.F.Y = 0.0f;

                // update positions
                node.Pt = node.Pt.Add(node.V.Mul(dt));

                // update energy
                double energy = node.V.MagnitudeSquare();
                eSum += energy;
                eMax = Math.Max(energy, eMax);
            }

            fEnergyMax = eMax;
            fEnergyMean = eSum / nodesCount;
            fEnergySum = eSum;
        }

        internal static double GetRndDouble()
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
