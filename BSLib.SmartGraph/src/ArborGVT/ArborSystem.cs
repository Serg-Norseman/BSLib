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
using System.Timers;
using BSLib.SmartGraph;

namespace ArborGVT
{
    internal class PSBounds
    {
        public ArborPoint LeftTop = ArborPoint.Null;
        public ArborPoint RightBottom = ArborPoint.Null;

        public PSBounds(ArborPoint leftTop, ArborPoint rightBottom)
        {
            this.LeftTop = leftTop;
            this.RightBottom = rightBottom;
        }
    }

    public sealed class ArborSystem : IDisposable
    {
        private static readonly int DEBUG_PROFILER_LIMIT = 2000;

        private static readonly Random _random = new Random();

        private readonly int[] margins = new int[4] { 20, 20, 20, 20 };
        private const double Mag = 0.04;

        private bool fAutoStop;
        private bool fBusy;
        private bool fDisposed;
        private readonly List<ArborEdge> fEdges;
        private Graph fGraph;
        private PSBounds fGraphBounds;
        private int fIterationsCounter;
        private readonly Hashtable fNames;
        private readonly List<ArborNode> fNodes;
        private EventHandler fOnStart;
        private EventHandler fOnStop;
        private readonly IArborRenderer fRenderer;
        private DateTime fPrevTime;
        private int fScreenHeight;
        private int fScreenWidth;
        private double fStopThreshold;
        private Timer fTimer;
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
            get { return this.fAutoStop; }
            set { this.fAutoStop = value; }
        }

        public List<ArborNode> Nodes
        {
            get { return this.fNodes; }
        }

        public List<ArborEdge> Edges
        {
            get { return this.fEdges; }
        }

        public event EventHandler OnStart
        {
            add
            {
                this.fOnStart = value;
            }
            remove
            {
                if (this.fOnStart == value)
                {
                    this.fOnStart = null;
                }
            }
        }

        public event EventHandler OnStop
        {
            add
            {
                this.fOnStop = value;
            }
            remove
            {
                if (this.fOnStop == value)
                {
                    this.fOnStop = null;
                }
            }
        }

        public double StopThreshold
        {
            get { return this.fStopThreshold; }
            set { this.fStopThreshold = value; }
        }

        #endregion

        public ArborSystem(Graph graph, double repulsion, double stiffness, double friction, IArborRenderer renderer)
        {
        	this.fGraph = graph;

            this.fAutoStop = true;
            this.fBusy = false;
            this.fNames = new Hashtable();
            this.fNodes = new List<ArborNode>();
            this.fEdges = new List<ArborEdge>();
            this.fRenderer = renderer;
            this.fPrevTime = DateTime.FromBinary(0);
            this.fStopThreshold = /*0.05*/ 0.7;
            this.fTimer = null;

            this.ParamRepulsion = repulsion;
            this.ParamStiffness = stiffness;
            this.ParamFriction = friction;
        }

        public void Dispose()
        {
            if (!this.fDisposed)
            {
                this.stop();
                this.fDisposed = true;
            }
        }

        private void prepareGraph()
        {
        	foreach (IVertex vertex in this.fGraph.Vertices) {
        		ArborNode node = new ArborNode(vertex.Sign);
        		((Vertex)vertex).Extensions.Add(node);
        		
        		this.resetCoords(node);
        		this.fNodes.Add(node);
        	}

        	foreach (IEdge edge in this.fGraph.Edges) {
        		Vertex vtxSrc = (Vertex)edge.Source;
        		Vertex vtxTgt = (Vertex)edge.Target;
        		ArborNode anSrc = (ArborNode)vtxSrc.Extensions.Find<ArborNode>();
        		ArborNode anTgt = (ArborNode)vtxTgt.Extensions.Find<ArborNode>();

        		ArborEdge arbEdge = new ArborEdge(anSrc, anTgt, 1, ParamStiffness);
        		((Edge)edge).Extensions.Add(arbEdge);
        		this.fEdges.Add(arbEdge);
        	}
        }

