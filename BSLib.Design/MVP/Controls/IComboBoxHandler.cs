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
using System.Collections;
using System.Collections.Generic;
using BSLib.Design.MVP;

namespace BSLib.Design.MVP.Controls
{
    public interface IComboBoxHandler : IBaseControl
    {
        IList Items { get; }
        bool ReadOnly { get; set; }
        int SelectedIndex { get; set; }
        object SelectedItem { get; set; }
        string Text { get; set; }

        void Add(object item);
        void AddItem<T>(string caption, T tag);
        void AddRange(object[] items, bool sorted = false);
        void AddRange(IList<string> items, bool sorted = false);
        void AddStrings(StringList strings);
        void BeginUpdate();
        void Clear();
        void EndUpdate();
        void SortItems();

        T GetSelectedTag<T>();
        void SetSelectedTag<T>(T tagValue);
    }
}
