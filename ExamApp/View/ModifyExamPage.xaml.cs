using ExamApp.Data.Model;
using ExamApp.ViewModel;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ExamApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ModifyExamPage : ContentPage
    {
        private ModifyExamViewModel _vm;
        public ModifyExamPage(Exam exam, ObservableCollection<Exam> exams)
        {
            InitializeComponent();
            _vm = new ModifyExamViewModel(exam, exams, this);
            BindingContext = _vm;
            InitDeSelect();
        }

        public ModifyExamPage()
        {
            InitializeComponent();
            _vm = new ModifyExamViewModel(null, null, this);
            BindingContext = _vm;
            InitDeSelect();
        }

        public ModifyExamPage(ObservableCollection<Exam> exams)
        {
            InitializeComponent();
            _vm = new ModifyExamViewModel(null, exams, this);
            BindingContext = _vm;
        }

        private void EditQuestion_Clicked(object sender, EventArgs e)
        {
            ExamQuestion q = ((ImageButton)sender).BindingContext as ExamQuestion;
            _vm.OpenQuestionView(q);
        }

        private void InitDeSelect()
        {
            QuestionsList.ItemTapped += (object sender, ItemTappedEventArgs e) => {
                if (e.Item == null) return;

                if (sender is ListView lv) lv.SelectedItem = null;
            };
        }

        private void DeleteQuestion_Clicked(object sender, EventArgs e)
        {
            ExamQuestion q = ((ImageButton)sender).BindingContext as ExamQuestion;
             _vm.DeleteQuestion(q);
        }
    }
}