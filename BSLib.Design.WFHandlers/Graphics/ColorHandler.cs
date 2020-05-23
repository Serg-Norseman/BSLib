﻿/*
 *  "BSLib.Design".
 *  Copyright (C) 2009-2020 by Sergey V. Zhdanovskih.
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

using System.Drawing;
using BSLib.Design.Graphics;

namespace BSLib.Design.Handlers
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ColorHandler: TypeHandler<Color>, IColor
    {
        public ColorHandler(Color handle) : base(handle)
        {
        }

        public IColor Darker(float fraction)
        {
            int rgb = Handle.ToArgb();
            Color darkColor = Color.FromArgb(GfxHelper.Darker(rgb, fraction));
            return new ColorHandler(darkColor);
        }

        public IColor Lighter(float fraction)
        {
            int rgb = Handle.ToArgb();
            Color lightColor = Color.FromArgb(GfxHelper.Lighter(rgb, fraction));
            return new ColorHandler(lightColor);
        }

        public string GetName()
        {
            Color color = this.Handle;
            return color.Name;
        }

        public int ToArgb()
        {
            int result = this.Handle.ToArgb();
            return result;
        }

        public string GetCode()
        {
            int argb = ToArgb() & 0xFFFFFF;
            string result = argb.ToString("X6");
            return result;
        }

        public byte GetR()
        {
            return Handle.R;
        }

        public byte GetG()
        {
            return Handle.G;
        }

        public byte GetB()
        {
            return Handle.B;
        }

        public byte GetA()
        {
            return Handle.A;
        }

        public bool IsTransparent()
        {
            return (Handle == Color.Transparent);
        }
    }
}
