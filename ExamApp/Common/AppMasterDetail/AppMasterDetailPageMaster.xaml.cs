using ExamApp.Data.SQlite;
using ExamApp.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ExamApp.Common.AppMasterDetail
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppMasterDetailPageMaster : ContentPage
    {
        public ListView ListView;

        public AppMasterDetailPageMaster()
        {
            InitializeComponent();

            BindingContext = new AppMasterDetailPageMasterViewModel();
            ListView = MenuItemsListView;
        }

        class AppMasterDetailPageMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<AppMasterDetailPageMasterMenuItem> MenuItems { get; set; }
            public string buildVersion { get; set; }
            public string user { get; set; }
            public AppMasterDetailPageMasterViewModel()
            {
                Task.Run(async () => { user = await SQLiteTools.repo.GetUserNameByID(CommonData.userID); });
                MenuItems = new ObservableCollection<AppMasterDetailPageMasterMenuItem>(new[]
                {
                    new AppMasterDetailPageMasterMenuItem { Id = 0, Title = "Home", TargetType = typeof(AppMasterDetailPage) },
                    new AppMasterDetailPageMasterMenuItem { Id = 1, Title = "Exams", TargetType = typeof(ExamsPage) },
                    new AppMasterDetailPageMasterMenuItem { Id = 2, Title = "Create", TargetType = typeof(ModifyExamPage)},
                    new AppMasterDetailPageMasterMenuItem { Id = 3, Title = "History", TargetType = typeof(HistoyPage) },
                    new AppMasterDetailPageMasterMenuItem { Id = 3, Title = "Demo", TargetType = typeof(DemoPage) }
                });
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                DateTime buildDate = new DateTime(2000, 1, 1)
                                        .AddDays(version.Build).AddSeconds(version.Revision * 2);
                buildVersion = $"Build {version} ({buildDate.ToString("MM/dd/yyyy H:mm")})";
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}