using System;
using Xamarin.Forms;

namespace RespondentProfileQuota
{
   public class CustomSwitch : Switch
    {
        public static readonly BindableProperty TextOnProperty = BindableProperty.Create<CustomSwitch, string>(p => p.TextOn, "On");
        public static readonly BindableProperty TextOffProperty = BindableProperty.Create<CustomSwitch, string>(p => p.TextOff, "Off");

        public string TextOn
        {
            get { return (string)GetValue(TextOnProperty); }
            set { SetValue(TextOnProperty, value); }
        }

        public string TextOff
        {
            get { return (string)GetValue(TextOffProperty); }
            set { SetValue(TextOffProperty, value); }
        }
    }
}
