using ExamApp.Common.AppMasterDetail;
using ExamApp.Data.SQlite;
using ExamApp.View;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ExamApp.Common
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SplashPage : ContentPage
    {
        public SplashPage()
        {
            AbsoluteLayout splashLayout = new AbsoluteLayout
            {
                HeightRequest = 600
            };

            var image = new Image()
            {
                Source = ImageSource.FromFile("examapp.png"),
                Scale = 0.5,
                WidthRequest = 100,
                HeightRequest = 100
            };

            Content = new StackLayout()
            {
                Children = { splashLayout }
            };

            AbsoluteLayout.SetLayoutFlags(image, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(image, new Rectangle(0f, 0f,
             1f, 1f));

            splashLayout.Children.Add(image);

            Content = new StackLayout()
            {
                Children = { splashLayout }
            };

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await StartUp.Init();

            NavigationPage navPage;
            Tuple<bool, int> tuple = await SQLiteTools.repo.IsSigned();
            navPage = new NavigationPage(new LoginPage());
            if (tuple.Item1)
            {
                CommonData.userID = tuple.Item2;
                navPage = new NavigationPage(new AppMasterDetailPage());
            }
            Application.Current.MainPage = navPage;
        }
    

    }
}