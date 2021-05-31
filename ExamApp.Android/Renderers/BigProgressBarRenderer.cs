using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ExamApp.Droid.Renderers;
using ExamApp.View.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BigProgressBar), typeof(BigProgressBarRenderer))]
namespace ExamApp.Droid.Renderers
{
    public class BigProgressBarRenderer : ProgressBarRenderer
    {
        public BigProgressBarRenderer(Context context) : base(context)
        {
        }
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ProgressBar> e)
        {
            base.OnElementChanged(e);


            //Control.ProgressTintList = Android.Content.Res.ColorStateList.ValueOf(Color.FromRgb(182, 231, 233).ToAndroid()); //Change the color
            Control.ScaleY = 10; //Changes the height

        }
    }
}