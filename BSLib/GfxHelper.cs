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

namespace BSLib
{
    /// <summary>
    /// 
    /// </summary>
    public static class GfxHelper
    {
        private static int MakeArgb(int alpha, int red, int green, int blue)
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

            byte red2, green2, blue2, alpha2;
            DecomposeARGB(color2, out alpha2, out red2, out green2, out blue2);

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
            float factor = (1.0f - fraction);
            byte red1, green1, blue1, alpha1;
            DecomposeARGB(rgb, out alpha1, out red1, out green1, out blue1);

            int red = (int) (red1 * factor);
            int green = (int) (green1 * factor);
            int blue = (int) (blue1 * factor);

            red = (red < 0) ? 0 : red;
            green = (green < 0) ? 0 : green;
            blue = (blue < 0) ? 0 : blue;

            return MakeArgb(alpha1, red, green, blue);
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
            byte red1, green1, blue1, alpha1;
            DecomposeARGB(rgb, out alpha1, out red1, out green1, out blue1);

            int red = (int) (red1 * factor);
            int green = (int) (green1 * factor);
            int blue = (int) (blue1 * factor);

            if (red < 0) {
                red = 0;
            } else if (red > 255) {
                red = 255;
            }
            if (green < 0) {
                green = 0;
            } else if (green > 255) {
                green = 255;
            }
            if (blue < 0) {
                blue = 0;
            } else if (blue > 255) {
                blue = 255;
            }

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
    }
}
