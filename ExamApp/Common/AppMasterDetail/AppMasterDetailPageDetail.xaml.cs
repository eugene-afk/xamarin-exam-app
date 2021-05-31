using ExamApp.Data.Model;
using ExamApp.View;
using ExamApp.ViewModel;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ExamApp.Common.AppMasterDetail
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppMasterDetailPageDetail : ContentPage
    {
        private MainPageViewModel _vm;
        public AppMasterDetailPageDetail()
        {
            InitializeComponent();
            _vm = new MainPageViewModel(this);
            BindingContext = _vm;
        }

        private async void ContentPage_Appearing(object sender, EventArgs e)
        {
            try
            {
                await _vm.Init();
            }
            catch(Exception ex)
            {
                Debug.Print("*ContentPage_Appearing* msg: " + ex);
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            ResultView res = ((StackLayout)sender).BindingContext as ResultView;
            await this.Navigation.PushAsync(new HistoryDetailedPage(res.id, res.dateEND.ToString("g") + "|" + res.label, res.examID));
        }
    }
}