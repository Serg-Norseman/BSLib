/*
 *  "BSLib.Timeline".
 *  Copyright (C) 2019-2020 by Sergey V. Zhdanovskih.
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

namespace BSLib.DataViz.Timeline
{
    /// <summary>
    ///   Common interface members for elements that can serve as timeline tracks.
    /// </summary>
    public interface ITimeObject
    {
        /// <summary>
        ///   The name of the time object (event frame or track).
        ///   This will be displayed alongside the track in the timeline.
        /// </summary>
        string Name { get; set; }
    }
}
