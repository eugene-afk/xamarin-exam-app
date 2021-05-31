using ExamApp.Common;
using ExamApp.Data.Model;
using ExamApp.Data.SQlite;
using System.Collections.ObjectModel;

namespace ExamApp.View.Controls
{
    public class QuestionsSortable : ObservableCollection<ExamQuestion>, IOrderable
    {
        public async void ChangeOrdinal(IOrderableItem item, int oldIndex, int newIndex)
        {
            Move(oldIndex, newIndex);
            await SQLiteTools.repo.ChangeQuestionsOrder(item.id, newIndex, oldIndex);
        }
    }
}
