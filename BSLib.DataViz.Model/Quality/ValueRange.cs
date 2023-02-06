/*
 *  "BSLib.DataViz".
 *  Copyright (C) 2019-2023 by Sergey V. Zhdanovskih.
 *
 *  This file is part of "BSLib".
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

namespace BSLib.DataViz.Quality
{
    public sealed class ValueRange
    {
        public double Min { get; private set; }
        public double Max { get; private set; }
        public int Color { get; private set; }
        public string Name { get; private set; }

        public ValueRange(double min, double max, int color, string name)
        {
            Min = min;
            Max = max;
            Color = color;
            Name = name;
        }
    }
}
