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
using System.Drawing.Imaging;
using System.Windows.Forms;
using BSLib.DataViz.TreeMap;

namespace BSLib.DataViz.TreeMap
{
    public class HintRequestEventArgs : EventArgs
    {
        public MapItem MapItem { get; private set; }
        public string Hint { get; set; }

        public HintRequestEventArgs(MapItem mapItem)
        {
            MapItem = mapItem;
        }
    }

    public delegate void HintRequestEventHandler(object sender, HintRequestEventArgs args);

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

        private Bitmap fBackBuffer;
        private readonly TreemapModel fModel;
        private readonly ToolTip fToolTip;
        private string fHint;

        private Color fLowerHighlight = Color.Red;
        private Color fUpperHighlight = Color.Yellow;

        private Pen fBorderPen;
        private Pen fLowerPen;
        private Pen fUpperPen;

        private MapItem fLowerHoveredItem;
        private MapItem fUpperHoveredItem;

        private bool fMouseoverHighlight;


        public TreemapModel Model
        {
            get { return fModel; }
        }

        public bool MouseoverHighlight
        {
            get { return fMouseoverHighlight; }
            set { fMouseoverHighlight = value; }
        }

        public event HintRequestEventHandler OnHintRequest;


        public TreeMapViewer()
        {
            base.DoubleBuffered = true;
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            int w = 1200;
            int h = 800;
            fModel = new SimpleModel(w, h);
            fModel.CalcLayout(new MapRect(0, 0, w, h));

            fToolTip = new ToolTip();
            fToolTip.AutoPopDelay = 5000;
            fToolTip.InitialDelay = 250;
            fToolTip.ReshowDelay = 50;
            fToolTip.ShowAlways = true;

            fBorderPen = new Pen(Color.Black);
            fLowerPen = new Pen(fLowerHighlight);
            fUpperPen = new Pen(fUpperHighlight, 2);

            fBackBuffer = null;
        }

        public void UpdateView()
        {
            fModel.CalcLayout(new MapRect(0, 0, Width, Height));
            fBackBuffer = null;
            Invalidate();
        }

        private string HintRequest(MapItem mapItem)
        {
            var onHintRequest = OnHintRequest;
            if (onHintRequest == null) return mapItem.Name;

            HintRequestEventArgs args = new HintRequestEventArgs(mapItem);
            onHintRequest(this, args);
            return args.Hint;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            string hint = "";
            MapItem upperItem = null;
            MapItem item = fModel.FindByCoord(e.X, e.Y, out upperItem);
            if (item != null) {
                hint = HintRequest(item);
            }

            if (fHint != hint) {
                fHint = hint;
                fToolTip.Show(hint, this, e.X, e.Y, 3000);
            }

            if (fMouseoverHighlight && (fUpperHoveredItem != upperItem || fLowerHoveredItem != item)) {
                fUpperHoveredItem = upperItem;
                fLowerHoveredItem = item;
                Invalidate();
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

            Graphics gfx = e.Graphics;

            if (fBackBuffer == null) {
                fBackBuffer = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

                using (var backGfx = Graphics.FromImage(fBackBuffer)) {
                    var items = fModel.GetItems();
                    if (items.Count > 0) {
                        DrawItems(backGfx, items);
                    } else {
                        backGfx.FillRectangle(new SolidBrush(Color.Silver), 0, 0, Width, Height);
                    }
                }
            }

            gfx.DrawImage(fBackBuffer, 0, 0);

            if (fMouseoverHighlight) {
                if (fUpperHoveredItem != null) {
                    var rect = fUpperHoveredItem.Bounds;
                    gfx.DrawRectangle(fUpperPen, rect.X, rect.Y, rect.W, rect.H);
                }

                if (fLowerHoveredItem != null) {
                    var rect = fLowerHoveredItem.Bounds;
                    gfx.DrawRectangle(fLowerPen, rect.X, rect.Y, rect.W, rect.H);
                }
            }
        }

        private void DrawItems(Graphics gfx, List<MapItem> items)
        {
            int num = items.Count;
            for (int i = 0; i < num; i++) {
                MapItem item = items[i];
                DrawItem(gfx, item, fBorderPen);

                if (!item.IsLeaf()) {
                    DrawItems(gfx, item.Items);
                }
            }
        }

        protected virtual void DrawItem(Graphics gfx, MapItem item, Pen borderPen)
        {
            var rect = item.Bounds;
            if (rect.W < 2 || rect.H < 2) {
                return;
            }

            if (item.IsLeaf()) {
                gfx.FillRectangle(new SolidBrush(((SimpleItem)item).Color), rect.X, rect.Y, rect.W, rect.H);
            }

            gfx.DrawRectangle(borderPen, rect.X, rect.Y, rect.W, rect.H);
        }
    }
}
