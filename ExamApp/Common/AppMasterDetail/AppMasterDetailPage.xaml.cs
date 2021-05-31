using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ExamApp.Common.AppMasterDetail
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppMasterDetailPage : MasterDetailPage
    {
        public AppMasterDetailPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as AppMasterDetailPageMasterMenuItem;
            if (item == null)
                return;

            //var page = (Page)Activator.CreateInstance(item.TargetType);
            //page.Title = item.Title;
            Type page = item.TargetType;

            //Detail = new NavigationPage(page);
            Detail.Navigation.PushAsync((Page)Activator.CreateInstance(page));
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }
    }
}