using ExamApp.Common;
using ExamApp.Common.AppMasterDetail;
using ExamApp.Data.Model;
using ExamApp.Data.SQlite;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ExamApp.ViewModel
{
    public class LoginViewModel : Base
    {
        private string _userName;
        public string userName
        {
            get { return _userName; }
            set { _userName = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { protected set; get; }

        public LoginViewModel()
        {
            SaveCommand = new Command(async () => { await Save(); });
        }

        private async Task Save()
        {
            User user = new User() { name = userName, type = Data.UserType.Parent };
            await SQLiteTools.repo.InsertUser(user);
            CommonData.userID = user.id;
            Config cfg = new Config() { signed = true, userID = user.id };
            await SQLiteTools.repo.InsertConfig(cfg);
            var navPage = new NavigationPage(new AppMasterDetailPage());
            Application.Current.MainPage = navPage;
        }

    }
}