        public void start()
        {
        	this.prepareGraph();

            if (fOnStart != null) fOnStart(this, new EventArgs());

            if (fTimer != null)
            {
                return;
            }
            fPrevTime = DateTime.FromBinary(0);

            fIterationsCounter = 0;

            fTimer = new System.Timers.Timer();
            fTimer.AutoReset = true;
            fTimer.Interval = ParamTimeout;
            fTimer.Elapsed += this.tickTimer;
            fTimer.Start();
        }

        public void stop()
        {
            if (fTimer != null)
            {
                fTimer.Stop();
                fTimer.Dispose();
                fTimer = null;
            }

            if (fOnStop != null) fOnStop(this, new EventArgs());
        }

        public ArborNode addNode(string sign, double x, double y)
        {
            ArborNode node = this.getNode(sign);
            if (node != null) return node;

            node = new ArborNode(sign);
            node.Pt = new ArborPoint(x, y);

            fNames.Add(sign, node);
            fNodes.Add(node);

            return node;
        }

        public ArborNode addNode(string sign)
        {
            ArborPoint lt = this.fGraphBounds.LeftTop;
            ArborPoint rb = this.fGraphBounds.RightBottom;
            double xx = lt.X + (rb.X - lt.X) * ArborSystem.NextRndDouble();
            double yy = lt.Y + (rb.Y - lt.Y) * ArborSystem.NextRndDouble();

            return this.addNode(sign, xx, yy);
        }

        private void resetCoords(ArborNode node)
        {
            ArborPoint lt = this.fGraphBounds.LeftTop;
            ArborPoint rb = this.fGraphBounds.RightBottom;
            double xx = lt.X + (rb.X - lt.X) * ArborSystem.NextRndDouble();
            double yy = lt.Y + (rb.Y - lt.Y) * ArborSystem.NextRndDouble();

        	node.Pt = new ArborPoint(xx, yy);
        }
        
        public ArborNode getNode(string sign)
        {
            return (ArborNode)fNames[sign];
        }

        public ArborEdge addEdge(string srcSign, string tgtSign, double len = 1.0)
        {
            ArborNode src = this.getNode(srcSign);
            src = (src != null) ? src : this.addNode(srcSign);

            ArborNode tgt = this.getNode(tgtSign);
            tgt = (tgt != null) ? tgt : this.addNode(tgtSign);

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
                x = new ArborEdge(src, tgt, len, ParamStiffness);
                fEdges.Add(x);
            }

