using ExamApp.Data.Model;
using ExamApp.Data.SQlite;
using ExamApp.View;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ExamApp.ViewModel
{
    public class ExamsViewModel : Base
    {
        public ICommand AddNewExamCommand { protected set; get; }
        public ICommand LoadMoreCommand { protected set; get; }

        public ExamsCollection exams { get; set; }
        private ExamsPage _page;

        public ExamsViewModel(ExamsPage page)
        {
            _page = page;
            exams = new ExamsCollection();
            AddNewExamCommand = new Command(OpenCreateExamPage);
            LoadMoreCommand = new Command(async () => await exams.loadMore.LoadMore());
        }

        public void OpenCreateExamPage()
        {
            Application.Current.MainPage.Navigation.PushAsync(new ModifyExamPage(exams));
        }

        public async Task ActionSheet(Exam exam)
        {
            string action = await _page.DisplayActionSheet("Exam Menu", "Cancel", null, "Edit", "Delete");
            switch (action)
            {
                case "Edit":
                    await App.Current.MainPage.Navigation.PushAsync(new ModifyExamPage(exam, exams));
                    break;
                case "Delete":
                    await DeleteAction(exam);
                    break;
                default:
                    break;
            }
        }

        internal async Task DeleteAction(Exam exam)
        {
            bool answer = await _page.DisplayAlert("Deleting exam", "You really want to permanently delete exam?", "Yes", "No");
            if (answer)
            {
                int res = await SQLiteTools.repo.DeleteExam(exam);
                if (res != 0)
                {
                    exams.Remove(exam);
                    await _page.DisplayAlert("Deleting exam", "Successfully deleted.", "OK");
                    return;
                }
                await _page.DisplayAlert("Deleting exam", "Failed.", "OK");
            }
        }

    }
}
