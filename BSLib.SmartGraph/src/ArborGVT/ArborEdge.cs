/*
 *  ArborGVT - a graph vizualization toolkit
 *
 *  Physics code derived from springy.js, copyright (c) 2010 Dennis Hotson
 *  JavaScript library, copyright (c) 2011 Samizdat Drafting Co.
 *
 *  Fork and C# implementation, copyright (c) 2012,2016 by Serg V. Zhdanovskih.
 */

using BSLib.Extensions;
using BSLib.SmartGraph;

namespace BSLib.ArborGVT
{
    public class ArborEdge : IExtension<GraphObject>
    {
    	private GraphObject fOwner;

        public ArborNode Source;
        public ArborNode Target;

        public double Length;
        public double Stiffness;
        public bool Directed;

        public ArborEdge(ArborNode src, ArborNode tgt, double len, double stiffness)
        {
            this.Source = src;
            this.Target = tgt;
            this.Length = len;
            this.Stiffness = stiffness;
        }

        public ArborEdge(ArborNode src, ArborNode tgt, double len, double stiffness, bool directed)
            : this(src, tgt, len, stiffness)
        {
            this.Directed = directed;
        }

		public void Attach(GraphObject owner)
		{
			this.fOwner = owner;
		}

		public void Detach(GraphObject owner)
		{
			this.fOwner = null;
		}
    }
}
