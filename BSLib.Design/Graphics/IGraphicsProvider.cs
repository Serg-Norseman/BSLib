﻿/*
 *  "BSLib.Design".
 *  Copyright (C) 2018-2020 by Sergey V. Zhdanovskih.
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
using System.IO;

namespace BSLib.Design.Graphics
{
    /// <summary>
    /// Interface for platform-independent graphics rendering providers.
    /// </summary>
    public interface IGraphicsProvider
    {
        IColor CreateColor(int argb);
        IColor CreateColor(int r, int g, int b);
        IColor CreateColor(int a, int r, int g, int b);

        IPen CreatePen(IColor color, float width);
        IBrush CreateSolidBrush(IColor color);

        IFont CreateFont(string fontName, float size, bool bold);

        IGfxPath CreatePath();

        IImage CreateImage(Stream stream);
        IImage CreateImage(Stream stream, int thumbWidth, int thumbHeight, ExtRect cutoutArea);
        IImage LoadImage(string fileName);
        void SaveImage(IImage image, string fileName);

        ExtSizeF GetTextSize(string text, IFont font, object target);

        string GetDefaultFontName();
    }
}
