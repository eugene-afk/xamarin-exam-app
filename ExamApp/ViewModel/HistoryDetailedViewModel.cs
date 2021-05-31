using ExamApp.Data.SQlite;
using ExamApp.View;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ExamApp.ViewModel
{
    public class HistoryDetailedViewModel : Base
    {
        public HistoryDetailedCollection resultBodies { get; set; }
        public ICommand LoadMoreCommand { protected set; get; }
        public ICommand PassAgainCommand { protected set; get; }
        public string title { get; set; }
        private int _examID;

        public HistoryDetailedViewModel(int resultHeadID, string examName, int examID)
        {
            title = examName;
            _examID = examID;
            resultBodies = new HistoryDetailedCollection(resultHeadID);
            LoadMoreCommand = new Command(async () => await resultBodies.loadMore.LoadMore());
            PassAgainCommand = new Command(async () => await PassAgain());
        }

        private async Task PassAgain()
        {
            var e = await SQLiteTools.repo.GetExamByID(_examID);
            if (e.isDeleted)
            {
                await App.Current.MainPage.DisplayAlert("Error", "This exam was deleted.", "OK");
                return;
            }
            string name = title.Substring(title.IndexOf("|") + 1);
            await Application.Current.MainPage.Navigation.PushAsync(new ExamPage(_examID, name));
        }

    }
}
