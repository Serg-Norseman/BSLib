/*
 *  "BSLib.Timeline".
 *  Copyright (C) 2019-2021 by Sergey V. Zhdanovskih.
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

namespace BSLib.DataViz.Timeline
{
    /// <summary>
    ///   Describes an item that can be placed on a track in the timeline.
    /// </summary>
    public class EventFrame : ITimeObject
    {
        /// <summary>
        ///   The name of the time object (event frame or track).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///   The beginning of the item.
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        ///   The end of the item.
        /// </summary>
        public DateTime End { get; set; }

        public int Color { get; set; }


        public EventFrame(string name, DateTime start, DateTime end, int color)
        {
            Name = name;
            Start = start;
            End = end;
            Color = color;
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, End: {1}, Start: {2}", Name, End, Start);
        }
    }
}
