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
using NUnit.Framework;

namespace BSLib
{
    [TestFixture]
    public class TypeHandlerTests
    {
        [Test]
        public void Test_General()
        {
            Assert.Throws(typeof(ArgumentNullException), () => { new ObjHandler(null); });

            object obj = new object();
            var objHandler = new ObjHandler(obj);
            Assert.IsNotNull(objHandler);
            Assert.AreEqual(obj, objHandler.Handle);
        }

        private sealed class ObjHandler: TypeHandler<Object>
        {
            public ObjHandler(Object handle) : base(handle)
            {
            }
        }
    }
}
