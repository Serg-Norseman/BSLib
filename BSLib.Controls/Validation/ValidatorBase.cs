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

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace BSLib.Controls.Validation
{
    public abstract class ValidatorBase : Component
    {
        private Control fControlToValidate;
        private string fErrorMessage = "";
        private readonly ErrorProvider fErrorProvider;
        private bool fIsValid;

        [Description("Gets or sets the text for the error message."), DefaultValue(""), Category("Appearance")]
        public string ErrorMessage
        {
            get
            {
                return fErrorMessage;
            }
            set
            {
                fErrorMessage = value;
            }
        }

        [Category("Behaviour"), DefaultValue(null), TypeConverter(typeof(ValidatableControlConverter)), Description("Gets or sets the input control to validate.")]
        public Control ControlToValidate
        {
            get
            {
                return fControlToValidate;
            }
            set
            {
                fControlToValidate = value;
                if (fControlToValidate == null || DesignMode) return;

                fControlToValidate.Validating += ControlToValidate_Validating;
                fControlToValidate.Validated += ControlToValidate_Validated;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsValid
        {
            get
            {
                return fIsValid;
            }
            set
            {
                fIsValid = value;
            }
        }

        protected ValidatorBase()
        {
            fErrorProvider = new ErrorProvider();
            fErrorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
        }

        protected abstract bool EvaluateIsValid();

        private void ControlToValidate_Validating(object sender, CancelEventArgs e)
        {
            fIsValid = EvaluateIsValid();
            if (fIsValid) return;

            fErrorProvider.SetError(fControlToValidate, ErrorMessage);
            e.Cancel = true;
        }

        private void ControlToValidate_Validated(object sender, EventArgs e)
        {
            fErrorProvider.SetError(fControlToValidate, "");
        }
    }
}
