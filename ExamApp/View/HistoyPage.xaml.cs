using ExamApp.Data.Model;
using ExamApp.ViewModel;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ExamApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoyPage : ContentPage
    {
        public HistoyPage()
        {
            InitializeComponent();
            BindingContext = new HistoryViewModel();
            resultsList.ItemTapped += (object sender, ItemTappedEventArgs e) => {
                if (e.Item == null) return;

                if (sender is ListView lv) lv.SelectedItem = null;
            };
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            ResultView res = ((StackLayout)sender).BindingContext as ResultView;
            await this.Navigation.PushAsync(new HistoryDetailedPage(res.id, res.dateEND.ToString("g") + "|" + res.label, res.examID));
        }
    }
}