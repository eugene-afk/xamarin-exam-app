using System;
using System.Linq;
using ExamApp.View.Controls;
using ExamApp.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ExamApp.Droid.Renderers;

[assembly: ResolutionGroupName("MediaLab")]
[assembly: ExportEffect(typeof(DropShadowEffect), "DropShadowEffect")]
namespace ExamApp.Droid.Renderers
{
    public class DropShadowEffect : PlatformEffect
    {
		protected override void OnAttached()
		{
			try
			{
				var control = Control ?? Container as Android.Views.View;

				var effect = (ViewShadowEffect)Element.Effects.FirstOrDefault(e => e is ViewShadowEffect);

				if (effect != null)
				{
					float radius = effect.Radius;
					Android.Graphics.Color color = effect.Color.ToAndroid();
					//control.SetShadowLayer(radius, distanceX, distanceY, color);

					control.Elevation = radius;
					control.TranslationZ = (effect.DistanceX + effect.DistanceY) / 2;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Cannot set property on attached control. Error: {0}", ex.Message);
			}
		}

		protected override void OnDetached()
		{
		}
	}
}