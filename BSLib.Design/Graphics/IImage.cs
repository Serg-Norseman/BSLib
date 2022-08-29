﻿/*
 *  "BSLib.Design".
 *  Copyright (C) 2018-2022 by Sergey V. Zhdanovskih.
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

namespace BSLib.Design.Graphics
{
    /// <summary>
    /// 
    /// </summary>
    public interface IImage : IDisposable
    {
        int Height { get; }
        int Width { get; }

        /// <summary>
        /// Default format: bmp.
        /// </summary>
        /// <returns></returns>
        byte[] GetBytes();

        /// <summary>
        /// Supported formats: bmp, gif, jpeg, png, tiff.
        /// </summary>
        byte[] GetBytes(string format);

        IImage Resize(int newWidth, int newHeight);
    }
}
