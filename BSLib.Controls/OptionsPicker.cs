/*
 *  "BSLib.Controls".
 *  Copyright (C) 2017-2018 by Sergey V. Zhdanovskih.
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
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace BSLib.Controls
{
    public class OptionsPicker : DropDownControl
    {
        private System.ComponentModel.IContainer components;
        private CheckedListBox fCheckedListBox;

        public event EventHandler OptionsChanged;

        public string[] Items
        {
            get {
                int num = fCheckedListBox.Items.Count;
                string[] result = new string[num];
                for (int i = 0; i < num; i++) {
                    result[i] = fCheckedListBox.Items[i].ToString();
                }
                return result;
            }
            set {
                RecreateItems(value);
                Invalidate();
            }
        }

        private void RecreateItems(string[] items)
        {
            int num = items.Length;
            fCheckedListBox.Items.AddRange(items);
            for (int i = 0; i < num; i++) {
                fCheckedListBox.SetItemChecked(i, false);
            }
        }

        public OptionsPicker()
        {
            InitializeComponent();
            InitializeDropDown(fCheckedListBox);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            SuspendLayout();

            fCheckedListBox = new CheckedListBox();
            fCheckedListBox.Location = new Point(0, 30);
            fCheckedListBox.Margin = new Padding(4, 4, 4, 4);
            fCheckedListBox.Name = "fCheckedListBox";
            fCheckedListBox.Size = new Size(243, 181);
            fCheckedListBox.TabIndex = 0;
            fCheckedListBox.CheckOnClick = true;
            fCheckedListBox.SelectedValueChanged += listView1_SelectedValue;

            AnchorSize = new Size(244, 21);
            AutoScaleDimensions = new SizeF(8F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(fCheckedListBox);
            Margin = new Padding(4, 4, 4, 4);
            Name = "OptionsPicker";
            Size = new Size(244, 215);
            ResumeLayout(false);
        }

        private void listView1_SelectedValue(object sender, EventArgs e)
        {
            if (DropState == DropDownState.Dropping || DropState == DropDownState.Closing) return;

            string text = "";
            IEnumerator checkedEnum = fCheckedListBox.CheckedIndices.GetEnumerator();
            while (checkedEnum.MoveNext())
            {
                int idx = (int)checkedEnum.Current;
                if (fCheckedListBox.GetItemChecked(idx)) {
                    text += fCheckedListBox.Items[idx] + ";";
                }
            }
            Text = text;

            //CloseDropDown();

            if (OptionsChanged != null)
                OptionsChanged(null, null);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }
    }
}
