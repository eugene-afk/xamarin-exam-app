using ExamApp.Common;
using ExamApp.Data.Model;
using ExamApp.Data.SQlite;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ExamApp.ViewModel
{
    public class HistoryDetailedCollection : ObservableCollection<ResultBodyView>, IScrollable
    {
        private List<ResultBodyView> _collectionTemp;
        public CollectionLoadMore loadMore { get; set; }
        private int _headID = 0;

        public HistoryDetailedCollection(int headID)
        {
            _headID = headID;
            _collectionTemp = new List<ResultBodyView>();
            loadMore = new CollectionLoadMore(this);
            Task.Run(async () => await loadMore.LoadMore());
        }

        public async Task LoadItems()
        {
            await Task.Run(() =>
            {
                foreach (var i in _collectionTemp)
                {
                    this.Add(i);
                }
            });
        }

        public async Task<int> ItemsCount(int limit, int offset)
        {
            _collectionTemp.Clear();
            _collectionTemp = await SQLiteTools.repo.GetResultBodiesByResultHeadID(_headID, limit, offset);
            return _collectionTemp.Count;
        }

    }
}
