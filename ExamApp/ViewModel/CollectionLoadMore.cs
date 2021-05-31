using ExamApp.Common;
using System.Threading.Tasks;

namespace ExamApp.ViewModel
{
    public class CollectionLoadMore : Base
    {
        private IScrollable _collection;

        private int limit = 10;
        private int offset = -10;
        private bool listEnd = false;
        private string _loadMoreText = "load more";
        public string loadMoreText
        {
            get { return _loadMoreText; }
            set { _loadMoreText = value; OnPropertyChanged(); }
        }

        private bool _isLoadMoreVisible;
        public bool isLoadMoreVisible
        {
            get { return _isLoadMoreVisible; }
            set { _isLoadMoreVisible = value; OnPropertyChanged(); }
        }


        public CollectionLoadMore(IScrollable collection)
        {
            _collection = collection;
        }

        public async Task LoadMore()
        {
            if (!listEnd)
            {
                Task task = Task.Factory.StartNew(async () =>
                {
                    offset += limit;
                    int count = await _collection.ItemsCount(limit, offset);
                    if (count == 0)
                    {
                        loadMoreText = "end of list";
                        listEnd = true;
                        offset -= limit;
                        return;
                    }
                    if(!isLoadMoreVisible && count == limit)
                    {
                        isLoadMoreVisible = true;
                    }

                    await _collection.LoadItems();
                });
                await task;
            }
        }
    }
}
