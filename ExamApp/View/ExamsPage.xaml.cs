using ExamApp.Data.Model;
using ExamApp.ViewModel;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ExamApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExamsPage : ContentPage
    {
        private ExamsViewModel _vm;
        public ExamsPage()
        {
            InitializeComponent();
            _vm = new ExamsViewModel(this);
            BindingContext = _vm;
            examsList.ItemTapped += (object sender, ItemTappedEventArgs e) => {
                if (e.Item == null) return;

                if (sender is ListView lv) lv.SelectedItem = null;
            };
        }

        private async void MoreButton_Clicked(object sender, EventArgs e)
        {
            Exam exam = ((ImageButton)sender).BindingContext as Exam;
            await _vm.ActionSheet(exam);
        }

        private async void PlayButton_Clicked(object sender, EventArgs e)
        {
            Exam exam = ((ImageButton)sender).BindingContext as Exam;
            await Application.Current.MainPage.Navigation.PushAsync(new ExamPage(exam.id, exam.name));
        }
    }
}