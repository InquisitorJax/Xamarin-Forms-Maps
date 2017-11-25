using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Wibci.LogicCommand;

namespace XamarinForms.Maps
{
    public class MainViewModel : BindableBase
    {
        private readonly SubscriptionToken _locationSelectionToken;
        private GeoLocation _location;
        private LocationModel _model;

        public MainViewModel()
        {
            Locations = new ObservableCollection<GeoLocation>();

            Model = LocationModel.Default();
            Location = GeoLocation.FromWellKnownText(Model.Location);

            _locationSelectionToken = App.EventMessenger.GetEvent<LocationSelectionMessageEvent>().Subscribe(OnLocationSelection);
            SelectLocationCommand = new DelegateCommand(SelectLocationAsync);
        }

        public GeoLocation Location
        {
            get { return _location; }
            set
            {
                if (value != null)
                    value.Description = Model.Name;
                SetProperty(ref _location, value);
                Model.Location = _location?.ToWellKnownText();
                Locations.Clear();
                if (value != null)
                {
                    Locations.Add(value);
                }
            }
        }

        public ObservableCollection<GeoLocation> Locations { get; private set; }

        public LocationModel Model
        {
            get { return _model; }
            set { SetProperty(ref _model, value); }
        }

        public ICommand SelectLocationCommand { get; private set; }

        private void OnLocationSelection(LocationSelectionResult result)
        {
            if (result.MessageId == Constants.Navigation.MainPage)
            {
                if (result.Result == TaskResult.Success)
                {
                    Location = result.Location;
                }
            }
        }

        private async void SelectLocationAsync()
        {
            await LocationHelper.SelectLocation(Location, Constants.Navigation.MainPage, Model.Address);
        }
    }
}