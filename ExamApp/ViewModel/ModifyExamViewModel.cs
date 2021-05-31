using ExamApp.Common;
using ExamApp.Data.Model;
using ExamApp.Data.SQlite;
using ExamApp.View;
using ExamApp.View.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ExamApp.ViewModel
{
    public class ModifyExamViewModel : ModifyExamBase
    {
        #region Properties
        private string _examPubState;
        public string examPubState
        {
            get { return _examPubState; }
            set { _examPubState = value; OnPropertyChanged(); }
        }
        private ObservableCollection<Exam> _exams;
        private ModifyExamPage _page;

        public ICommand ModifyExamCommand { protected set; get; }
        public ICommand ToggleDraftedStateCommand { protected set; get; }
        public ICommand TogglePublishStateCommand { protected set; get; }
        public ICommand OpenQuestionViewCommand { protected set; get; }

        #endregion

        public ModifyExamViewModel(Exam exam, ObservableCollection<Exam> exams, ModifyExamPage page)
        {
            _page = page;
            questions = new QuestionsSortable();
            _exams = exams;
            PrepareExam(exam);
            Task.Run(async () => await GetExamQuestions());
            ModifyExamCommand = new Command(async () => await ModifyExam());
            ToggleDraftedStateCommand = new Command(async () => await ToggleDraftedState());
            TogglePublishStateCommand = new Command(async () => await TogglePublishState());
            OpenQuestionViewCommand = new Command(() => OpenQuestionView());
        }

        #region Commands

        internal async Task ModifyExam()
        {
            if (string.IsNullOrEmpty(editableText))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Exam name must contain at least one symbol.", "OK");
                return;
            }
            if (currentExam == null)
            {
                currentExam = new Exam() 
                {
                    name = editableText 
                };
                currentExam.isDraft = true;
                currentExam.ownerID = CommonData.userID;
                await SQLiteTools.repo.InsertExam(currentExam);
                editableText = currentExam.name;
                pageTitle = "Editing " + editableText;
                examPubState = "DRAFTED";
                if (_exams != null)
                {
                    _exams.Insert(0, currentExam);
                }
                return;
            }
            if (currentExam.id != 0 && editableText != currentExam.name)
            {
                pageTitle = pageTitle.Replace(currentExam.name, editableText);
                currentExam.name = editableText;
                await SQLiteTools.repo.UpdateExam(currentExam);
            }
        }

        private async Task ToggleDraftedState()
        {
            if (currentExam != null && !currentExam.isDraft)
            {
                examPubState = "DRAFTED";
                currentExam.isDraft = true;
                await SQLiteTools.repo.UpdateExam(currentExam);
            }
        }

        private async Task TogglePublishState()
        {
            if (questions.Count > 1)
            {
                if (currentExam != null && currentExam.isDraft)
                {
                    for(int i = 0; i < questions.Count; i++)
                    {
                        var v = await SQLiteTools.repo.GetQuestionVariantsByQuestionID(questions[i].id);
                        if (v.Count < 2)
                        {
                            await App.Current.MainPage.DisplayAlert("Error", "Question " + questions[i].name + " not contain enought answers.", "OK");
                            return;
                        }
                    }

                    examPubState = "PUBLISHED";
                    currentExam.isDraft = false;
                    await SQLiteTools.repo.UpdateExam(currentExam);
                }
                return;
            }
            await App.Current.MainPage.DisplayAlert("Error", "Exam not contain enought questions.", "OK");
        }

        public async void OpenQuestionView(ExamQuestion question = null)
        {
            if (currentExam == null)
            {
                await ModifyExam();
                if (currentExam == null)
                {
                    return;
                }
            }
            await _page.Navigation.PushAsync(new ModifyQuestionPage(questions, currentExam, question));
        }

        #endregion

        #region Methods
        private async Task GetExamQuestions()
        {
            var list = await SQLiteTools.repo.GetQuestionsByExamID(currentExam.id);
            foreach (var i in list)
            {
                questions.Add(i);
            }
        }

        private void PrepareExam(Exam exam)
        {
            if (exam == null)
            {
                examPubState = "";
                pageTitle = "Creating New Exam";
                return;
            }
            isEdit = true;
            currentExam = exam;

            examPubState = "PUBLISHED";
            if (currentExam.isDraft)
            {
                examPubState = "DRAFTED";
            }

            editableText = currentExam.name;
            pageTitle = "Editing " + editableText;
        }

        public async void DeleteQuestion(ExamQuestion q)
        {
            bool answer = await _page.DisplayAlert("Deleting question", "You really want to permanently delete question?", "Yes", "No");
            if (answer)
            {
                bool isPlayed = false;

                if(currentExam.playedTimes > 0)
                {
                    isPlayed = true;
                }

                int res = await SQLiteTools.repo.DeleteQuestion(q, isPlayed);
                if (res != 0)
                {
                    questions.Remove(q);
                    await _page.DisplayAlert("Deleting question", "Successfully deleted.", "OK");
                    return;
                }
                await _page.DisplayAlert("Deleting question", "Failed.", "OK");
            }
        }

        #endregion

    }
}
