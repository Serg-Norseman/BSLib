﻿/*
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

using BSLib.Design.Graphics;

namespace BSLib.Design.MVP.Controls
{
    public delegate void ItemAction(IMenuItem sender);


    public interface IMenuItems : IControlItems<IMenuItem>
    {
    }


    /// <summary>
    /// 
    /// </summary>
    public interface IMenuItem : IControl
    {
        bool Checked { get; set; }
        bool Enabled { get; set; }
        IMenuItems SubItems { get; }
        object Tag { get; set; }
        string Text { get; set; }

        IMenuItem AddItem(string text, object tag, IImage image, ItemAction action);
        void ClearItems();
    }
}
