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

using System;
using System.Globalization;

namespace BSLib
{
    public struct ExtPoint : ICloneable<ExtPoint>
    {
        public static readonly ExtPoint Empty = default(ExtPoint);

        public int X;
        public int Y;

        public bool IsEmpty
        {
            get { return X == 0 && Y == 0; }
        }

        public ExtPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator ExtPointF(ExtPoint p)
        {
            return new ExtPointF(p.X, p.Y);
        }

        public static ExtPoint Add(ExtPoint pt, ExtSize sz)
        {
            return new ExtPoint(pt.X + sz.Width, pt.Y + sz.Height);
        }

        public static ExtPoint Subtract(ExtPoint pt, ExtSize sz)
        {
            return new ExtPoint(pt.X - sz.Width, pt.Y - sz.Height);
        }

        public static ExtPoint Truncate(ExtPointF value)
        {
            return new ExtPoint((int)value.X, (int)value.Y);
        }

        public bool Equals(int ax, int ay)
        {
            return X == ax && Y == ay;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ExtPoint)) {
                return false;
            }
            ExtPoint point = (ExtPoint)obj;
            return point.X == X && point.Y == Y;
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }

        public void Offset(int dx, int dy)
        {
            X += dx;
            Y += dy;
        }

        public override string ToString()
        {
            return string.Concat(new string[] {
                "{X=", X.ToString(CultureInfo.CurrentCulture),
                ",Y=", Y.ToString(CultureInfo.CurrentCulture),
                "}"
            });
        }

        public ExtPoint Clone()
        {
            return new ExtPoint(X, Y);
        }
    }


    public struct ExtPointF
    {
        public static readonly ExtPointF Empty = default(ExtPointF);

        public float X;
        public float Y;

        public bool IsEmpty
        {
            get { return X == 0f && Y == 0f; }
        }

        public ExtPointF(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static ExtPointF Add(ExtPointF pt, ExtSize sz)
        {
            return new ExtPointF(pt.X + (float)sz.Width, pt.Y + (float)sz.Height);
        }

        public static ExtPointF Subtract(ExtPointF pt, ExtSize sz)
        {
            return new ExtPointF(pt.X - (float)sz.Width, pt.Y - (float)sz.Height);
        }

        public static ExtPointF Add(ExtPointF pt, ExtSizeF sz)
        {
            return new ExtPointF(pt.X + sz.Width, pt.Y + sz.Height);
        }

        public static ExtPointF Subtract(ExtPointF pt, ExtSizeF sz)
        {
            return new ExtPointF(pt.X - sz.Width, pt.Y - sz.Height);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ExtPointF)) {
                return false;
            }
            ExtPointF pointF = (ExtPointF)obj;
            return pointF.X == X && pointF.Y == Y && pointF.GetType().Equals(GetType());
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void Offset(float dx, float dy)
        {
            X += dx;
            Y += dy;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{{X={0}, Y={1}}}", new object[] {
                X,
                Y
            });
        }
    }
}
