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

using System.Collections.Generic;

namespace BSLib.DataViz.Timeline
{
    /// <summary>
    ///   Wraps a single <see cref="EventFrame" /> into an Track.
    /// </summary>
    public class Track : ITimeObject
    {
        /// <summary>
        ///   The wrapped timeline track.
        /// </summary>
        private readonly IList<EventFrame> fFrames;


        /// <summary>
        ///   The elements within this track.
        /// </summary>
        public IList<EventFrame> Frames
        {
            get { return fFrames; }
        }

        /// <summary>
        ///   The name of the track.
        ///   This will be displayed alongside the track in the timeline.
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        ///   Construct a new Track.
        /// </summary>
        public Track()
        {
            fFrames = new List<EventFrame>();
        }

        /// <summary>
        ///   Construct a new Track.
        /// </summary>
        /// <param name="track">The timeline track that should be wrapped.</param>
        public Track(EventFrame track) : this()
        {
            fFrames.Add(track);
            Name = track.Name;
        }
    }
}
