using System.Windows.Input;
using Xamarin.Forms;

namespace ExamApp.ViewModel
{
    public class HistoryViewModel : Base
    {
        public HistoryCollection results { get; set; }
        public ICommand LoadMoreCommand { protected set; get; }

        public HistoryViewModel()
        {
            results = new HistoryCollection();
            LoadMoreCommand = new Command(async () => await results.loadMore.LoadMore());
        }

    }
}
