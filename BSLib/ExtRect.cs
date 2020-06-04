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
    /// <summary>
    /// The structure of a typical planar rectangle in integer coordinates.
    /// The main task is to untie the use of the classic System.Drawings namespace
    /// and from setting the size via Width/Height.
    /// </summary>
    public struct ExtRect : ICloneable<ExtRect>, IEquatable<ExtRect>
    {
        public static readonly ExtRect Empty = default(ExtRect);

        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public int Square
        {
            get { return Width * Height; }
        }

        public int Width
        {
            get { return (Right == Left) ? 0 : Right - Left + 1; }
        }

        public int Height
        {
            get { return (Bottom == Top) ? 0 : Bottom - Top + 1; }
        }

        public static ExtRect Create(int left, int top, int right, int bottom)
        {
            ExtRect result;
            result.Left = left;
            result.Top = top;
            result.Right = right;
            result.Bottom = bottom;
            return result;
        }

        public static ExtRect CreateBounds(int left, int top, int width, int height)
        {
            return Create(left, top, left + width - 1, top + height - 1);
        }

        public static ExtRect CreateEmpty()
        {
            return Create(0, 0, 0, 0);
        }

        public int GetWidth()
        {
            return (Right == Left) ? 0 : Right - Left + 1;
        }

        public int GetHeight()
        {
            return (Bottom == Top) ? 0 : Bottom - Top + 1;
        }

        public bool IsEmpty()
        {
            return Right <= Left || Bottom <= Top;
        }

        public bool Contains(int x, int y)
        {
            return x >= Left && y >= Top && x <= Right && y <= Bottom;
        }

        public bool Contains(ExtPoint pt)
        {
            return Contains(pt.X, pt.Y);
        }

        public bool Contains(ExtRect rt)
        {
            return Contains(rt.Left, rt.Top) && Contains(rt.Right, rt.Top)
                && Contains(rt.Left, rt.Bottom) && Contains(rt.Right, rt.Bottom);
        }

        public ExtRect GetOffset(int dX, int dY)
        {
            return Create(Left + dX, Top + dY, Right + dX, Bottom + dY);
        }

        public void Offset(int dX, int dY)
        {
            Left += dX;
            Right += dX;
            Top += dY;
            Bottom += dY;
        }

        public void Inflate(int dX, int dY)
        {
            Left -= dX;
            Top -= dY;
            Right += dX;
            Bottom += dY;
        }

        public bool IntersectsWith(ExtRect rect)
        {
            return rect.Left < Right && Left < rect.Right && rect.Top < Bottom && Top < rect.Bottom;
        }

        public bool IsIntersect(ExtRect other)
        {
            ExtRect r = this;

            if (other.Left > Left) {
                r.Left = other.Left;
            }
            if (other.Top > Top) {
                r.Top = other.Top;
            }
            if (other.Right < Right) {
                r.Right = other.Right;
            }
            if (other.Bottom < Bottom) {
                r.Bottom = other.Bottom;
            }

            return !r.IsEmpty();
        }

        public bool IsBorder(int x, int y)
        {
            return y == Top || y == Bottom || x == Left || x == Right;
        }

        public bool IsInside(ExtRect checkedRect)
        {
            return checkedRect.Left >= Left && checkedRect.Top >= Top && checkedRect.Right <= Right && checkedRect.Bottom <= Bottom;
        }

        public ExtPoint GetCenter()
        {
            int cx = Left + Width / 2;
            int cy = Top + Height / 2;
            return new ExtPoint(cx, cy);
        }

        public void SetBounds(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public override string ToString()
        {
            return string.Concat("{X=", Left.ToString(), ",Y=", Top.ToString(),
                                 ",Width=", GetWidth().ToString(), ",Height=", GetHeight().ToString(), "}");
        }

        public ExtRect Clone()
        {
            return Create(Left, Top, Right, Bottom);
        }

        public bool Equals(ExtRect other)
        {
            return (Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom);
        }
    }


    public struct ExtRectF
    {
        public static readonly ExtRect Empty = default(ExtRect);

        public float Left;
        public float Top;
        public float Right;
        public float Bottom;

        public static implicit operator ExtRectF(ExtRect rt)
        {
            return Create(rt.Left, rt.Top, rt.Right, rt.Bottom);
        }

        public static ExtRectF Create(float left, float top, float right, float bottom)
        {
            ExtRectF result;
            result.Left = left;
            result.Top = top;
            result.Right = right;
            result.Bottom = bottom;
            return result;
        }

        public static ExtRectF CreateBounds(float left, float top, float width, float height)
        {
            return Create(left, top, left + width - 1, top + height - 1);
        }

        public static ExtRectF CreateEmpty()
        {
            return Create(0, 0, 0, 0);
        }

        public float GetWidth()
        {
            return (Right == Left) ? 0 : Right - Left + 1;
        }

        public float GetHeight()
        {
            return (Bottom == Top) ? 0 : Bottom - Top + 1;
        }

        public bool IsEmpty()
        {
            return Right <= Left || Bottom <= Top;
        }

        public bool Contains(int x, int y)
        {
            return x >= Left && y >= Top && x <= Right && y <= Bottom;
        }

        public ExtRectF GetOffset(float dX, float dY)
        {
            return Create(Left + dX, Top + dY, Right + dX, Bottom + dY);
        }

        public void Offset(int dX, int dY)
        {
            Left += dX;
            Right += dX;
            Top += dY;
            Bottom += dY;
        }

        public void Inflate(int dX, int dY)
        {
            Left += dX;
            Right -= dX;
            Top += dY;
            Bottom -= dY;
        }

        public bool IntersectsWith(ExtRect rect)
        {
            return rect.Left < Right && Left < rect.Right && rect.Top < Bottom && Top < rect.Bottom;
        }

        public override string ToString()
        {
            return string.Concat("{X=", Left.ToString(), ",Y=", Top.ToString(),
                                 ",Width=", GetWidth().ToString(), ",Height=", GetHeight().ToString(), "}");
        }
    }
}
