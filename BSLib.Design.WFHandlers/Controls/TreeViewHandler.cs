﻿/*
 *  "BSLib.Design".
 *  Copyright (C) 2009-2020 by Sergey V. Zhdanovskih.
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

using System.Windows.Forms;
using BSLib.Design.MVP.Controls;

namespace BSLib.Design.Handlers
{
    public class TreeNodeEx : TreeNode, ITVNode
    {
        public TreeNodeEx(string text, object tag) : base(text)
        {
            Tag = tag;
        }
    }


    public sealed class TreeViewHandler : BaseControlHandler<TreeView, TreeViewHandler>, ITreeView
    {
        public TreeViewHandler(TreeView control) : base(control)
        {
        }

        public ITVNode AddNode(ITVNode parent, string name, object tag)
        {
            var node = new TreeNodeEx(name, tag);
            if (parent == null) {
                Control.Nodes.Add(node);
            } else {
                ((TreeNodeEx)parent).Nodes.Add(node);
            }
            return node;
        }

        public void BeginUpdate()
        {
            Control.BeginUpdate();
        }

        public void Clear()
        {
            Control.Nodes.Clear();
        }

        public void EndUpdate()
        {
            Control.EndUpdate();
        }

        public void Expand(ITVNode node)
        {
            TreeNode treeNode = node as TreeNode;
            if (treeNode != null) {
                treeNode.ExpandAll();
            }
        }

        public object GetSelectedData()
        {
            TreeNodeEx node = Control.SelectedNode as TreeNodeEx;
            return (node == null) ? null : node.Tag;
        }
    }
}
