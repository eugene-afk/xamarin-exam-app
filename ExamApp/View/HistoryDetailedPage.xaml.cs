using ExamApp.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ExamApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryDetailedPage : ContentPage
    {
        public HistoryDetailedPage(int id, string name, int examID)
        {
            InitializeComponent();
            BindingContext = new HistoryDetailedViewModel(id, name, examID);
            resultsList.ItemTapped += (object sender, ItemTappedEventArgs e) => {
                if (e.Item == null) return;

                if (sender is ListView lv) lv.SelectedItem = null;
            };
        }
    }
}