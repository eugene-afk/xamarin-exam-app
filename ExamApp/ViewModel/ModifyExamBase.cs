using ExamApp.Data.Model;
using System.Collections.ObjectModel;

namespace ExamApp.ViewModel
{
    public class ModifyExamBase : Base
    {
        private string _editableText;
        private string _pageTitle;

        public string editableText
        {
            get { return _editableText; }
            set { _editableText = value; OnPropertyChanged(); }
        }
        public string pageTitle
        {
            get { return _pageTitle; }
            set { _pageTitle = value; OnPropertyChanged(); }
        }

        public bool isEdit;

        public ObservableCollection<ExamQuestion> questions { get; set; }
        public Exam currentExam;
    }
}
