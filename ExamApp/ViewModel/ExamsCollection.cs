using ExamApp.Common;
using ExamApp.Data.Model;
using ExamApp.Data.SQlite;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ExamApp.ViewModel
{
    public class ExamsCollection : ObservableCollection<Exam>, IScrollable
    {
        private List<Exam> _collectionTemp;
        public CollectionLoadMore loadMore { get; set; }

        public ExamsCollection()
        {
            _collectionTemp = new List<Exam>();
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
                    Add(i);
                }
            });
        }

        public async Task<int> ItemsCount(int limit, int offset)
        {
            _collectionTemp.Clear();
            _collectionTemp = await SQLiteTools.repo.GetExams(limit, offset);
            return _collectionTemp.Count;
        }
    }
}
