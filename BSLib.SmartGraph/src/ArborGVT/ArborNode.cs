/*
 *  ArborGVT - a graph vizualization toolkit
 *
 *  Physics code derived from springy.js, copyright (c) 2010 Dennis Hotson
 *  JavaScript library, copyright (c) 2011 Samizdat Drafting Co.
 *
 *  Fork and C# implementation, copyright (c) 2012,2016 by Serg V. Zhdanovskih.
 */

#if !NETSTANDARD1_6
using System.Drawing;
#endif

using BSLib.Extensions;
using BSLib.SmartGraph;

namespace BSLib.ArborGVT
{
    public class ArborNode : IExtension<GraphObject>
    {
        private GraphObject fOwner;

        public string Sign;
        public object Data;

        public bool Fixed;
        public double Mass;
        public ArborPoint Pt;

        #if !NETSTANDARD1_6
        public Color Color;
        public RectangleF Box;
        #endif

        internal ArborPoint V;
        internal ArborPoint F;

        public ArborNode(string sign)
        {
            Sign = sign;

            Fixed = false;
            Mass = 1;
            Pt = ArborPoint.Null;

            #if !NETSTANDARD1_6
            Color = Color.Gray;
            #endif

            V = new ArborPoint(0, 0);
            F = new ArborPoint(0, 0);
        }

        internal void applyForce(ArborPoint a)
        {
            F = F.add(a.div(Mass));
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
