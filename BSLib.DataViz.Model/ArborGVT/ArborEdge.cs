﻿/*
 *  ArborGVT - a graph vizualization toolkit
 *
 *  Physics code derived from springy.js, copyright (c) 2010 Dennis Hotson
 *  JavaScript library, copyright (c) 2011 Samizdat Drafting Co.
 *
 *  Fork and C# implementation, copyright (c) 2012,2016 by Serg V. Zhdanovskih.
 */

using System;
using BSLib.DataViz.SmartGraph;
using BSLib.Extensions;

namespace BSLib.DataViz.ArborGVT
{
    public class ArborEdge : IExtension<GraphObject>
    {
        private GraphObject fOwner;

        public ArborNode Source;
        public ArborNode Target;

        public double Length;
        public double Stiffness;
        public bool Directed;

        public ArborEdge(ArborNode source, ArborNode target, double length, double stiffness, bool directed = false)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (target == null)
                throw new ArgumentNullException("target");

            Source = source;
            Target = target;
            Length = length;
            Stiffness = stiffness;
            Directed = directed;
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
