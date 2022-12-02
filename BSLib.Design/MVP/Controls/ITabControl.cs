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

namespace BSLib.Design.MVP.Controls
{
    public interface ITabPages : IControlItems<ITabPage>
    {
    }


    public interface ITabControl : IBaseControl
    {
        int SelectedIndex { get; set; }
        ITabPages Pages { get; }
    }
}
