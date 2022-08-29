/*
 *  "BSLib".
 *  Copyright (C) 2009-2022 by Sergey V. Zhdanovskih.
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
using System.Linq;
using System.Text;

namespace BSLib
{
    public class StringNode
    {
        public string Value { get; set; }

        public List<StringNode> Children { get; set; }

        public StringNode()
        {
            Children = new List<StringNode>();
        }
    }


    public static class StringTree
    {
        public static string Create<TNode>(
            TNode root,
            Func<TNode, string> dumpNode,
            Func<TNode, IEnumerable<TNode>> getChildren)
        {
            var sb = new StringBuilder();

            Create(root, dumpNode, getChildren, sb, 0, new int[] { }, true);

            return sb.ToString();
        }

        public static void Create<TNode>(
            TNode node,
            Func<TNode, string> dumpNode,
            Func<TNode, IEnumerable<TNode>> getChildren,
            StringBuilder sb,
            int indentation,
            int[] siblingDepths,
            bool lastChild)
        {
            for (int i = 0; i < indentation; i++) {
                sb.Append(
                    lastChild && i == indentation - 1 ? "└" :
                    siblingDepths.Any() && siblingDepths.Last() == i && i == indentation - 1 ? "├" :
                    siblingDepths.Any() && siblingDepths.Contains(i) ? "│" :
                    " ");
            }

            sb.AppendLine(dumpNode(node));
            var children = getChildren(node);

            foreach (var child in children) {
                var lc = child.Equals(children.Last());

                Create(child, dumpNode, getChildren, sb, indentation + 1,
                    !lc ? siblingDepths.Concat(new[] { indentation }).ToArray() : siblingDepths, lc);
            }
        }
    }
}
