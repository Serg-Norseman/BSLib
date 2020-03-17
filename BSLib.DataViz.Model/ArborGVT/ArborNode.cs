/*
 *  ArborGVT - a graph vizualization toolkit
 *
 *  Physics code derived from springy.js, copyright (c) 2010 Dennis Hotson
 *  JavaScript library, copyright (c) 2011 Samizdat Drafting Co.
 *
 *  Fork and C# implementation, copyright (c) 2012,2016 by Serg V. Zhdanovskih.
 */

using BSLib.DataViz.SmartGraph;
using BSLib.Extensions;

namespace BSLib.DataViz.ArborGVT
{
    public class ArborNode : IExtension<GraphObject>
    {
        private GraphObject fOwner;

        public string Sign;
        public object Data;

        public bool Fixed;
        public double Mass;
        public ArborPoint Pt;

        internal ArborPoint V;
        internal ArborPoint F;

        public ArborNode(string sign)
        {
            Sign = sign;

            Fixed = false;
            Mass = 1.0f;
            Pt = ArborPoint.Null;

            V = ArborPoint.Zero;
            F = ArborPoint.Zero;
        }

        internal void ApplyForce(ArborPoint a)
        {
            F = F.Add(a.Div(Mass));
        }

        public void Attach(GraphObject owner)
        {
            fOwner = owner;
        }

        public void Detach(GraphObject owner)
        {
            fOwner = null;
        }
    }
}
