using ExamApp.Data.Model;
using ExamApp.Data.SQlite;
using ExamApp.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ExamApp.ViewModel
{
    public class ExamViewModel : Base
    {
        #region Properties

        private string _currentQName;
        public string currentQName
        {
            get { return _currentQName; }
            set { _currentQName = value; OnPropertyChanged(); }
        }
        public int totalQCount { get; set; }
        private int _qCount;
        public int qCount
        {
            get
            {
                return _qCount;
            }
            set
            {
                _qCount = value;
                OnPropertyChanged();
            }
        }
        private string _btnText = "next";
        public string btnText
        {
            get { return _btnText; }
            set { _btnText = value; OnPropertyChanged(); }
        }
        private bool _successMsgVisible = false;
        public bool successMsgVisible
        {
            get { return _successMsgVisible; }
            set { _successMsgVisible = value; OnPropertyChanged(); }
        }
        private bool _mainVisible = true;
        public bool mainVisible
        {
            get { return _mainVisible; }
            set { _mainVisible = value; OnPropertyChanged(); }
        }

        public ICommand NextCommand { protected set; get; }
        public ICommand FinishCommand { protected set; get; }
        public ICommand ExitCommand { protected set; get; }
        public ICommand ShowDetailsCommand { protected set; get; }

        private int _examID;
        public string examName { get; set; }
        public List<ExamQuestion> questions { get; set; }
        public ObservableCollection<ExamQuestionVariant> variants { get; set; }
        private Result result;
        private ResultView _resultView;
        public ResultView resultView
        {
            get { return _resultView; }
            set { _resultView = value; OnPropertyChanged(); }
        }
        private List<ResultBody> results;

        #endregion

        public ExamViewModel(int id, string name)
        {
            _examID = id;
            examName = name;

            result = new Result();
            result.examID = _examID;
            result.userID = Common.CommonData.userID;
            results = new List<ResultBody>();
            variants = new ObservableCollection<ExamQuestionVariant>();
            questions = new List<ExamQuestion>();

            NextCommand = new Command(async () => { await Next(); });
            FinishCommand = new Command(async () => { await Finish(); });
            ExitCommand = new Command(async () => { await Exit(); });
            ShowDetailsCommand = new Command(async () => { await ShowDetails(); });

            Task.Run(async () => await GetQuestions()).Wait();
            totalQCount = questions.Count;
            Task.Run(async () => await GetQuestionVariants(questions[0].id));

            qCount = 1;
            currentQName = questions[0].name;
        }

        #region Commands

        internal async Task Next()
        {
            if(variants.Where(i => i.isChecked == true).Any())
            {
                ResultBody rb = new ResultBody();
                rb.chosenVariantID = variants.Where(i => i.isChecked == true).First().id;
                rb.questionID = questions[qCount - 1].id;
                results.Add(rb);
                qCount++;
                if (qCount <= totalQCount)
                {
                    currentQName = questions[qCount - 1].name;
                    await GetQuestionVariants(questions[qCount - 1].id);
                    if (totalQCount == qCount)
                    {
                        btnText = "finish";
                    }
                    return;
                }
                await Finish();
                return;
            }
            await Application.Current.MainPage.DisplayAlert("Error", "You need to choose answer to go next.", "OK");
        }

        private async Task Finish()
        {
            int rightAnswers = 0;
            foreach (var i in results)
            {
                var r = await SQLiteTools.repo.GetQuestionRightVariantByQuestionID(i.questionID);
                if (r == i.chosenVariantID)
                {
                    i.isRight = true;
                    rightAnswers++;
                }
            }
            result.percent = (int)Math.Round((double)(100 * rightAnswers) / totalQCount);
            result.dateEND = DateTime.Now;

            await SQLiteTools.repo.InsertResult(result);

            results.Select(i => { i.headID = result.id; return i; }).ToList();
            await SQLiteTools.repo.InsertResultBodiesRange(results);
            mainVisible = false;
            if(result.id != 0)
            {
                var ex = await SQLiteTools.repo.GetExamByID(_examID);
                ex.playedTimes += 1;
                await SQLiteTools.repo.UpdateExam(ex);

                successMsgVisible = true;
                resultView = await SQLiteTools.repo.GetResultByID(result.id);
            }
        }

        #endregion

        #region Methods
        private async Task GetQuestions()
        {
            questions = await SQLiteTools.repo.GetQuestionsByExamID(_examID);
        }

        internal async Task GetQuestionVariants(int id)
        {
            variants.Clear();
            var list = await SQLiteTools.repo.GetQuestionVariantsByQuestionID(id);
            foreach(var i in list)
            {
                variants.Add(i);
            }
        }

        private async Task Exit()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private async Task ShowDetails()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new HistoryDetailedPage(result.id, result.dateEND.ToString("g") + "|" + examName, result.examID));
        }
        #endregion
    }
}
