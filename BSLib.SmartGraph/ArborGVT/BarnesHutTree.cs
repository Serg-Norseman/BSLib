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

namespace BSLib.ArborGVT
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
        private const int QNe = 0;
        private const int QNw = 1;
        private const int QSe = 2;
        private const int QSw = 3;
        private const int QNone = 4;

        private readonly double fDist; // default = 0.5
        private readonly Branch fRoot;

        public BarnesHutTree(ArborPoint lt, ArborPoint rb, double dist)
        {
            fDist = dist;
            //fDist = dist * dist;
            fRoot = new Branch(lt, rb.Sub(lt));
        }

        private static int GetQuad(ArborNode node, Branch branch)
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

                            branch = fp as Branch;

                            gst.Insert(0, h);
                        } else {
                            ArborPoint l = branch.Size.Div(2);
                            ArborPoint n = new ArborPoint(branch.Origin.X, branch.Origin.Y);

                            if (qd == QSe || qd == QSw) {
                                n.Y += l.Y;
                            }
                            if (qd == QNe || qd == QSe) {
                                n.X += l.X;
                            }

                            ArborNode o = fp as ArborNode;
                            fp = new Branch(n, l);
                            branch.Q[qd] = fp;

                            branch.Mass = m;
                            branch.Pt = h.Pt.Mul(m);

                            branch = fp as Branch;

                            if (o.Pt.X == h.Pt.X && o.Pt.Y == h.Pt.Y) {
                                double k = l.X * 0.08f;
                                double i = l.Y * 0.08f;
                                o.Pt.X = Math.Min(n.X + l.X, Math.Max(n.X, o.Pt.X - k / 2 + ArborSystem.GetRndDouble() * k));
                                o.Pt.Y = Math.Min(n.Y + l.Y, Math.Max(n.Y, o.Pt.Y - i / 2 + ArborSystem.GetRndDouble() * i));
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

                    ArborPoint ptx, i, k;
                    double l, kMag, massx;

                    if (obj is ArborNode) {
                        ArborNode node = (obj as ArborNode);
                        massx = node.Mass;
                        ptx = node.Pt;

                        k = m.Pt.Sub(ptx);
                        kMag = k.Magnitude();
                        //kMag = k.magnitudeSquare();

                        l = Math.Max(1.0f, kMag);
                        i = ((kMag > 0.0f) ? k : ArborPoint.NewRandom(1.0f)).Normalize();
                        m.ApplyForce(i.Mul(repulsion * massx).Div(l * l));
                        //m.applyForce(i.mul(repulsion * massx).div(l));
                    } else {
                        Branch branch = (obj as Branch);
                        massx = branch.Mass;
                        ptx = branch.Pt.Div(massx);

                        k = m.Pt.Sub(ptx);
                        kMag = k.Magnitude();
                        //kMag = k.magnitudeSquare();

                        double h = Math.Sqrt(branch.Size.X * branch.Size.Y);
                        //double h = branch.Size.X * branch.Size.Y;
                        if (h / kMag > fDist) {
                            queue.Enqueue(branch.Q[QNe]);
                            queue.Enqueue(branch.Q[QNw]);
                            queue.Enqueue(branch.Q[QSe]);
                            queue.Enqueue(branch.Q[QSw]);
                        } else {
                            l = Math.Max(1.0f, kMag);
                            i = ((kMag > 0.0f) ? k : ArborPoint.NewRandom(1.0f)).Normalize();
                            m.ApplyForce(i.Mul(repulsion * massx).Div(l * l));
                            //m.applyForce(i.mul(repulsion * massx).div(l));
                        }
                    }
                }
            } catch (Exception ex) {
                Debug.WriteLine("BarnesHutTree.ApplyForces(): " + ex.Message);
            }
        }
    }
}
