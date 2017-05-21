using System;
using RespondentProfileQuota;
using RespondentProfileQuota.Droid;
using Xamarin.Forms;

[assembly: Xamarin.Forms.ExportRenderer(typeof(CustomSwitch), typeof(CustomSwitchRenderer))]
namespace RespondentProfileQuota.Droid
{
    using Xamarin.Forms.Platform.Android;

    using Switch = Android.Widget.Switch;

    public class CustomSwitchRenderer : ViewRenderer<CustomSwitch, Switch>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<CustomSwitch> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || this.Element == null)
            {
                return;
            }

            var customSwitch = this.Element;

            var control = new Switch(Forms.Context)
            {
                TextOn = customSwitch.TextOn,
                TextOff = customSwitch.TextOff
            };


            this.SetNativeControl(control);
        }
     }
}
