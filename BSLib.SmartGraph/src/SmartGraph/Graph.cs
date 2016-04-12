/*
 *  "SmartGraph", the small library for store and manipulations over graphs.
 *  Copyright (C) 2011-2016 by Serg V. Zhdanovskih (aka Alchemist, aka Norseman).
 *
 *  This file is part of "GEDKeeper".
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;

namespace BSLib.SmartGraph
{
	[Serializable]
	public class GraphException : Exception
	{
		public GraphException()
		{
		}
		public GraphException(string message) : base(message)
		{
		}
	}

	public enum GraphNotification
	{
		Added,
		Deleted,
		Cleared
	}

	/// <summary>
	/// 
	/// </summary>
	public class NotifyEventArgs : EventArgs
	{
		public GraphNotification Notification { get; private set; }
		public object ItemData { get; set; }

		public NotifyEventArgs(GraphNotification notification, object itemData)
		{
			this.Notification = notification;
			this.ItemData = itemData;
		}
	}

	public delegate void NotifyEventHandler(object sender, NotifyEventArgs e);

	/// <summary>
    /// 
    /// </summary>
    public class Graph : BaseObject
	{
		#region Private members

		private readonly List<Edge> fEdgesList;
		private readonly List<Vertex> fVerticesList;
		private readonly Dictionary<string, Vertex> fVerticesDictionary;
		private NotifyEventHandler fOnChange;
		private NotifyEventHandler fOnChanging;
		private int fUpdateCount;

		#endregion

		#region Properties

		public IEnumerable<Vertex> Vertices
		{
			get { return this.fVerticesList; }
		}

		public IEnumerable<Edge> Edges
		{
			get { return this.fEdgesList; }
		}

		public event NotifyEventHandler OnChange
		{
			add { this.fOnChange = value; }
			remove { if (this.fOnChange == value) this.fOnChange = null; }
		}

		public event NotifyEventHandler OnChanging
		{
			add { this.fOnChanging = value; }
			remove { if (this.fOnChanging == value) this.fOnChanging = null; }
		}

		#endregion

		#region Instance control

		public Graph()
		{
			this.fVerticesList = new List<Vertex>();
			this.fEdgesList = new List<Edge>();
			this.fVerticesDictionary = new Dictionary<string, Vertex>();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Clear();
			}
			base.Dispose(disposing);
		}

		#endregion

		#region Data management

		public bool IsEmpty()
		{
			return (this.fEdgesList.Count == 0 && this.fVerticesList.Count == 0);
		}

		public void Clear()
		{
			foreach (Vertex vertex in this.fVerticesList)
			{
				vertex.EdgeIn = null;
			}

			this.fEdgesList.Clear();
			this.fVerticesList.Clear();
			this.fVerticesDictionary.Clear();
			this.Notify(null, GraphNotification.Cleared);
		}

		private void Notify(object instance, GraphNotification action)
		{
		}

		public Vertex AddVertex(string sign, object data = null)
		{
			if (string.IsNullOrEmpty(sign))
				throw new ArgumentNullException("sign");

			Vertex result = new Vertex();
			result.Sign = sign;
			result.Value = data;

			this.fVerticesList.Add(result);
			this.fVerticesDictionary.Add(sign, result);
			this.Notify(result, GraphNotification.Added);

			return result;
		}

		public bool AddUndirectedEdge(Vertex source, Vertex target, int cost, object srcValue, object tgtValue)
		{
			Edge edge1 = this.AddDirectedEdge(source, target, cost, srcValue);
			Edge edge2 = this.AddDirectedEdge(target, source, cost, tgtValue);

			return (edge1 != null && edge2 != null);
		}

		public Edge AddDirectedEdge(string sourceSign, string targetSign, bool canCreate = true)
		{
			return this.AddDirectedEdge(sourceSign, targetSign, 0, null, canCreate);
		}

		public Edge AddDirectedEdge(string sourceSign, string targetSign, int cost, object edgeValue, bool canCreate = true)
		{
			Vertex source = this.FindVertex(sourceSign);
			Vertex target = this.FindVertex(targetSign);

			if (source == null && canCreate) source = this.AddVertex(sourceSign);
			if (target == null && canCreate) target = this.AddVertex(targetSign);

			return this.AddDirectedEdge(source, target, cost, edgeValue);
		}

		public Edge AddDirectedEdge(Vertex source, Vertex target, int cost, object edgeValue)
		{
			if (source == null || target == null || source == target) return null;

			Edge resultEdge = new Edge(source, target, cost, edgeValue);

			this.fEdgesList.Add(resultEdge);
			this.Notify(resultEdge, GraphNotification.Added);

			return resultEdge;
		}

		public void DeleteVertex(Vertex vertex)
		{
			if (vertex == null) return;

			for (int i = this.fEdgesList.Count - 1; i >= 0; i--)
			{
				Edge edge = this.fEdgesList[i];

				if (edge.Source == vertex || edge.Target == vertex)
				{
					this.DeleteEdge(edge);
				}				
			}

			this.fVerticesList.Remove(vertex);
			this.Notify(vertex, GraphNotification.Deleted);
		}

		public void DeleteEdge(Edge edge)
		{
			if (edge == null) return;

			this.fEdgesList.Remove(edge);
			this.Notify(edge, GraphNotification.Deleted);
		}

		public Vertex FindVertex(string sign)
		{
			Vertex result;
			this.fVerticesDictionary.TryGetValue(sign, out result);
			return result;
		}

		public IEnumerable<Edge> GetEdgesOut(Vertex source)
		{
			for (int i = 0; i < this.fEdgesList.Count; i++)
			{
				Edge edge = this.fEdgesList[i];

				if (edge.Source == source)
				{
					yield return edge;
				}
			}

			yield break;
		}

		#endregion

		#region Updating

		private void SetUpdateState(bool updating)
		{
			if (updating)
			{
				this.Changing();
			}
			else
			{
				this.Changed();
			}
		}

		public void BeginUpdate()
		{
			if (this.fUpdateCount == 0)
			{
				this.SetUpdateState(true);
			}
			this.fUpdateCount++;
		}

		public void EndUpdate()
		{
			this.fUpdateCount--;
			if (this.fUpdateCount == 0)
			{
				this.SetUpdateState(false);
			}
		}

		private void Changed()
		{
			if (this.fUpdateCount == 0 && this.fOnChange != null)
			{
				this.fOnChange(this, null);
			}
		}

		private void Changing()
		{
			if (this.fUpdateCount == 0 && this.fOnChanging != null)
			{
				this.fOnChanging(this, null);
			}
		}

		#endregion

		#region Pathes search

		public void FindPathTree(Vertex root)
		{
			if (root == null) return;

			// reset path tree
			foreach (Vertex node in this.fVerticesList)
			{
				node.Dist = int.MaxValue;
				node.Visited = false;
				node.EdgeIn = null;
			}

			// init root
			root.Dist = 0;
			root.Visited = true;
			root.EdgeIn = null;

			PathCandidate topCandidate = new PathCandidate(root, null);

			// processing
			while (topCandidate != null)
			{
				Vertex topNode = topCandidate.Node;
				topCandidate = topCandidate.Next;

				int nodeDist = topNode.Dist;
				topNode.Visited = false;

				foreach (Edge link in GetEdgesOut(topNode))
				{
                    Vertex target = link.Target;
                    int newDist = nodeDist + link.Cost;

					if (newDist < target.Dist)
					{
						target.Dist = newDist;
						target.EdgeIn = link;

						if (!target.Visited)
						{
							target.Visited = true;
							topCandidate = new PathCandidate(target, topCandidate);
						}
					}
				}
			}
		}

		public IEnumerable<Edge> GetPath(Vertex target)
		{
			List<Edge> result = new List<Edge>();

			if (target != null)
			{
				Edge edge = target.EdgeIn;
				while (edge != null)
				{
					result.Insert(0, edge);
					edge = edge.Source.EdgeIn;
				}
			}

			return result;
		}

		#endregion
	}
}
