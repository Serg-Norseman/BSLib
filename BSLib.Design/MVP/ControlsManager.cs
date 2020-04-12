/*
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
using System.Collections.Generic;

namespace BSLib.Design.MVP
{
    public sealed class ControlsManager
    {
        private static readonly Dictionary<Type, Type> fHandlerTypes = new Dictionary<Type, Type>();

        private readonly Dictionary<object, IControl> fHandlers = new Dictionary<object, IControl>();

        public T GetControlHandler<T>(object control) where T : class, IControl
        {
            IControl handler;
            if (!fHandlers.TryGetValue(control, out handler)) {
                Type controlType = control.GetType();
                Type handlerType;
                if (fHandlerTypes.TryGetValue(controlType, out handlerType)) {
                    handler = (IControl)Activator.CreateInstance(handlerType, control);
                    fHandlers.Add(control, handler);
                } else {
                    throw new Exception("handler type not found");
                }
            }
            return (T)handler;
        }

        public static void RegisterHandlerType(Type controlType, Type handlerType)
        {
            if (fHandlerTypes.ContainsKey(controlType)) {
                fHandlerTypes.Remove(controlType);
            }
            fHandlerTypes.Add(controlType, handlerType);
        }
    }
}
