﻿/*
 *  "BSLib.IPC", the interprocess communication via TCP-sockets.
 *  Copyright (C) 2018 by Sergey V. Zhdanovskih.
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

using System;
using System.Net;

namespace BSLib.IPC
{
    public sealed class DataReceiveEventArgs : EventArgs
    {
        private readonly IPEndPoint fEndPoint;
        private readonly IPCMessage fMessage;
        private readonly IPCStream fStream;

        public IPEndPoint EndPoint
        {
            get { return fEndPoint; }
        }

        public IPCMessage Message
        {
            get { return fMessage; }
        }

        public IPCStream Stream
        {
            get { return fStream; }
        }

        public DataReceiveEventArgs(IPEndPoint endPoint, IPCMessage message, IPCStream stream)
        {
            fEndPoint = endPoint;
            fMessage = message;
            fStream = stream;
        }
    }
}
