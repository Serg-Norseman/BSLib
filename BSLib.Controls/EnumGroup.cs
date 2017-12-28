using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Indusoft.AF.Asset.DataReference
{
    public static class Conversion
    {
        public static string GetDescription(this object enumerationValue)
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum) {
                return enumerationValue.ToString();
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0) {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0) {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();

        }
    }

    public class EnumGroup : GroupBox, INotifyPropertyChanged
    {
        private List<RadioButton> fButtons;
        private Type fEnumType;
        private object fValue;
        private int fMaxTextWidth;

        public event PropertyChangedEventHandler PropertyChanged;

        [DefaultValue(0)]
        public object Value
        {
            get {
                return fValue;
            }
            set {
                for (int i = 0; i < fButtons.Count; i++) {
                    if (fButtons[i].Tag.Equals(value)) {
                        fButtons[i].Checked = true;
                        break;
                    }
                }

                fValue = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
        }

        public Type Enumtype
        {
            get { return fEnumType; }
            set {
                fMaxTextWidth = 0;
                this.Controls.Clear();
                if (fButtons == null)
                    fButtons = new List<RadioButton>();
                fButtons.Clear();
                Array strs = Enum.GetValues(value);
                for (int i = 0; i < strs.Length; i++) {
                    RadioButton obj = new RadioButton();
                    obj.Width = this.GetMaxTextWidth();
                    fButtons.Add(obj);
                    this.Controls.Add(obj);
                    obj.Text = strs.GetValue(i).GetDescription();
                    obj.Tag = strs.GetValue(i);
                    obj.Click += ButonClick;
                }
                OrderButtons();
                fEnumType = value;
            }
        }

        public EnumGroup()
        {
            DoubleBuffered = true;
            fMaxTextWidth = 0;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            OrderButtons();
            base.OnSizeChanged(e);
        }

        private int GetMaxTextWidth()
        {
            if (fMaxTextWidth == 0) {
                float mx = 150;
                if (fEnumType != null) {
                    using (var gfx = CreateGraphics()) {
                        Array strs = Enum.GetValues(fEnumType);
                        for (int i = 0; i < strs.Length; i++) {
                            string str = strs.GetValue(i).GetDescription();
                            mx = Math.Max(mx, gfx.MeasureString(str, this.Font).Width);
                        }
                    }
                }
                fMaxTextWidth = (int)mx;
            }

            return fMaxTextWidth;
        }

        private void OrderButtons()
        {
            try {
                int x = 5;
                int y = 20;
                for (int i = 0; i < fButtons.Count; i++) {
                    RadioButton obj = fButtons[i];
                    obj.Top = y;
                    obj.Left = x;
                    y += 20;
                    if (y + 20 > this.Height) {
                        y = 20;
                        x += GetMaxTextWidth();
                    }
                }
            } catch (Exception ex) {
            }
        }

        private void ButonClick(object sender, EventArgs e)
        {
            this.Value = (sender as RadioButton).Tag;
        }
    }
}
