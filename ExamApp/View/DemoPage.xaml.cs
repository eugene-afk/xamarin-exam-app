using ExamApp.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ExamApp.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DemoPage : ContentPage
    {
        public DemoPage()
        {
            InitializeComponent();
            BindingContext = new DemoViewModel();
        }
    }
}