            return x;
        }

        public void setScreenSize(int width, int height)
        {
            this.fScreenWidth = width;
            this.fScreenHeight = height;
            this.updateViewBounds();
        }

        public ArborPoint toScreen(ArborPoint pt)
        {
            if (fViewBounds == null) return ArborPoint.Null;

            ArborPoint vd = fViewBounds.RightBottom.sub(fViewBounds.LeftTop);
            double sx = margins[3] + pt.sub(fViewBounds.LeftTop).div(vd.X).X * (this.fScreenWidth - (margins[1] + margins[3]));
            double sy = margins[0] + pt.sub(fViewBounds.LeftTop).div(vd.Y).Y * (this.fScreenHeight - (margins[0] + margins[2]));
            return new ArborPoint(sx, sy);
        }

        public ArborPoint fromScreen(double sx, double sy)
        {
            if (fViewBounds == null) return ArborPoint.Null;

            ArborPoint vd = fViewBounds.RightBottom.sub(fViewBounds.LeftTop);
            double x = (sx - margins[3]) / (this.fScreenWidth - (margins[1] + margins[3])) * vd.X + fViewBounds.LeftTop.X;
            double y = (sy - margins[0]) / (this.fScreenHeight - (margins[0] + margins[2])) * vd.Y + fViewBounds.LeftTop.Y;
            return new ArborPoint(x, y);
        }

        public ArborNode nearest(int sx, int sy)
        {
            ArborPoint x = this.fromScreen(sx, sy);

            ArborNode resNode = null;
            double minDist = +1.0;

            foreach (ArborNode node in this.fNodes)
            {
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

            //minDist = this.toScreen(resNode.Pt).sub(this.toScreen(x)).magnitude();
            return resNode;
        }

        private void updateGraphBounds()
        {
            ArborPoint lt = new ArborPoint(-1, -1);
            ArborPoint rb = new ArborPoint(1, 1);

            foreach (ArborNode node in this.fNodes)
            {
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

            this.fGraphBounds = new PSBounds(cent.sub(d), cent.add(d));
        }

        private void updateViewBounds()
        {
            try
            {
                this.updateGraphBounds();

                if (fViewBounds == null)
                {
                    fViewBounds = fGraphBounds;
                    return;
                }

                ArborPoint vLT = fGraphBounds.LeftTop.sub(fViewBounds.LeftTop).mul(Mag);
                ArborPoint vRB = fGraphBounds.RightBottom.sub(fViewBounds.RightBottom).mul(Mag);

                double aX = vLT.magnitude() * this.fScreenWidth;
                double aY = vRB.magnitude() * this.fScreenHeight;

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

        private void tickTimer(object sender, ElapsedEventArgs e)
        {
            if (DEBUG_PROFILER_LIMIT > 0)
            {
                if (fIterationsCounter >= DEBUG_PROFILER_LIMIT)
                {
                    return;
                }
                else
                {
                    fIterationsCounter++;
                }
            }

            if (this.fBusy) return;
            this.fBusy = true;
            try
            {
                this.updatePhysics();
                this.updateViewBounds();

                if (fRenderer != null)
                {
                    fRenderer.Invalidate();
                }

                if (this.fAutoStop)
                {
                    if (EnergyMean <= this.fStopThreshold)
                    {
                        if (fPrevTime == DateTime.FromBinary(0))
                        {
                            fPrevTime = DateTime.Now;
                        }
                        TimeSpan ts = DateTime.Now - fPrevTime;
                        if (ts.TotalMilliseconds > 1000)
                        {
                            this.stop();
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
            this.fBusy = false;
        }

        private void updatePhysics()
        {
            try
            {
                // tend particles
                foreach (ArborNode p in fNodes)
                {
                    p.V.X = 0;
                    p.V.Y = 0;
                }

                if (ParamStiffness > 0)
                {
                    this.applySprings();
                }

                // euler integrator
                if (ParamRepulsion > 0)
                {
                    this.applyBarnesHutRepulsion();
                }

                this.updateVelocityAndPosition(ParamDt);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ArborSystem.updatePhysics(): " + ex.Message);
            }
        }

        private void applyBarnesHutRepulsion()
        {
            BarnesHutTree bht = new BarnesHutTree(fGraphBounds.LeftTop, fGraphBounds.RightBottom, ParamTheta);

            foreach (ArborNode node in fNodes)
            {
                bht.insert(node);
            }

            foreach (ArborNode node in fNodes)
            {
                bht.applyForces(node, ParamRepulsion);
            }
        }

        private void applySprings()
        {
            foreach (ArborEdge edge in fEdges)
            {
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
            ArborPoint rr = new ArborPoint(0, 0);
            foreach (ArborNode node in fNodes)
            {
                rr = rr.sub(node.Pt);
            }
            ArborPoint drift = rr.div(size);

            // main updates loop
            foreach (IVertex vertex in fGraph.Vertices)//(ArborNode node in fNodes)
            {
            	ArborNode node = (ArborNode)((Vertex)vertex).Extensions.Find<ArborNode>();

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
                    node.V = new ArborPoint(0, 0);
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

        internal static double NextRndDouble()
        {
            return _random.NextDouble();
        }
    }
}
