using ExamApp.Common;
using ExamApp.Common.AppMasterDetail;
using ExamApp.Data.Model;
using ExamApp.Data.SQlite;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ExamApp.ViewModel
{
    public class MainPageViewModel : Base
    {
        #region Properties

        public string colorPB { get; set; }
        private int _AVGResult;
        public decimal AVGResultPB { get; set; }
        public int AVGResult 
        {
            get
            {
                return _AVGResult;
            }
            set
            {
                _AVGResult = value;
                OnPropertyChanged();
                if(_AVGResult >= 70)
                {
                    colorPB = "#34A853";
                }
                else if(_AVGResult < 70 && _AVGResult >= 40)
                {
                    colorPB = "#EF913D";
                }
                else
                {
                    colorPB = "#EA4335";
                }

                AVGResultPB = (decimal)_AVGResult / 100;
                OnPropertyChanged("AVGResultPB");
                OnPropertyChanged("colorPB");
            } 
        }
        private AppMasterDetailPageDetail _page;
        public ObservableCollection<ResultView> topResults { get; set; }
        private ResultView _bestResult;
        public ResultView bestResult 
        {
            get { return _bestResult; }
            set { _bestResult = value; OnPropertyChanged(); }
        }
        private ResultView _worstResult;
        public ResultView worstResult 
        {
            get { return _worstResult; }
            set { _worstResult = value; OnPropertyChanged(); }
        }
        private ResultView _lastResult;
        public ResultView lastResult
        {
            get { return _lastResult; }
            set { _lastResult = value; OnPropertyChanged(); }
        }
        public string moreButtonText { get; set; } = "More";

        private bool _isFullStats = false;

        public bool isFullStats
        {
            get { return _isFullStats; }
            set 
            {
                _isFullStats = value;
                if (!_isFullStats)
                {
                    moreButtonText = "More";
                    topResults.Clear();
                }
                else
                {
                    moreButtonText = "Hide";
                    FillTopResults();
                }
                OnPropertyChanged();
                OnPropertyChanged("moreButtonText");
            }
        }

        public ICommand showMoreCommand { protected set; get; }
        #endregion

        public MainPageViewModel(AppMasterDetailPageDetail page)
        {
            _page = page;
            topResults = new ObservableCollection<ResultView>();
            showMoreCommand = new Command(ToggleMore);
        }

        #region Methods
        private void ToggleMore()
        {
            isFullStats = !isFullStats;
            
        }

        private async void FillTopResults()
        {
            var list = await SQLiteTools.repo.GetReultsByUserID(Common.CommonData.userID, true);
            foreach (var i in list)
            {
                topResults.Add(i);
            }
            worstResult = await SQLiteTools.repo.GetReultByUserID(CommonData.userID);
            bestResult = await SQLiteTools.repo.GetReultByUserID(CommonData.userID, "desc");
        }

        public async Task Init()
        {

            AVGResult = await SQLiteTools.repo.GetAVGResultByUserID(CommonData.userID);
            if(AVGResult == 0)
            {
                return;
            }
            lastResult = await SQLiteTools.repo.GetReultByUserID(CommonData.userID, "desc", true);
        }
        #endregion

    }
}
