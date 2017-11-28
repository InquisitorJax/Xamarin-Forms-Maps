using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Wibci.LogicCommand;
using XamarinForms.Maps.Commands;
using XamarinForms.Maps.MapServices;

namespace XamarinForms.Maps
{
    public class LocationSelectionViewModel : BindableBase
    {
        private string _busyMessage;
        private bool _isBusy;
        private GeoLocation _mapCenter;
        private string _searchLocationText;
        private bool _selectionMade;
        private string _sourceId;

        public LocationSelectionViewModel(IGeocodeService locationService, IFetchCurrentLocationCommand fetchCurrentLocationCommand)
        {
            LocationService = locationService;
            SearchLocationCommand = new DelegateCommand(SearchLocation);
            SelectLocationCommand = new DelegateCommand(SelectLocationAsync);
            Locations = new ObservableCollection<GeoLocation>();
            RequestCurrentLocationCommand = new DelegateCommand(RequestCurrentLocation);
            FetchCurrentLocationCommand = fetchCurrentLocationCommand;
        }

        public string BusyMessage
        {
            get { return _busyMessage; }
            set { SetProperty(ref _busyMessage, value); }
        }

        public IFetchCurrentLocationCommand FetchCurrentLocationCommand { get; private set; }

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        public ObservableCollection<GeoLocation> Locations { get; private set; }

        public IGeocodeService LocationService { get; private set; }

        public GeoLocation MapCenter
        {
            get { return _mapCenter; }
            set { SetProperty(ref _mapCenter, value); }
        }

        public ICommand RequestCurrentLocationCommand { get; private set; }
        public ICommand SearchLocationCommand { get; private set; }

        public string SearchLocationText
        {
            get { return _searchLocationText; }
            set { SetProperty(ref _searchLocationText, value); }
        }

        public ICommand SelectLocationCommand { get; private set; }

        public void Closing()
        {
            if (!_selectionMade)
            {
                //NOTE: Always fire location selection event, so subscribers can unsubscribe
                var result = new LocationSelectionResult(TaskResult.Canceled) { MessageId = _sourceId };
                App.EventMessenger.GetEvent<LocationSelectionMessageEvent>().Publish(result);
            }
        }

        public async Task InitializeAsync(Dictionary<string, string> args)
        {
            if (args != null && args.ContainsKey(Constants.Parameters.Location))
            {
                string setLocation = args[Constants.Parameters.Location];
                GeoLocation location = JsonConvert.DeserializeObject<GeoLocation>(setLocation);
                if (location != null)
                {
                    MapCenter = location;
                    Locations.Add(location);
                }
                else
                {
                    await RequestCurrentLocationAsync();
                }
            }
            else
            {
                if (args != null && args.ContainsKey(Constants.Parameters.LocationSearch))
                {
                    SearchLocationText = args[Constants.Parameters.LocationSearch];
                    await SearchLocationAsync();
                    if (Locations.Count == 0)
                    {
                        await RequestCurrentLocationAsync();
                    }
                }
                else
                {
                    await RequestCurrentLocationAsync();
                }
            }

            if (args != null && args.ContainsKey(Constants.Parameters.MessageId))
            {
                _sourceId = args[Constants.Parameters.MessageId];
            }
        }

        private void NotBusy()
        {
            IsBusy = false;
            BusyMessage = null;
        }

        private async void RequestCurrentLocation()
        {
            await RequestCurrentLocationAsync();
        }

        private async Task RequestCurrentLocationAsync()
        {
            ShowBusy("Searching...");

            try
            {
                var result = await FetchCurrentLocationCommand.ExecuteAsync(null);

                Locations.Clear();
                if (result.IsValid())
                {
                    SearchLocationText = string.Empty;
                    MapCenter = result.CurrentLocation;
                    //Locations.Add(result.CurrentLocation);
                }
                else
                {
                    App.ShowMessage(result.Notification.ToString(), "Error");
                }
            }
            finally
            {
                NotBusy();
            }
        }

        private async void SearchLocation()
        {
            await SearchLocationAsync();
        }

        private async Task SearchLocationAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchLocationText))
                return;

            ShowBusy("Searching...");

            try
            {
                var address = SearchLocationText;
                var result = await LocationService.FindLocationAsync(new GeocodeLocationRequest(SearchLocationText));

                NotBusy();

                if (result.Locations != null && result.Locations.Count > 0)
                {
                    Locations.Clear();
                    MapCenter = result.Locations[0];
                    Locations.Add(MapCenter);
                }
                else
                {
                    App.ShowMessage("Could not find location", "search");
                }
            }
            finally
            {
                NotBusy();
            }
        }

        private async void SelectLocationAsync()
        {
            LocationSelectionResult result = new LocationSelectionResult(TaskResult.Success)
            {
                Location = Locations != null ? Locations.FirstOrDefault() : null,
                MessageId = _sourceId
            };

            if (result.Location == null)
            {
                App.ShowMessage("Please select a location", "make selection");
            }
            else
            {
                _selectionMade = true;
                App.EventMessenger.GetEvent<LocationSelectionMessageEvent>().Publish(result);

                await App.Current.MainPage.Navigation.PopAsync();
            }
        }

        private void ShowBusy(string message)
        {
            IsBusy = true;
            BusyMessage = message;
        }
    }
}