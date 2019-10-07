﻿/*
 *  "BSLib".
 *  Copyright (C) 2009-2019 by Sergey V. Zhdanovskih.
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
    // Levels in order of increasing priority: ALL, DEBUG, INFO, WARN, ERROR, FATAL, OFF
    public interface ILogger
    {
        void WriteDebug(string msg);
        void WriteDebug(string str, params object[] args);

        void WriteInfo(string msg);
        void WriteInfo(string str, params object[] args);

        void WriteError(string msg);
        void WriteError(string msg, Exception ex);
    }
}
