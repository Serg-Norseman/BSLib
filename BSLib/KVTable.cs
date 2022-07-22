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

using System.Collections.Generic;

namespace BSLib
{
    public class KVTable<TKey, TValue>
    {
        private Dictionary<TKey, TValue> fTable = new Dictionary<TKey, TValue>();

        public TValue this[TKey key]
        {
            get {
                TValue val;
                if (!fTable.TryGetValue(key, out val)) {
                    return default(TValue);
                }
                return val;
            }
            set {
                if (!fTable.ContainsKey(key)) {
                    fTable.Add(key, value);
                } else {
                    fTable[key] = value;
                }
            }
        }
    }
}
