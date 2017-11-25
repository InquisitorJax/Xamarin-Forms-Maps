using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinForms.Maps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
        }
    }
}