using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content.PM;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(Diplom2.Droid.ChangeHotBar))]
namespace Diplom2.Droid
{
    public class ChangeHotBar:IChangeHotBar
    {
        public void ChengeHotBarByTheme( int R,int G, int B, int A)
        {
            Xamarin.Essentials.Platform.CurrentActivity.Window.SetStatusBarColor(Android.Graphics.Color.Argb(A, R, G, B));
        }
    }
}