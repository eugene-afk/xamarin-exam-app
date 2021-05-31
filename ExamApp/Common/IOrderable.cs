namespace ExamApp.Common
{
    public interface IOrderable
    {
        void ChangeOrdinal(IOrderableItem item, int oldIndex, int newIndex);
    }
}
