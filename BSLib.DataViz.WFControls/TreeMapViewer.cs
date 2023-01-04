/*
 *  "BSLib.DataViz".
 *  Copyright (C) 2017-2022 by Sergey V. Zhdanovskih.
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

    public class PaintItemEventArgs : EventArgs
    {
        private Graphics graphics;
        private readonly MapItem item;

        public Graphics Graphics
        {
            get { return graphics; }
        }

        public MapItem Item
        {
            get { return item; }
        }

        public PaintItemEventArgs(Graphics graphics, MapItem item)
        {
            if (graphics == null) {
                throw new ArgumentNullException("graphics");
            }

            this.graphics = graphics;
            this.item = item;
        }
    }

    public delegate void PaintItemEventHandler(object sender, PaintItemEventArgs args);

    /// <summary>
    /// TreeMap Viewer's control.
    /// </summary>
    public class TreeMapViewer : UserControl
    {
        private readonly ToolTip fToolTip;

        private Bitmap fBackBuffer;
        private Pen fBorderPen;
        private MapItem fCurrentItem;
        private Brush fHeaderBrush;
        private Color fHighlightColor;
        private Pen fHighlightPen;
        private string fHint;
        private MapItem fHoveredItem;
        private int fItemsPadding;
        private TreemapModel fModel;
        private bool fMouseoverHighlight;
        private MapItem fRootItem;
        private bool fShowNames;
        private MapItem fUpperItem;


        public Pen BorderPen
        {
            get { return fBorderPen; }
            set {
                if (fBorderPen != value) {
                    fBorderPen = value;
                    UpdateView();
                }
            }
        }

        public MapItem CurrentItem
        {
            get {
                return fCurrentItem;
            }
        }

        public Brush HeaderBrush
        {
            get { return fHeaderBrush; }
            set {
                if (fHeaderBrush != value) {
                    fHeaderBrush = value;
                    UpdateView();
                }
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
            set {
                fModel = value;
                UpdateView();
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

        public event HintRequestEventHandler HintRequest;

        public event PaintItemEventHandler PaintItem;


        public TreeMapViewer()
        {
            base.DoubleBuffered = true;
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            fItemsPadding = 4;
            fModel = new TreemapModel();
            fModel.CreatingItem += CreateSimpleItem;

            fToolTip = new ToolTip();
            fToolTip.AutoPopDelay = 5000;
            fToolTip.InitialDelay = 250;
            fToolTip.ReshowDelay = 50;
            fToolTip.ShowAlways = true;

            fBackBuffer = null;
            fBorderPen = new Pen(Color.Black);
            fHeaderBrush = new SolidBrush(Color.Black);
            fHighlightColor = Color.White;
            fHighlightPen = new Pen(fHighlightColor);
        }

        private MapItem CreateSimpleItem(MapItem parent, string name, double size)
        {
            return new SimpleItem(parent, name, size);
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
                    int headerHeight = (fShowNames) ? Font.Height : fItemsPadding;

                    fModel.CalcLayout(itemsList, new MapRect(0, 0, Width, Height), headerHeight, fItemsPadding);

                    fBackBuffer = null;
                    Invalidate();
                }
            } catch {
            }
        }

        protected virtual string OnHintRequest(MapItem mapItem)
        {
            var hintRequest = HintRequest;
            if (hintRequest == null) return mapItem.Name;

            HintRequestEventArgs args = new HintRequestEventArgs(mapItem);
            hintRequest(this, args);
            return args.Hint;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            List<MapItem> itemsList = GetRootList();

            string hint = "";
            fUpperItem = null;
            fCurrentItem = fModel.FindByCoord(itemsList, e.X, e.Y, out fUpperItem);
            if (fCurrentItem != null) {
                hint = OnHintRequest(fCurrentItem);
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
                fHoveredItem = fCurrentItem;
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
                        PaintItems(backGfx, itemsList);
                    } else {
                        backGfx.FillRectangle(new SolidBrush(Color.Silver), 0, 0, Width, Height);
                    }
                }
            }

            gfx.DrawImage(fBackBuffer, 0, 0);

            if (fMouseoverHighlight) {
                if (fHoveredItem != null) {
                    var rect = fHoveredItem.Bounds;
                    gfx.DrawRectangle(fHighlightPen, rect.X, rect.Y, rect.W, rect.H);
                }
            }
        }

        protected virtual void PaintItems(Graphics gfx, IList<MapItem> items)
        {
            int num = items.Count;
            for (int i = 0; i < num; i++) {
                MapItem item = items[i];
                OnPaintItem(gfx, item);
            }
        }

        protected RectangleF ToRectangle(MapRect rect)
        {
            return new RectangleF(rect.X, rect.Y, rect.W, rect.H);
        }

        protected virtual void OnPaintItem(Graphics gfx, MapItem item)
        {
            var handler = PaintItem;
            if (handler != null) {
                handler(this, new PaintItemEventArgs(gfx, item));
            } else {
                var rect = item.Bounds;
                if (rect.W > 2 && rect.H > 2) {
                    var simpleItem = (SimpleItem)item;
                    gfx.FillRectangle(new SolidBrush(simpleItem.Color), rect.X, rect.Y, rect.W, rect.H);
                    gfx.DrawRectangle(fBorderPen, rect.X, rect.Y, rect.W, rect.H);
                    gfx.DrawString(simpleItem.Name, Font, fHeaderBrush, ToRectangle(rect));
                }
            }

            PaintItems(gfx, item.Items);
        }
    }
}
