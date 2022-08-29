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

namespace BSLib
{
    public static class Visitor
    {
        public static void Visit<TElement>(
            TElement group,
            Action<TElement> action,
            Func<TElement, IEnumerable<TElement>> getChildren)
        {
            action(group);

            foreach (var c in getChildren(group).ToArray()) {
                Visit(c, action, getChildren);
            }
        }

        public static TResult Select<TElement, TResult>(
            TElement element,
            Func<TElement, TResult> selector,
            Action<TResult, TResult> addChild,
            Func<TElement, IEnumerable<TElement>> getChildren)
        {
            var result = selector(element);
            var children = getChildren(element);

            foreach (var child in children) {
                addChild(result, Select(child, selector, addChild, getChildren));
            }

            return result;
        }

        public static TElement Where<TElement>(
            TElement element,
            Func<TElement, bool> predicate,
            Action<TElement, TElement> addChild,
            Action<TElement, TElement> removeChild,
            Func<TElement, IEnumerable<TElement>> getChildren)
        {
            return Where(element, default(TElement), predicate, addChild, removeChild, getChildren);
        }

        private static TElement Where<TElement>(
            TElement element,
            TElement parent,
            Func<TElement, bool> predicate,
            Action<TElement, TElement> addChild,
            Action<TElement, TElement> removeChild,
            Func<TElement, IEnumerable<TElement>> getChildren)
        {
            Action<TElement, TElement> filterChild = (p, c) => {
                var filteredChild = Where(c, p, predicate, addChild, removeChild, getChildren);

                if (filteredChild != null && !filteredChild.Equals(default(TElement))) {
                    addChild(p, filteredChild);
                }
            };

            if (predicate(element)) {
                foreach (var child in getChildren(element).ToArray()) {
                    removeChild(element, child);
                    filterChild(element, child);
                }

                return element;
            } else {
                if (parent == null) {
                    throw new InvalidOperationException("Cannot filter root.");
                }

                var children = getChildren(element);
                removeChild(parent, element);

                foreach (var child in children) {
                    filterChild(parent, child);
                }

                return default(TElement);
            }
        }

        public static bool Any<TElement>(
            TElement element,
            Func<TElement, bool> predicate,
            Func<TElement, IEnumerable<TElement>> getChildren)
        {
            if (predicate(element)) {
                return true;
            } else {
                return getChildren(element).Any(x => Any(x, predicate, getChildren));
            }
        }
    }
}
