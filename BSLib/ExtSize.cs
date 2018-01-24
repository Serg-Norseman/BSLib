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

using System.Globalization;

namespace BSLib
{
    public struct ExtSize
    {
        public static readonly ExtSize Empty = default(ExtSize);

        public int Width;
        public int Height;

        public bool IsEmpty
        {
            get {
                return Width == 0 && Height == 0;
            }
        }

        public ExtSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public static implicit operator ExtSizeF(ExtSize p)
        {
            return new ExtSizeF((float)p.Width, (float)p.Height);
        }

        public static explicit operator ExtPoint(ExtSize size)
        {
            return new ExtPoint(size.Width, size.Height);
        }

        public static ExtSize Add(ExtSize sz1, ExtSize sz2)
        {
            return new ExtSize(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
        }

        public static ExtSize Subtract(ExtSize sz1, ExtSize sz2)
        {
            return new ExtSize(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
        }

        public static ExtSize Truncate(ExtSizeF value)
        {
            return new ExtSize((int)value.Width, (int)value.Height);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ExtSize)) {
                return false;
            }
            ExtSize size = (ExtSize)obj;
            return size.Width == Width && size.Height == Height;
        }

        public override int GetHashCode()
        {
            return Width ^ Height;
        }

        public override string ToString()
        {
            return string.Concat(new string[] {
                "{Width=", Width.ToString(CultureInfo.CurrentCulture),
                ", Height=", Height.ToString(CultureInfo.CurrentCulture),
                "}"
            });
        }
    }


    public struct ExtSizeF
    {
        public static readonly ExtSizeF Empty = default(ExtSizeF);

        public float Width;
        public float Height;

        public bool IsEmpty
        {
            get {
                return Width == 0f && Height == 0f;
            }
        }

        public ExtSizeF(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public static ExtSizeF Add(ExtSizeF sz1, ExtSizeF sz2)
        {
            return new ExtSizeF(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
        }

        public static ExtSizeF Subtract(ExtSizeF sz1, ExtSizeF sz2)
        {
            return new ExtSizeF(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ExtSizeF)) {
                return false;
            }
            ExtSizeF sizeF = (ExtSizeF)obj;
            return sizeF.Width == Width && sizeF.Height == Height && sizeF.GetType().Equals(GetType());
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public ExtSize ToSize()
        {
            return ExtSize.Truncate(this);
        }

        public override string ToString()
        {
            return string.Concat(new string[] {
                "{Width=", Width.ToString(CultureInfo.CurrentCulture),
                ", Height=", Height.ToString(CultureInfo.CurrentCulture),
                "}"
            });
        }
    }
}
