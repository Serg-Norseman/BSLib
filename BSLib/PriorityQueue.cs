/*
 *  "BSLib".
 *  Copyright (C) 2015-2017 by Sergey V. Zhdanovskih.
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

namespace BSLib
{
    public class PriorityQueue<T> where T : IComparable<T>
    {
        private sealed class Node
        {
            public readonly T Value;
            public Node Next;

            public Node(T value, Node next)
            {
                Value = value;
                Next = next;
            }
        }

        private readonly int fCapacity;
        private Node fHead;
        private int fSize;

        public PriorityQueue(int capacity)
        {
            fCapacity = capacity;
            fHead = null;
            fSize = 0;
        }

        public bool Add(T obj)
        {
            if (Full) {
                return false;
            }

            if (fHead == null) {
                fHead = new Node(obj, null);
            } else if (obj.CompareTo(fHead.Value) < 0) {
                fHead = new Node(obj, fHead);
            } else {
                Node p = fHead;
                while (p.Next != null && obj.CompareTo(p.Next.Value) >= 0) {
                    p = p.Next; //or equal to preserve FIFO on equal items
                }
                p.Next = new Node(obj, p.Next);
            }
            ++fSize;
            return true;
        }

        public T Peek()
        {
            return Empty ? default(T) : fHead.Value;
        }

        public T Poll()
        {
            if (Empty) {
                return default(T);
            }

            T value = fHead.Value;
            fHead = fHead.Next;
            --fSize;
            return value;
        }

        public int Size()
        {
            return fSize;
        }

        public bool Contains(T @object)
        {
            Node p = fHead;
            while (p != null) {
                if (((IComparable<T>)@object).CompareTo(p.Value) == 0) {
                    return true;
                }
                p = p.Next;
            }
            return false;
        }

        public void Clear()
        {
            fHead = null;
            fSize = 0;
        }

        public bool Empty
        {
            get { return (fSize == 0); }
        }

        public bool Full
        {
            get { return (fSize == fCapacity); }
        }
    }
}
