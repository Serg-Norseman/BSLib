/*
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
            using (var stream = new MemoryStream()) {
                Handle.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                return stream.ToArray();
            }
        }
    }
}
