/*
 *  "BSLib".
 *  Copyright (C) 2009-2017 by Sergey V. Zhdanovskih.
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
using System.Reflection;

namespace BSLib
{
    /// <summary>
    /// 
    /// </summary>
    public static class ReflectionHelper
    {
        private static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            PropertyInfo propInfo;
            do {
                propInfo = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                type = type.BaseType;
            } while (propInfo == null && type != null);
            return propInfo;
        }

        public static object GetPropertyValue(object obj, string propertyName)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            Type objType = obj.GetType();
            PropertyInfo propInfo = GetPropertyInfo(objType, propertyName);
            if (propInfo == null)
                throw new ArgumentOutOfRangeException("propertyName",
                                                      string.Format("Couldn't find property {0} in type {1}", propertyName, objType.FullName));
            return propInfo.GetValue(obj, null);
        }

        public static void SetPropertyValue(object obj, string propertyName, object val)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            Type objType = obj.GetType();
            PropertyInfo propInfo = GetPropertyInfo(objType, propertyName);
            if (propInfo == null)
                throw new ArgumentOutOfRangeException("propertyName",
                                                      string.Format("Couldn't find property {0} in type {1}", propertyName, objType.FullName));
            propInfo.SetValue(obj, val, null);
        }


        public static object GetFieldValue(object obj, string fieldName)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            Type objType = obj.GetType();
            FieldInfo fieldInfo = objType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (fieldInfo == null)
                throw new ArgumentOutOfRangeException("fieldName",
                                                      string.Format("Couldn't find field {0} in type {1}", fieldName, objType.FullName));

            return fieldInfo.GetValue(obj);
        }


        private const BindingFlags AllBindings = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        public static FieldInfo FindFieldInfo(Type t, string fieldName)
        {
            foreach (FieldInfo fi in t.GetFields(AllBindings))
            {
                if (fi.Name == fieldName)
                {
                    return fi;
                }
            }

            return t.BaseType != null ? FindFieldInfo(t.BaseType, fieldName) : null;
        }
    }
}
