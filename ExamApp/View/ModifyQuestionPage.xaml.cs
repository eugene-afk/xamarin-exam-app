using ExamApp.Data.Model;
using ExamApp.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ExamApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ModifyQuestionPage : ContentPage
    {
        private ModifyQuestionViewModel _vm;
        public ModifyQuestionPage(ObservableCollection<ExamQuestion> qs, Exam e, ExamQuestion question)
        {
            InitializeComponent();
            _vm = new ModifyQuestionViewModel(qs, e, question, this);
            BindingContext = _vm;

            VariantsList.ItemTapped += (object sender, ItemTappedEventArgs ev) => {
                if (ev.Item == null) return;

                if (sender is ListView lv) lv.SelectedItem = null;
            };
        }

        private void RightVariant_Clicked(object sender, EventArgs e)
        {
            ExamQuestionVariant v = ((ImageButton)sender).BindingContext as ExamQuestionVariant;
            Task.Run(async () => { await _vm.MakeRightVariant(v); });
        }

        private void EditVariant_Clicked(object sender, EventArgs e)
        {
            ExamQuestionVariant v = ((ImageButton)sender).BindingContext as ExamQuestionVariant;
            _vm.ModifyVariant(v.id);
        }

        private void DeleteQuestionVariant_Clicked(object sender, EventArgs e)
        {
            ExamQuestionVariant v = ((ImageButton)sender).BindingContext as ExamQuestionVariant;
            _vm.DeleteVariant(v);
        }

    }
}