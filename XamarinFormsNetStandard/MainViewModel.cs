using Prism.Events;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace XamarinForms.Maps
{
    public class MainViewModel : BindableBase
    {
        private GeoLocation _location;
        private SubscriptionToken _locationSelectionToken;

        private LocationModel _model;

        public MainViewModel()
        {
            Locations = new ObservableCollection<GeoLocation>();

            Model = LocationModel.Default();
            Location = GeoLocation.FromWellKnownText(Model.Location);

            //SelectLocationCommand = new DelegateCommand(SelectLocationAsync);
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

        //private async void SelectLocationAsync()
        //{
        //    //TODO ... Check: this doesn't get unsubscribed when back is pressed
        //    _locationSelectionToken = App.EventMessenger.GetEvent<LocationSelectionMessageEvent>().Subscribe(OnLocationSelection);

        //    await LocationHelper.SelectLocation(Location, Constants.Navigation.ServiceRequest, Model.Address);
        //}

        //private void OnLocationSelection(LocationSelectionResult result)
        //{
        //    if (result.MessageId == Constants.Navigation.ServiceRequest)
        //    {
        //        App.EventMessenger.GetEvent<LocationSelectionMessageEvent>().Unsubscribe(_locationSelectionToken);
        //        if (result.Result == TaskResult.Success)
        //        {
        //            Location = result.Location;
        //        }
        //    }
        //}
    }
}