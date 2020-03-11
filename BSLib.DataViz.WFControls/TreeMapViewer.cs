/*
 *  "BSLib.DataViz".
 *  Copyright (C) 2017-2020 by Sergey V. Zhdanovskih.
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
    public delegate void HintRequestEventHandler(object sender, HintRequestEventArgs args);

    public sealed class SimpleItem : MapItem
    {
        public Color Color;

        public SimpleItem(MapItem parent, string name, double size) : base(parent, name, size)
        {
            Color = Color.Silver;
        }
    }

    /// <summary>
    /// TreeMap Viewer's control.
    /// </summary>
    public class TreeMapViewer : UserControl
    {
        private class SimpleModel : TreemapModel
        {
            public SimpleModel() : base()
            {
            }

            protected override MapItem newItem(MapItem parent, string name, double size)
            {
                return new SimpleItem(parent, name, size);
            }
        }

        private readonly TreemapModel fModel;
        private readonly ToolTip fToolTip;

        private Bitmap fBackBuffer;
        private MapItem fCurrentItem;
        private string fHint;
        private int fItemsPadding;
        private bool fMouseoverHighlight;
        private MapItem fRootItem;
        private bool fShowNames;
        private MapItem fUpperItem;

        private Pen fBorderPen;
        private Color fLowerHighlight = Color.Red;
        private MapItem fLowerHoveredItem;
        private Pen fLowerPen;
        private Color fUpperHighlight = Color.Yellow;
        private MapItem fUpperHoveredItem;
        private Pen fUpperPen;


        public MapItem CurrentItem
        {
            get {
                return fCurrentItem;
            }
        }

        public int ItemsPadding
        {
            get {
                return fItemsPadding;
            }
            set {
                if (fItemsPadding != value) {
                    fItemsPadding = value;
                    UpdateView();
                }
            }
        }

        public TreemapModel Model
        {
            get {
                return fModel;
            }
        }

        public bool MouseoverHighlight
        {
            get {
                return fMouseoverHighlight;
            }
            set {
                if (fMouseoverHighlight != value) {
                    fMouseoverHighlight = value;
                    UpdateView();
                }
            }
        }

        public MapItem RootItem
        {
            get { return fRootItem; }
            set {
                if (fRootItem != value) {
                    fRootItem = value;
                    UpdateView();
                }
            }
        }

        public bool ShowNames
        {
            get {
                return fShowNames;
            }
            set {
                if (fShowNames != value) {
                    fShowNames = value;
                    UpdateView();
                }
            }
        }

        public MapItem UpperItem
        {
            get {
                return fUpperItem;
            }
        }

        public event HintRequestEventHandler OnHintRequest;


        public TreeMapViewer()
        {
            base.DoubleBuffered = true;
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            fItemsPadding = 4;
            fModel = new SimpleModel();

            fToolTip = new ToolTip();
            fToolTip.AutoPopDelay = 5000;
            fToolTip.InitialDelay = 250;
            fToolTip.ReshowDelay = 50;
            fToolTip.ShowAlways = true;

            fBackBuffer = null;
            fBorderPen = new Pen(Color.Black);
            fLowerPen = new Pen(fLowerHighlight);
            fUpperPen = new Pen(fUpperHighlight, 2);
        }

        private List<MapItem> GetRootList()
        {
            return (fRootItem == null) ? fModel.Items : fRootItem.Items;
        }

        public void UpdateView()
        {
            try {
                if (Width != 0 && Height != 0) {
                    List<MapItem> itemsList = GetRootList();
                    int headerHeight = (fShowNames) ? 10 : fItemsPadding;

                    fModel.CalcLayout(itemsList, new MapRect(0, 0, Width, Height), headerHeight, fItemsPadding);

                    fBackBuffer = null;
                    Invalidate();
                }
            } catch {
            }
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
            List<MapItem> itemsList = GetRootList();

            string hint = "";
            fUpperItem = null;
            fCurrentItem = fModel.FindByCoord(itemsList, e.X, e.Y, out fUpperItem);
            if (fCurrentItem != null) {
                hint = HintRequest(fCurrentItem);
            }

            if (fHint != hint) {
                fHint = hint;
                if (string.IsNullOrEmpty(fHint)) {
                    fToolTip.Hide(this);
                } else {
                    fToolTip.Show(hint, this, e.X, e.Y, 3000);
                }
            }

            if (fMouseoverHighlight) {
                fUpperHoveredItem = fUpperItem;
                fLowerHoveredItem = fCurrentItem;
                Invalidate();
            }

            base.OnMouseMove(e);
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
                    List<MapItem> itemsList = GetRootList();
                    if (itemsList.Count > 0) {
                        DrawItems(backGfx, itemsList);
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

        protected void DrawItems(Graphics gfx, IList<MapItem> items)
        {
            int num = items.Count;
            for (int i = 0; i < num; i++) {
                MapItem item = items[i];
                DrawItem(gfx, item);
            }
        }

        protected virtual void DrawItem(Graphics gfx, MapItem item)
        {
            var rect = item.Bounds;
            if (rect.W > 2 && rect.H > 2) {
                var simpleItem = (SimpleItem)item;
                gfx.FillRectangle(new SolidBrush(simpleItem.Color), rect.X, rect.Y, rect.W, rect.H);
                gfx.DrawRectangle(fBorderPen, rect.X, rect.Y, rect.W, rect.H);
            }

            DrawItems(gfx, item.Items);
        }
    }
}
