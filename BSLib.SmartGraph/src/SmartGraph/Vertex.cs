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
	public class Vertex : GraphObject, IComparable
	{
    	private static int NextIndex = 0;

    	public readonly int Index;

        public string Sign { get; set; }
		public object Value { get; set; }

		#region Path-search runtime

		internal int Dist { get; set; }
		internal bool Visited { get; set; }
		internal Edge EdgeIn { get; set; }

		#endregion

		public Vertex()
		{
			this.Index = NextIndex++;
		}

		public int CompareTo(object obj)
		{
			if (!(obj is Vertex))
				throw new ArgumentException("Cannot compare two objects");

			return GetHashCode().CompareTo(obj.GetHashCode());
		}
	}
}
