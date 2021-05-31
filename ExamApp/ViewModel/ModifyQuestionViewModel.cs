using ExamApp.Data.Model;
using ExamApp.Data.SQlite;
using ExamApp.View;
using ExamApp.View.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ExamApp.ViewModel
{
    public class ModifyQuestionViewModel : ModifyExamBase
    {
        #region Properties

        internal ExamQuestion _currentQuestion;
        internal ExamQuestionVariant _currentVariant;
        private ModifyQuestionPage _page;
        public ObservableCollection<ExamQuestionVariant> variants { get; set; }

        public ICommand ModifyQuestionCommand { protected set; get; }
        public ICommand ModifyVariantCommand { protected set; get; }
        public ICommand NewQuestionCommand { protected set; get; }

        #endregion

        public ModifyQuestionViewModel(ObservableCollection<ExamQuestion> qs, Exam e, ExamQuestion question, ModifyQuestionPage page)
        {
            _page = page;
            questions = qs;
            currentExam = e;
            variants = new VariantsSortable();
            _currentQuestion = question;
            pageTitle = "Creating new question";
            if (_currentQuestion != null && _currentQuestion.id != 0)
            {
                Task.Run(async () => await GetQuestionData());
                editableText = _currentQuestion.name;
                pageTitle = "Editing " + editableText;
            }
            ModifyQuestionCommand = new Command(async () => await ModifyQuestion());
            ModifyVariantCommand = new Command(() => ModifyVariant());
            NewQuestionCommand = new Command(async () => await NewQuestion());
        }

        #region Commands

        internal async Task ModifyQuestion()
        {
            if (_currentQuestion != null && _currentQuestion.id != 0)
            {
                await UpdateQuestion();
                return;
            }
            if (string.IsNullOrEmpty(editableText))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Question name must contain at least one symbol.", "OK");
                return;
            }
            await CreateQuestion();
        }

        internal async void ModifyVariant(int id = 0)
        {
            if (_currentQuestion == null || _currentQuestion.id == 0)
            {
                await ModifyQuestion();
                if (_currentQuestion == null)
                {
                    return;
                }
            }

            if (id != 0)
            {
                await UpdateVariant(id);
                return;
            }
            await CreateVariantSheet();
        }

        private async Task NewQuestion()
        {
            if (_currentQuestion == null || _currentQuestion.id == 0)
            {
                await App.Current.MainPage.DisplayAlert("Error", "You don't save current question.", "OK");
                return;
            }
            _currentQuestion = new ExamQuestion();
            editableText = "";
            variants.Clear();
            pageTitle = "Creating new question";
        }

        #endregion

        #region Methods

        private async Task GetQuestionData()
        {
            var v = await SQLiteTools.repo.GetQuestionVariantsByQuestionID(_currentQuestion.id);
            foreach(var i in v)
            {
                variants.Add(i);
            }
        }

        private async Task UpdateQuestion()
        {
            if (_currentQuestion.name != editableText && editableText != "")
            {
                if (!questions.Where(i => i.name == editableText).Any())
                {
                    _currentQuestion.name = editableText;
                    await SQLiteTools.repo.UpdateQuestion(_currentQuestion);
                    pageTitle = "Editing " + editableText;
                    return;
                }
                await App.Current.MainPage.DisplayAlert("Error", "Question with that name already exist.", "OK");
                return;
            }
            editableText = _currentQuestion.name;
        }

        private async Task CreateQuestion()
        {
            if (!questions.Where(i => i.name == editableText).Any())
            {
                _currentQuestion = new ExamQuestion() { name = editableText };
                _currentQuestion.examID = currentExam.id;
                _currentQuestion.position = questions.Count + 1;
                await SQLiteTools.repo.InsertExamQuestion(_currentQuestion);
                questions.Add(_currentQuestion);
                pageTitle = "Editing " + editableText;
                return;
            }
            await App.Current.MainPage.DisplayAlert("Error", "Question with that name already exist.", "OK");
        }

        internal async Task UpdateVariant(int id)
        {
            _currentVariant = variants.Where(i => i.id == id).First();
            string result = await App.Current.MainPage.DisplayPromptAsync("Answer", "Edit answer here", initialValue: _currentVariant.name);
            if (string.IsNullOrEmpty(result))
            {
                return;
            }
            if (!variants.Where(i => i.name == result).Any())
            {
                _currentVariant.name = result;
                await SQLiteTools.repo.UpdateQuestionVariant(_currentVariant);
                return;
            }
            await App.Current.MainPage.DisplayAlert("Error", "Answer with that name already exist.", "OK");
        }

        private async Task CreateVariantSheet()
        {
            string result = await App.Current.MainPage.DisplayPromptAsync("Answer", "Type answer here");
            if (result == "" || result == null)
            {
                return;
            }
            if (!variants.Where(i => i.name == result).Any())
            {
                await CreateVariant(result);
                variants.Add(_currentVariant);
                return;
            }
            await App.Current.MainPage.DisplayAlert("Error", "Answer with that name already exist.", "OK");
        }

        internal async Task CreateVariant(string name)
        {
            _currentVariant = new ExamQuestionVariant();
            _currentVariant.position = variants.Count + 1;
            _currentVariant.name = name;
            _currentVariant.questionID = _currentQuestion.id;
            if (variants.Count == 0)
            {
                _currentVariant.isRight = true;
            }
            await SQLiteTools.repo.InsertExamQuestionVariant(_currentVariant);
        }

        public async Task MakeRightVariant(ExamQuestionVariant v)
        {
            if (variants.Where(i => i.isRight == true).Any())
            {
                variants.Where(i => i.isRight == true).Select(i =>
                {
                    i.isRight = false;
                    Task.Run(async () => { await SQLiteTools.repo.UpdateQuestionVariant(i); });
                    return i;
                }).ToList();
            }
            v.isRight = true;
            await SQLiteTools.repo.UpdateQuestionVariant(v);
        }

        public async void DeleteVariant(ExamQuestionVariant v)
        {
            bool answer = await _page.DisplayAlert("Deleting answer", "You really want to permanently delete answer?", "Yes", "No");
            if (answer)
            {
                bool isPlayed = false;
                if(currentExam.playedTimes > 0)
                {
                    isPlayed = true;
                }
                int res = await SQLiteTools.repo.DeleteVariant(v, isPlayed);
                if (res != 0)
                {
                    variants.Remove(v);
                    await _page.DisplayAlert("Deleting answer", "Successfully deleted.", "OK");
                    return;
                }
                await _page.DisplayAlert("Deleting answer", "Failed.", "OK");
            }
        }

        #endregion
    }
}
