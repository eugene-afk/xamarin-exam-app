using ExamApp.Common;
using ExamApp.Data.Model;
using ExamApp.Data.SQlite;
using System.Collections.ObjectModel;

namespace ExamApp.View.Controls
{
    public class VariantsSortable : ObservableCollection<ExamQuestionVariant>, IOrderable
    {
        public async void ChangeOrdinal(IOrderableItem item, int oldIndex, int newIndex)
        {
            Move(oldIndex, newIndex);
            await SQLiteTools.repo.ChangeVariantsOrder(item.id, newIndex, oldIndex);
        }
    }
}
