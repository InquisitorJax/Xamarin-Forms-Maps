using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinForms.Maps.Commands;
using XamarinForms.Maps.MapServices;

namespace XamarinForms.Maps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocationSelectionPage : ContentPage
    {
        private LocationSelectionViewModel _viewModel;

        public LocationSelectionPage()
        {
            InitializeComponent();
            var geoCodeService = new GeocodeService();
            var fetchCurrentLocationCommand = new FetchCurrentLocationCommand(Plugin.Geolocator.CrossGeolocator.Current);
            _viewModel = new LocationSelectionViewModel(geoCodeService, fetchCurrentLocationCommand);
            BindingContext = _viewModel;
        }

        public LocationSelectionViewModel ViewModel
        {
            get { return _viewModel; }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _viewModel.Closing();
        }
    }
}