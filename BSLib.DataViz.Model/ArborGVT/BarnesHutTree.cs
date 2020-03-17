/*
 *  ArborGVT - a graph vizualization toolkit
 *
 *  Physics code derived from springy.js, copyright (c) 2010 Dennis Hotson
 *  JavaScript library, copyright (c) 2011 Samizdat Drafting Co.
 *
 *  Fork and C# implementation, copyright (c) 2012,2016 by Serg V. Zhdanovskih.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BSLib.DataViz.ArborGVT
{
    internal class Branch
    {
        public ArborPoint Origin;
        public ArborPoint Size;
        public object[] Q;

        public double Mass;
        public ArborPoint Pt;

        public Branch(ArborPoint origin, ArborPoint size)
        {
            Origin = origin;
            Size = size;
            Q = new object[4] { null, null, null, null };
            Mass = 0.0f;
            Pt = ArborPoint.Zero;
        }
    }

    internal class BarnesHutTree
    {
        internal const int QNe = 0;
        internal const int QNw = 1;
        internal const int QSe = 2;
        internal const int QSw = 3;
        internal const int QNone = 4;

        private readonly double fDist; // default = 0.5
        private readonly Branch fRoot;

        internal Branch Root
        {
            get { return fRoot; }
        }

        public BarnesHutTree(ArborPoint lt, ArborPoint rb, double dist)
        {
            fDist = dist;
            fRoot = new Branch(lt, rb.Sub(lt));
        }

        public void Reset()
        {
            fRoot.Q = new object[4] { null, null, null, null };
            fRoot.Mass = 0.0f;
            fRoot.Pt = ArborPoint.Zero;
        }

        internal static int GetQuad(ArborNode node, Branch branch)
        {
            try {
                if (node.Pt.IsExploded()) {
                    return QNone;
                }
                ArborPoint h = node.Pt.Sub(branch.Origin);
                ArborPoint g = branch.Size.Div(2.0f);
                if (h.Y < g.Y) {
                    return (h.X < g.X) ? QNw : QNe;
                } else {
                    return (h.X < g.X) ? QSw : QSe;
                }
            } catch (Exception ex) {
                Debug.WriteLine("BarnesHutTree.GetQuad(): " + ex.Message);
                return QNone;
            }
        }

        public void Insert(ArborNode node)
        {
            try {
                Branch branch = fRoot;
                List<ArborNode> gst = new List<ArborNode>();
                gst.Add(node);
                while (gst.Count > 0) {
                    ArborNode h = gst[0];
                    gst.RemoveAt(0);

                    double m = h.Mass;
                    int qd = GetQuad(h, branch);
                    object fp = branch.Q[qd];

                    if (fp == null) {
                        branch.Q[qd] = h;

                        branch.Mass += m;
                        branch.Pt = branch.Pt.Add(h.Pt.Mul(m));
                    } else {
                        if (fp is Branch) {
                            branch.Mass += m;
                            branch.Pt = branch.Pt.Add(h.Pt.Mul(m));

                            branch = (Branch)fp;

                            gst.Insert(0, h);
                        } else {
                            ArborPoint l = branch.Size.Div(2);
                            ArborPoint n = branch.Origin;

                            if (qd == QSe || qd == QSw) {
                                n.Y += l.Y;
                            }
                            if (qd == QNe || qd == QSe) {
                                n.X += l.X;
                            }

                            ArborNode o = (ArborNode)fp;
                            fp = new Branch(n, l);
                            branch.Q[qd] = fp;

                            branch.Mass = m;
                            branch.Pt = h.Pt.Mul(m);

                            branch = (Branch)fp;

                            ArborPoint oPt = o.Pt;
                            if (oPt.X == h.Pt.X && oPt.Y == h.Pt.Y) {
                                double lX = l.X * 0.08f;
                                double lY = l.Y * 0.08f;
                                oPt.X = Math.Min(n.X + l.X, Math.Max(n.X, oPt.X - lX / 2 + ArborSystem.GetRandom() * lX));
                                oPt.Y = Math.Min(n.Y + l.Y, Math.Max(n.Y, oPt.Y - lY / 2 + ArborSystem.GetRandom() * lY));
                                o.Pt = oPt;
                            }

                            gst.Add(o);
                            gst.Insert(0, h);
                        }
                    }
                }
            } catch (Exception ex) {
                Debug.WriteLine("BarnesHutTree.Insert(): " + ex.Message);
            }
        }

        public void ApplyForces(ArborNode m, double repulsion)
        {
            try {
                Queue<object> queue = new Queue<object>();

                queue.Enqueue(fRoot);
                while (queue.Count > 0) {
                    object obj = queue.Dequeue();
                    if (obj == null || obj == m) continue;

                    ArborPoint ptx, k;
                    double l, kMag, massx;

                    if (obj is ArborNode) {
                        ArborNode node = (ArborNode)obj;
                        massx = node.Mass;
                        ptx = node.Pt;

                        k = m.Pt.Sub(ptx);
                        kMag = k.Magnitude();

                        l = Math.Max(1.0f, kMag);
                        m.ApplyForce(k.Normalize().Mul(repulsion * massx).Div(l * l));
                    } else {
                        Branch branch = (Branch)obj;
                        massx = branch.Mass;
                        ptx = branch.Pt.Div(massx);

                        k = m.Pt.Sub(ptx);
                        kMag = k.Magnitude();

                        double h = Math.Sqrt(branch.Size.X * branch.Size.Y);
                        if (h / kMag > fDist) {
                            queue.Enqueue(branch.Q[QNe]);
                            queue.Enqueue(branch.Q[QNw]);
                            queue.Enqueue(branch.Q[QSe]);
                            queue.Enqueue(branch.Q[QSw]);
                        } else {
                            l = Math.Max(1.0f, kMag);
                            m.ApplyForce(k.Normalize().Mul(repulsion * massx).Div(l * l));
                        }
                    }
                }
            } catch (Exception ex) {
                Debug.WriteLine("BarnesHutTree.ApplyForces(): " + ex.Message);
            }
        }
    }
}
