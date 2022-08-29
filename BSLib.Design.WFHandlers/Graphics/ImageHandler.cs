﻿/*
 *  "BSLib.Design".
 *  Copyright (C) 2009-2022 by Sergey V. Zhdanovskih.
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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using BSLib.Design.Graphics;

namespace BSLib.Design.Handlers
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ImageHandler: TypeHandler<Image>, IImage
    {
        public int Height
        {
            get { return Handle.Height; }
        }

        public int Width
        {
            get { return Handle.Width; }
        }

        public ImageHandler(Image handle) : base(handle)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                Handle.Dispose();
            }
            base.Dispose(disposing);
        }

        public byte[] GetBytes()
        {
            return GetBytes("bmp");
        }

        public byte[] GetBytes(string format)
        {
            ImageFormat imgFormat = ImageFormat.Bmp;
            switch (format) {
                case "bmp":
                    imgFormat = ImageFormat.Bmp;
                    break;

                case "gif":
                    imgFormat = ImageFormat.Gif;
                    break;

                case "jpeg":
                    imgFormat = ImageFormat.Jpeg;
                    break;

                case "png":
                    imgFormat = ImageFormat.Png;
                    break;

                case "tiff":
                    imgFormat = ImageFormat.Tiff;
                    break;
            }

            using (var stream = new MemoryStream()) {
                Handle.Save(stream, imgFormat);
                return stream.ToArray();
            }
        }

        public IImage Resize(int newWidth, int newHeight)
        {
            var original = Handle as Bitmap;

            int imgWidth = original.Width;
            int imgHeight = original.Height;

            if (newWidth != imgWidth || newHeight != imgHeight) {
                float ratio = GfxHelper.ZoomToFit(imgWidth, imgHeight, newWidth, newHeight);
                imgWidth = (int)(imgWidth * ratio);
                imgHeight = (int)(imgHeight * ratio);

                Bitmap newImage = new Bitmap(imgWidth, imgHeight, PixelFormat.Format24bppRgb);
                using (var graphic = System.Drawing.Graphics.FromImage(newImage)) {
                    graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphic.SmoothingMode = SmoothingMode.HighQuality;
                    graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphic.CompositingQuality = CompositingQuality.HighQuality;
                    graphic.DrawImage(original, 0, 0, imgWidth, imgHeight);
                }

                return new ImageHandler(newImage);
            } else {
                return this;
            }
        }
    }
}
