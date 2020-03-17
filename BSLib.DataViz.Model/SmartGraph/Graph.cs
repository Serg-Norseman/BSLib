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

namespace BSLib.DataViz.SmartGraph
{
    [Serializable]
    public class GraphException : Exception
    {
        public GraphException()
        {
        }
        public GraphException(string message)
            : base(message)
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
            Notification = notification;
            ItemData = itemData;
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

        public IList<Vertex> Vertices
        {
            get { return fVerticesList; }
        }

        public IList<Edge> Edges
        {
            get { return fEdgesList; }
        }

        public event NotifyEventHandler OnChange
        {
            add {
                fOnChange = value;
            }
            remove {
                if (fOnChange == value)
                    fOnChange = null;
            }
        }

        public event NotifyEventHandler OnChanging
        {
            add {
                fOnChanging = value;
            }
            remove {
                if (fOnChanging == value)
                    fOnChanging = null;
            }
        }

        #endregion

        #region Instance control

        public Graph()
        {
            fVerticesList = new List<Vertex>();
            fEdgesList = new List<Edge>();
            fVerticesDictionary = new Dictionary<string, Vertex>();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                Clear();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Data management

        public bool IsEmpty()
        {
            return (fEdgesList.Count == 0 && fVerticesList.Count == 0);
        }

        public void Clear()
        {
            for (int i = 0, verticesCount = fVerticesList.Count; i < verticesCount; i++) {
                Vertex vertex = fVerticesList[i];
                vertex.EdgeIn = null;
                vertex.EdgesOut.Clear();
            }

            fEdgesList.Clear();
            fVerticesList.Clear();
            fVerticesDictionary.Clear();
            Notify(null, GraphNotification.Cleared);
        }

        private void Notify(object instance, GraphNotification action)
        {
        }

        public Vertex AddVertex(string sign, object data = null)
        {
            if (string.IsNullOrEmpty(sign))
                throw new ArgumentNullException("sign");

            Vertex result;
            if (!fVerticesDictionary.TryGetValue(sign, out result)) {
                result = new Vertex();
                result.Sign = sign;
                result.Value = data;

                fVerticesList.Add(result);
                fVerticesDictionary.Add(sign, result);
                Notify(result, GraphNotification.Added);
            }

            return result;
        }

        public bool AddUndirectedEdge(Vertex source, Vertex target, int cost, object srcValue, object tgtValue)
        {
            Edge edge1 = AddDirectedEdge(source, target, cost, srcValue);
            Edge edge2 = AddDirectedEdge(target, source, cost, tgtValue);

            return (edge1 != null && edge2 != null);
        }

        public Edge AddDirectedEdge(string sourceSign, string targetSign, bool canCreate = true)
        {
            return AddDirectedEdge(sourceSign, targetSign, 0, null, canCreate);
        }

        public Edge AddDirectedEdge(string sourceSign, string targetSign, int cost, object edgeValue, bool canCreate = true)
        {
            Vertex source = FindVertex(sourceSign);
            Vertex target = FindVertex(targetSign);

            if (source == null && canCreate)
                source = AddVertex(sourceSign);
            if (target == null && canCreate)
                target = AddVertex(targetSign);

            return AddDirectedEdge(source, target, cost, edgeValue);
        }

        public Edge AddDirectedEdge(Vertex source, Vertex target, int cost, object edgeValue)
        {
            if (source == null || target == null || source == target)
                return null;

            Edge resultEdge = new Edge(source, target, cost, edgeValue);
            source.EdgesOut.Add(resultEdge);
            fEdgesList.Add(resultEdge);

            Notify(resultEdge, GraphNotification.Added);

            return resultEdge;
        }

        public void DeleteVertex(Vertex vertex)
        {
            if (vertex == null)
                return;

            for (int i = fEdgesList.Count - 1; i >= 0; i--) {
                Edge edge = fEdgesList[i];

                if (edge.Source == vertex || edge.Target == vertex) {
                    DeleteEdge(edge);
                }
            }

            fVerticesList.Remove(vertex);
            Notify(vertex, GraphNotification.Deleted);
        }

        public void DeleteEdge(Edge edge)
        {
            if (edge == null)
                return;

            Vertex src = edge.Source;
            src.EdgesOut.Remove(edge);

            fEdgesList.Remove(edge);

            Notify(edge, GraphNotification.Deleted);
        }

        public Vertex FindVertex(string sign)
        {
            Vertex result;
            fVerticesDictionary.TryGetValue(sign, out result);
            return result;
        }

        #endregion

        #region Updating

        private void SetUpdateState(bool updating)
        {
            if (updating) {
                Changing();
            } else {
                Changed();
            }
        }

        public void BeginUpdate()
        {
            if (fUpdateCount == 0) {
                SetUpdateState(true);
            }
            fUpdateCount++;
        }

        public void EndUpdate()
        {
            fUpdateCount--;
            if (fUpdateCount == 0) {
                SetUpdateState(false);
            }
        }

        private void Changed()
        {
            if (fUpdateCount == 0 && fOnChange != null) {
                fOnChange(this, null);
            }
        }

        private void Changing()
        {
            if (fUpdateCount == 0 && fOnChanging != null) {
                fOnChanging(this, null);
            }
        }

        #endregion

        #region Pathes search

        public void FindPathTree(Vertex root)
        {
            if (root == null)
                return;

            // reset path tree
            for (int i = 0, verticesCount = fVerticesList.Count; i < verticesCount; i++) {
                Vertex vertex = fVerticesList[i];
                vertex.Dist = int.MaxValue;
                vertex.Visited = false;
                vertex.EdgeIn = null;
            }

            // init root
            root.Dist = 0;
            root.Visited = true;
            root.EdgeIn = null;

            PathCandidate topCandidate = new PathCandidate(root, null);

            // processing
            while (topCandidate != null) {
                Vertex topNode = topCandidate.Node;
                topCandidate = topCandidate.Next;

                int nodeDist = topNode.Dist;
                topNode.Visited = false;

                var edgesOut = topNode.EdgesOut;
                for (int i = 0, edgesOutCount = edgesOut.Count; i < edgesOutCount; i++) {
                    Edge link = edgesOut[i];

                    Vertex target = link.Target;
                    int newDist = nodeDist + link.Cost;

                    if (newDist < target.Dist) {
                        target.Dist = newDist;
                        target.EdgeIn = link;

                        if (!target.Visited) {
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

            if (target != null) {
                Edge edge = target.EdgeIn;
                while (edge != null) {
                    result.Insert(0, edge);
                    edge = edge.Source.EdgeIn;
                }
            }

            return result;
        }

        #endregion
    }
}
