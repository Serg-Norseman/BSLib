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

namespace BSLib.Controls.Validation
{
    public class RequiredFieldValidator : ValidatorBase
    {
        internal const string TEXT_FIELD_REQUIRE = "Require text value";

        public RequiredFieldValidator()
        {
            ErrorMessage = TEXT_FIELD_REQUIRE;
        }

        protected override bool EvaluateIsValid()
        {
            string text = ControlToValidate.Text.Trim();
            return !string.IsNullOrEmpty(text);
        }
    }
}
