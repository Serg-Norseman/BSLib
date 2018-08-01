/*
 *  "BSLib.DataViz".
 *  Copyright (C) 2017-2018 by Sergey V. Zhdanovskih.
 *
 *  This file is part of "BSLib".
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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BSLib.DataViz.TreeMap;

namespace BSLib.DataViz.TreeMap
{
    public sealed class SimpleItem : MapItem
    {
        public Color Color;

        public SimpleItem(string name, double size)
            : base(name, size)
        {
        }
    }

    /// <summary>
    /// TreeMap Viewer's control.
    /// </summary>
    public class TreeMapViewer : UserControl
    {
        private class SimpleModel : TreemapModel
        {
            public SimpleModel(int width, int height)
                : base(width, height)
            {
            }

            public override MapItem newItem(string name, double size)
            {
                return new SimpleItem(name, size);
            }
        }

        private readonly TreemapModel fModel;
        private readonly ToolTip fToolTip;
        private string fHint;

        public TreemapModel Model
        {
            get { return fModel; }
        }

        public TreeMapViewer()
        {
            DoubleBuffered = true;

            int w = 1200;
            int h = 800;
            fModel = new SimpleModel(w, h);
            fModel.CalcLayout(new MapRect(0, 0, w, h));

            fToolTip = new ToolTip();
            fToolTip.AutoPopDelay = 5000;
            fToolTip.InitialDelay = 250;
            fToolTip.ReshowDelay = 50;
            fToolTip.ShowAlways = true;
        }

        public void UpdateView()
        {
            fModel.CalcLayout(new MapRect(0, 0, Width, Height));
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            string hint = "";
            MapItem item = fModel.FindByCoord(e.X, e.Y);
            if (item != null) {
                hint = item.Name;
            }

            if (fHint != hint) {
                fHint = hint;
                fToolTip.Show(hint, this, e.X, e.Y, 3000);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            UpdateView();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (var borderPen = new Pen(Color.Black)) {
                DrawItems(e.Graphics, fModel.GetItems(), borderPen);
            }
        }

        private void DrawItems(Graphics gfx, List<MapItem> items, Pen borderPen)
        {
            foreach (MapItem item in items) {
                DrawItem(gfx, item, borderPen);

                if (!item.IsLeaf()) {
                    DrawItems(gfx, item.Items, borderPen);
                }
            }
        }

        protected virtual void DrawItem(Graphics gfx, MapItem item, Pen borderPen)
        {
            MapRect rect = item.Bounds;

            if (item.IsLeaf()) {
                gfx.FillRectangle(new SolidBrush(((SimpleItem)item).Color), (int)rect.X, (int)rect.Y, (int)rect.W, (int)rect.H);
            }

            gfx.DrawRectangle(borderPen, (int)rect.X, (int)rect.Y, (int)rect.W, (int)rect.H);
        }
    }
}
