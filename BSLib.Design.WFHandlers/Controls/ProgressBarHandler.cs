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

using System.Windows.Forms;
using BSLib.Design.MVP.Controls;

namespace BSLib.Design.Handlers
{
    public sealed class ProgressBarHandler : BaseControlHandler<ProgressBar, ProgressBarHandler>, IProgressBar
    {
        public ProgressBarHandler(ProgressBar control) : base(control)
        {
        }

        public int Minimum
        {
            get { return Control.Minimum; }
            set { Control.Minimum = value; }
        }

        public int Maximum
        {
            get { return Control.Maximum; }
            set { Control.Maximum = value; }
        }

        public int Value
        {
            get { return Control.Value; }
            set { Control.Value = value; }
        }

        public void Increment(int value)
        {
            Control.Increment(value);
        }
    }
}
