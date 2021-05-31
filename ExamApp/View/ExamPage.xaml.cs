using ExamApp.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ExamApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExamPage : ContentPage
    {
        public ExamPage(int id, string name)
        {
            InitializeComponent();
            BindingContext = new ExamViewModel(id, name);
        }
    }
}