/*
 *  "BSLib".
 *  Copyright (C) 2018 by Sergey V. Zhdanovskih.
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

namespace BSLib
{
    public static class EnumHelper
    {
        public static T Parse<T>(string value)
        {
            return Parse<T>(value, false);
        }

        public static T Parse<T>(string value, bool ignoreCase)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        public static T Parse<T>(string value, bool ignoreCase, T defaultValue)
        {
            T result = defaultValue;
            try {
                result = (T)Enum.Parse(typeof(T), value, ignoreCase);
            } catch {
            }
            return result;
        }
    }
}
