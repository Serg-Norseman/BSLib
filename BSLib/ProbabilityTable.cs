/*
 *  "BSLib".
 *  Copyright (C) 2009-2017 by Sergey V. Zhdanovskih.
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
    public sealed class ProbabilityTable<T>
    {
        private sealed class Pair
        {
            public T Value;
            public int Weight;

            public Pair(T value, int weight)
            {
                Value = value;
                Weight = weight;
            }
        }

        private readonly List<Pair> fTable;
        private int fTotal;

        public ProbabilityTable()
        {
            fTable = new List<Pair>();
            fTotal = 0;
        }

        public int Size()
        {
            return fTable.Count;
        }

        public void Add(T item, int weight)
        {
            fTable.Add(new Pair(item, weight));
            fTotal += weight;
        }

        public T GetRandomItem()
        {
            if (fTable.Count == 0) {
                return default(T);
            }
    
            int index = RandomHelper.GetRandom(fTotal);
    
            foreach (Pair pair in fTable) {
                index -= (pair.Weight);
                if (index < 0) {
                    return pair.Value;
                }
            }
    
            return default(T);
        }
    }
}
