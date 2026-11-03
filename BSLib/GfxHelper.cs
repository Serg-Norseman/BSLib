/*
 *  "BSLib".
 *  Copyright (C) 2009-2026 by Sergey V. Zhdanovskih.
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
    public delegate void LineProc(int aX, int aY, ref bool refContinue);


    /// <summary>
    /// 
    /// </summary>
    public static class GfxHelper
    {
        public static int MakeArgb(int alpha, int red, int green, int blue)
        {
            return (red << 16 | green << 8 | blue | alpha << 24);
        }

        public static void DecomposeARGB(int color, out byte alpha, out byte red, out byte green, out byte blue)
        {
            alpha = (byte)((color >> 24) & 0xFF);
            red = (byte)((color >> 16) & 0xFF);
            green = (byte)((color >> 8) & 0xFF);
            blue = (byte)((color >> 0) & 0xFF);
        }

        /// <summary>
        /// Blend two colors.
        /// </summary>
        /// <param name="color1"> First color to blend. </param>
        /// <param name="color2"> Second color to blend. </param>
        /// <param name="ratio"> Blend ratio. 0.5 will give even blend, 1.0 will
        /// return color1, 0.0 will return color2 and so on. </param>
        /// <returns> Blended color. </returns>
        public static int Blend(int color1, int color2, float ratio)
        {
            float rat1 = ratio;
            float rat2 = (float)1.0 - rat1;

            byte red1, green1, blue1, alpha1;
            DecomposeARGB(color1, out alpha1, out red1, out green1, out blue1);

            byte red2, green2, blue2;
            DecomposeARGB(color2, out _, out red2, out green2, out blue2);

            int color = MakeArgb(alpha1,
                (byte)(red1 * rat1 + red2 * rat2),
                (byte)(green1 * rat1 + green2 * rat2),
                (byte)(blue1 * rat1 + blue2 * rat2));

            return color;
        }

        /// <summary>
        /// Make a color darker.
        /// </summary>
        /// <param name="rgb"> Color to make darker. </param>
        /// <param name="fraction"> Darkness fraction. </param>
        /// <returns> Darker color. </returns>
        public static int Darker(int rgb, float fraction)
        {
            float factor = (1.0f - fraction.Clamp(0.0f, 1.0f));
            return ColorRatio(rgb, factor);
        }

        /// <summary>
        /// Make a color lighter.
        /// </summary>
        /// <param name="rgb"> Color to make lighter. </param>
        /// <param name="fraction"> Darkness fraction. </param>
        /// <returns> Lighter color. </returns>
        public static int Lighter(int rgb, float fraction)
        {
            float factor = (1.0f + fraction);
            return ColorRatio(rgb, factor);
        }

        private static int ColorRatio(int rgb, float factor)
        {
            byte red1, green1, blue1, alpha1;
            DecomposeARGB(rgb, out alpha1, out red1, out green1, out blue1);

            int red = (int) (red1 * factor);
            int green = (int) (green1 * factor);
            int blue = (int) (blue1 * factor);

            red = red.Clamp(0, 255);
            green = green.Clamp(0, 255);
            blue = blue.Clamp(0, 255);

            return MakeArgb(alpha1, red, green, blue);
        }

        public static float ZoomToFit(float imgWidth, float imgHeight,
                                      float requireWidth, float requireHeight)
        {
            if (imgWidth == 0.0f || imgHeight == 0.0f) return 1.0f;

            float aspectRatio;

            if (imgWidth > imgHeight) {
                aspectRatio = requireWidth / imgWidth;

                if (requireHeight < imgHeight * aspectRatio) {
                    aspectRatio = requireHeight / imgHeight;
                }
            } else {
                aspectRatio = requireHeight / imgHeight;

                if (requireWidth < imgWidth * aspectRatio) {
                    aspectRatio = requireWidth / imgWidth;
                }
            }

            return aspectRatio;
        }

        private const float DEG2RAD = 3.14159F / 180;

        public static ExtPoint[] GetCirclePoints(ExtPoint center, int count, float radius)
        {
            ExtPoint[] result = new ExtPoint[count];
            if (count > 0) {
                float degSection = 360.0f / count;
                for (int i = 0; i < count; i++) {
                    float degInRad = (i * degSection) * DEG2RAD;
                    float dx = (float)Math.Cos(degInRad) * radius;
                    float dy = (float)Math.Sin(degInRad) * radius;
                    result[i] = new ExtPoint((int)Math.Round(center.X + dx), (int)Math.Round(center.Y + dy));
                }
            }
            return result;
        }

        public static ExtPoint MidPt(ExtPoint pt1, ExtPoint pt2)
        {
            int dx = (int)Math.Round((pt2.X + pt1.X) / 2.0f);
            int dy = (int)Math.Round((pt2.Y + pt1.Y) / 2.0f);
            return new ExtPoint(dx, dy);
        }

        public static ExtPointF IntersectLine2Line(ExtPointF p1, ExtPointF p2, ExtPointF p3, ExtPointF p4)
        {
            float denom = ((p4.Y - p3.Y) * (p2.X - p1.X) - (p4.X - p3.X) * (p2.Y - p1.Y));
            if (denom == 0) {
                return ExtPointF.Empty; // lines are parallel
            }
            float ua = ((p4.X - p3.X) * (p1.Y - p3.Y) - (p4.Y - p3.Y) * (p1.X - p3.X)) / denom;
            float ub = ((p2.X - p1.X) * (p1.Y - p3.Y) - (p2.Y - p1.Y) * (p1.X - p3.X)) / denom;

            if (ua < 0 || ua > 1 || ub < 0 || ub > 1) {
                return ExtPointF.Empty;
            }

            return new ExtPointF(p1.X + ua * (p2.X - p1.X), p1.Y + ua * (p2.Y - p1.Y));
        }

        public static ExtPointF IntersectLine2Rect(ExtPointF p1, ExtPointF p2, ExtRect rt)
        {
            float bx1 = rt.Left;
            float by1 = rt.Top;
            float bx2 = rt.Right;
            float by2 = rt.Bottom;

            ExtPointF tl = new ExtPointF(bx1, by1);
            ExtPointF tr = new ExtPointF(bx2, by1);
            ExtPointF bl = new ExtPointF(bx1, by2);
            ExtPointF br = new ExtPointF(bx2, by2);

            ExtPointF pt;

            pt = IntersectLine2Line(p1, p2, tl, tr);
            if (!pt.IsEmpty) {
                return pt;
            }

            pt = IntersectLine2Line(p1, p2, tr, br);
            if (!pt.IsEmpty) {
                return pt;
            }

            pt = IntersectLine2Line(p1, p2, br, bl);
            if (!pt.IsEmpty) {
                return pt;
            }

            pt = IntersectLine2Line(p1, p2, bl, tl);
            if (!pt.IsEmpty) {
                return pt;
            }

            return ExtPointF.Empty;
        }

        public static int CalcDistanceToArea(ExtPoint p, ExtRect a)
        {
            int result;
            if (a.Contains(p.X, p.Y)) {
                result = 0;
            } else {
                int dist = int.MaxValue;

                for (int x = a.Left; x <= a.Right; x++) {
                    int td = MathHelper.Distance(p.X, p.Y, x, a.Top);
                    if (td < dist) {
                        dist = td;
                    }
                    td = MathHelper.Distance(p.X, p.Y, x, a.Bottom);
                    if (td < dist) {
                        dist = td;
                    }
                }

                for (int y = a.Top; y <= a.Bottom; y++) {
                    int td = MathHelper.Distance(p.X, p.Y, a.Left, y);
                    if (td < dist) {
                        dist = td;
                    }
                    td = MathHelper.Distance(p.X, p.Y, a.Right, y);
                    if (td < dist) {
                        dist = td;
                    }
                }
                result = dist;
            }
            return result;
        }

        public static bool DoLine(int x1, int y1, int x2, int y2, LineProc lineProc, bool excludeFirst)
        {
            bool result = true;

            int dX = Math.Abs(x1 - x2);
            int dY = Math.Abs(y1 - y2);
            int dX2 = dX << 1;
            int dY2 = dY << 1;

            int XInc;
            if (x1 < x2) {
                XInc = 1;
            } else {
                XInc = -1;
            }

            int YInc;
            if (y1 < y2) {
                YInc = 1;
            } else {
                YInc = -1;
            }

            int X = x1;
            int Y = y1;

            bool refContinue = result;
            if (!excludeFirst) {
                lineProc(X, Y, ref refContinue);
            }

            if (refContinue) {
                if (dX > dY) {
                    int S = dY2 - dX;
                    int dXY = dY2 - dX2;

                    for (int i = 1; i <= dX; i++) {
                        if (S >= 0) {
                            Y += YInc;
                            S += dXY;
                        } else {
                            S += dY2;
                        }
                        X += XInc;

                        lineProc(X, Y, ref refContinue);
                        if (!refContinue) {
                            break;
                        }
                    }
                } else {
                    int S = dX2 - dY;
                    int dXY = dX2 - dY2;

                    for (int i = 1; i <= dY; i++) {
                        if (S >= 0) {
                            X += XInc;
                            S += dXY;
                        } else {
                            S += dX2;
                        }
                        Y += YInc;

                        lineProc(X, Y, ref refContinue);
                        if (!refContinue) {
                            break;
                        }
                    }
                }
            }

            return refContinue;
        }
    }
}
