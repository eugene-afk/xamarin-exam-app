using ExamApp.Common;
using ExamApp.Data.Model;
using ExamApp.Data.SQlite;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ExamApp.ViewModel
{
    public class HistoryCollection : ObservableCollection<ResultView>, IScrollable
    {
        private List<ResultView> _collectionTemp;
        public CollectionLoadMore loadMore { get; set; }

        public HistoryCollection()
        {
            _collectionTemp = new List<ResultView>();
            loadMore = new CollectionLoadMore(this);
            Task.Run(async () => await loadMore.LoadMore());
        }

        public async Task LoadItems()
        {
            await Task.Run(async () =>
            {
                await Task.Delay(350);
                foreach (var i in _collectionTemp)
                {
                    this.Add(i);
                }
            });
        }

        public async Task<int> ItemsCount(int limit, int offset)
        {
            _collectionTemp.Clear();
            _collectionTemp = await SQLiteTools.repo.GetReultsByUserID(CommonData.userID, limit: limit, offset: offset);
            return _collectionTemp.Count;
        }

    }
}
