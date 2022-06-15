/*
 *  "BSLib".
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

using System.Collections.Generic;

namespace BSLib
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class NavigationStack<T> where T : class
    {
        private readonly Stack<T> fStackBackward;
        private readonly Stack<T> fStackForward;
        private T fCurrent;


        public T Current
        {
            get {
                return fCurrent;
            }
            set {
                if (fCurrent == value) return;

                if (fCurrent != null) {
                    fStackBackward.Push(fCurrent);
                }
                fCurrent = value;
                fStackForward.Clear();
            }
        }

        public T[] FullArray
        {
            get {
                var result = new T[fStackBackward.Count + fStackForward.Count];
                fStackBackward.CopyTo(result, 0);
                fStackForward.CopyTo(result, fStackBackward.Count);
                return result;
            }
        }


        public NavigationStack()
        {
            fStackBackward = new Stack<T>();
            fStackForward = new Stack<T>();
            fCurrent = default(T);
        }

        public T Back()
        {
            if (fCurrent != null) {
                fStackForward.Push(fCurrent);
            }
            fCurrent = (fStackBackward.Count > 0) ? fStackBackward.Pop() : null;
            return fCurrent;
        }

        public T Next()
        {
            if (fCurrent != null) {
                fStackBackward.Push(fCurrent);
            }
            fCurrent = (fStackForward.Count > 0) ? fStackForward.Pop() : null;
            return fCurrent;
        }

        public void Clear()
        {
            fStackBackward.Clear();
            fStackForward.Clear();
            fCurrent = null;
        }

        public bool CanBackward()
        {
            return fStackBackward.Count > 0;
        }

        public bool CanForward()
        {
            return fStackForward.Count > 0;
        }
    }
}
