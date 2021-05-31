using System.Threading.Tasks;

namespace ExamApp.Common
{
    public interface IScrollable
    {
        Task LoadItems();
        Task<int> ItemsCount(int limit, int offset);
    }
}
