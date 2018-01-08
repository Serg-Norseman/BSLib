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

namespace BSLib
{
    /// <summary>
    /// 
    /// </summary>
    public struct ExtMargins
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public static readonly ExtMargins Empty = new ExtMargins(0);

        public ExtMargins(int all)
        {
            Left = all;
            Top = all;
            Right = all;
            Bottom = all;
        }

        public ExtMargins(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
}
