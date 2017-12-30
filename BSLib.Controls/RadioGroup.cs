/*
 *  "BSLib.Controls".
 *  Copyright (C) 2017 by Sergey V. Zhdanovskih.
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace BSLib.Controls
{
    /// <summary>
    /// Group of radio-buttons.
    /// </summary>
    public class RadioGroup : GroupBox
    {
        private class RadioItem
        {
            public string Text;
            public Rectangle Rect;

            public RadioItem(string text)
            {
                Text = text;
            }
        }


        private const bool DefaultEvenly = true;

        private int fColumns;
        private int fDownedIndex;
        private bool fEvenly;
        private int fHoveredIndex;
        private RadioItem[] fItems;
        private int fSelectedIndex;


        public RadioGroup()
        {
            DoubleBuffered = true;

            fColumns = 1;
            fDownedIndex = -1;
            fEvenly = DefaultEvenly;
            fHoveredIndex = -1;
            fItems = new RadioItem[0];
            fSelectedIndex = -1;
        }


        public string[] Items
        {
            get {
                if (fItems == null) {
                    return null;
                }
                string[] result = new string[fItems.Length];
                for (int i = 0; i < fItems.Length; i++) {
                    result[i] = fItems[i].Text;
                }
                return result;
            }
            set {
                RecreateItems(value);
                CalcLayout();
                Invalidate();
            }
        }

        [Browsable(true)]
        [DefaultValue(DefaultEvenly)]
        public virtual bool Evenly
        {
            get {
                return fEvenly;
            }
            set {
                if (fEvenly != value) {
                    fEvenly = value;
                    CalcLayout();
                    Invalidate();
                }
            }
        }

        public int Columns
        {
            get {
                return fColumns;
            }
            set {
                if (value < 1)
                    fColumns = 1;
                else
                    fColumns = value;

                CalcLayout();
                Invalidate();
            }
        }

        [Browsable(false)]
        public int SelectedIndex
        {
            get { return fSelectedIndex; }
            set {
                int n;
                if (Count == 0)
                    n = -1;
                else if (value < 0)
                    n = -1;
                else if (value >= Count)
                    n = Count - 1;
                else
                    n = value;

                if (n != fSelectedIndex) {
                    fSelectedIndex = n;
                    CalcLayout();
                    Invalidate();

                    var eventHandler = SelectedIndexChanged;
                    if (eventHandler != null) {
                        eventHandler(this, new EventArgs());
                    }
                }
            }
        }

        [Browsable(false)]
        public int Count { get { return fItems.Length; } }

        [Browsable(false)]
        public string this[int index]
        {
            get {
                if (Count > 0 && index > -1 && index < Count)
                    return fItems[index].Text;
                else
                    return "";
            }
            set {
                if (Count > 0 && index > -1 && index < Count)
                    fItems[index].Text = value;
            }
        }

        public event EventHandler SelectedIndexChanged;


        public void AddItem(string text)
        {
            if (text.Length > 0) {
                int num = fItems.Length;
                Array.Resize(ref fItems, num + 1);
                fItems[num] = new RadioItem(text);

                CalcLayout();
                Invalidate();
            }
        }


        private void RecreateItems(string[] items)
        {
            int num = items.Length;
            fItems = new RadioItem[items.Length];
            for (int i = 0; i < num; i++) {
                fItems[i] = new RadioItem(items[i]);
            }
        }

        private void CalcLayout()
        {
            Rectangle cr = ClientRectangle;
            cr.Y = cr.Y + 10;
            cr.Inflate(-10, -10);

            int nRow = Count / Columns;
            if (nRow == 0)
                return;
            if (Count % Columns > 0)
                nRow++;

            int h = (cr.Bottom - cr.Top) / nRow;
            int w = (cr.Right - cr.Left) / Columns;
            cr.Height = h;
            cr.Width = w;

            for (int j = 0; j < nRow; j++) {
                for (int i = 0; i < Columns; i++) {
                    int n = i + Columns * j;
                    if (n >= Count)
                        break;

                    Rectangle itemRt = cr;
                    itemRt.Offset(w * i, h * j);
                    fItems[n].Rect = itemRt;
                }
            }
        }


        protected override void OnSizeChanged(EventArgs e)
        {
            CalcLayout();

            base.OnSizeChanged(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            fDownedIndex = -1;

            if (e.Button == MouseButtons.Left) {
                for (int i = 0; i < Count; i++)
                    if (fItems[i].Rect.Contains(e.Location)) {
                    fDownedIndex = i;
                    Invalidate();
                    break;
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Left) {
                for (int i = 0; i < Count; i++)
                    if (fItems[i].Rect.Contains(e.Location)) {
                    if (fDownedIndex == i)
                        SelectedIndex = i;
                    fDownedIndex = -1;
                    Invalidate();
                    break;
                }
            }

            fDownedIndex = -1;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            for (int i = 0; i < Count; i++)
                if (fItems[i].Rect.Contains(e.Location)) {
                fHoveredIndex = i;
                Invalidate();
                break;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            fHoveredIndex = -1;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            RadioButtonState state;
            TextFormatFlags flags = TextFormatFlags.VerticalCenter;

            for (int i = 0; i < Count; i++) {
                if (i == fSelectedIndex) {
                    if (Enabled) {
                        if (i == fDownedIndex)
                            state = RadioButtonState.CheckedPressed;
                        else if (i == fHoveredIndex)
                            state = RadioButtonState.CheckedHot;
                        else
                            state = RadioButtonState.CheckedNormal;
                    } else
                        state = RadioButtonState.CheckedDisabled;
                } else {
                    if (Enabled) {
                        if (i == fDownedIndex)
                            state = RadioButtonState.UncheckedPressed;
                        else if (i == fHoveredIndex)
                            state = RadioButtonState.UncheckedHot;
                        else
                            state = RadioButtonState.UncheckedNormal;
                    } else
                        state = RadioButtonState.UncheckedDisabled;
                }

                Rectangle r = fItems[i].Rect;
                r.X = r.X + 18;

                Point p = fItems[i].Rect.Location;
                p.Y = p.Y + r.Height / 2 - 6;

                RadioButtonRenderer.DrawRadioButton(e.Graphics, p, r, fItems[i].Text, this.Font, flags, this.Focused, state);
            }
        }
    }
}