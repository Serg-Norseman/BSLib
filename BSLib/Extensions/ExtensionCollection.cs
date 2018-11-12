/*
 *  "BSLib".
 *  Copyright (C) 2009-2018 by Sergey V. Zhdanovskih.
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
using System.Collections.ObjectModel;

namespace BSLib.Extensions
{
    public sealed class ExtensionCollection<T, X> : IExtensionCollection<T, X>
        where T : class, IExtensibleObject<T>
        where X : class, IExtension<T>
    {
        private T fOwner;
        private X[] fItems;
        private int fSize;
        private readonly object fSyncRoot;

        public ExtensionCollection(T owner)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            fOwner = owner;
            fItems = new X[0];
            fSize = 0;
            fSyncRoot = new Object();
        }

        public int Count
        {
            get {
                lock (fSyncRoot) {
                    return fSize;
                }
            }
        }

        public void Add(X item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            lock (fSyncRoot) {
                int newSize = fSize + 1;
                if (newSize > fItems.Length) {
                    X[] array = new X[newSize];
                    if (fSize > 0) {
                        Array.Copy(fItems, 0, array, 0, fSize);
                    }
                    fItems = array;
                }
                fItems[fSize] = item;
                fSize++;

                item.Attach(fOwner);
            }
        }

        public bool Remove(X item)
        {
            lock (fSyncRoot) {
                int index = IndexOf(item);
                if (index < 0) return false;

                fItems[index].Detach(fOwner);

                fSize--;
                if (index < fSize) {
                    Array.Copy(fItems, index + 1, fItems, index, fSize - index);
                }
                fItems[fSize] = default(X);

                return true;
            }
        }

        private int IndexOf(X item)
        {
            for (int i = 0; i < fSize; i++) {
                if (Equals(fItems[i], item)) {
                    return i;
                }
            }
            return -1;
        }

        public bool Contains(X item)
        {
            lock (fSyncRoot) {
                return (IndexOf(item) >= 0);
            }
        }

        public void Clear()
        {
            lock (fSyncRoot) {
                if (fSize > 0) {
                    for (int i = 0; i < fSize; i++) {
                        fItems[i].Detach(fOwner);
                    }

                    Array.Clear(fItems, 0, fSize);
                    fSize = 0;
                }
            }
        }


        public E Find<E>()
        {
            lock (fSyncRoot) {
                for (int i = 0; i < fSize; i++) {
                    IExtension<T> item = fItems[i];
                    if (item is E) return (E)item;
                }
            }

            return default(E);
        }

        public Collection<E> FindAll<E>()
        {
            var result = new Collection<E>();

            lock (fSyncRoot) {
                for (int i = 0; i < fSize; i++) {
                    IExtension<T> item = fItems[i];
                    if (item is E) result.Add((E)item);
                }
            }

            return result;
        }
    }
}
