﻿/*
 *  "BSLib.Design".
 *  Copyright (C) 2018-2020 by Sergey V. Zhdanovskih.
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
using BSLib.Design.Graphics;

namespace BSLib.Design.MVP.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public interface IListItem : IComparable
    {
        bool Checked { get; set; }
        object Data { get; set; }

        void AddSubItem(object itemValue);
        void SetBackColor(IColor color);
        void SetForeColor(IColor color);
        void SetFont(IFont font);
    }


    public interface IListViewItems
    {
        IListItem this[int index] { get; }
        int Count { get; }
    }


    /// <summary>
    /// 
    /// </summary>
    public interface IListView : IBaseControl
    {
        IListViewItems Items { get; }
        int SelectedIndex { get; set; }
        int SortColumn { get; set; }

        void AddCheckedColumn(string caption, int width, bool autoSize = false);
        void AddColumn(string caption, int width, bool autoSize);
        void AddColumn(string caption, int width, bool autoSize, BSDTypes.HorizontalAlignment textAlign);
        IListItem AddItem(object rowData, bool isChecked, params object[] columnValues);
        IListItem AddItem(object rowData, params object[] columnValues);
        void BeginUpdate();
        void Clear();
        void ClearColumns();
        void ClearItems();
        //void DeleteRecord(object data); // GK specific
        void EndUpdate();
        object GetSelectedData();
        void SelectItem(object rowData);
        void SetColumnCaption(int index, string caption); // GK specific
        void SetSortColumn(int sortColumn, bool checkOrder = true);
        void Sort(int sortColumn, BSDTypes.SortOrder sortOrder);
        void UpdateContents(bool columnsChanged = false); // GK specific
    }
}
