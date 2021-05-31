using ExamApp.Common;
using Xamarin.Forms;

namespace ExamApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            StartUp.splash = new SplashPage();
            MainPage = StartUp.splash;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
