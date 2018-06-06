using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using TK.CustomMap;
using Xamarin.Forms;

namespace XamarinForms.Maps.MapControls
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TKMapView : ContentView
    {
        public static readonly BindableProperty LocationsProperty = BindableProperty.Create("Locations", typeof(IList<GeoLocation>), typeof(TKMapView), new ObservableCollection<GeoLocation>(), BindingMode.TwoWay, null, OnLocationsChanged);
        public static readonly BindableProperty MapCenterProperty = BindableProperty.Create("MapCenter", typeof(GeoLocation), typeof(TKMapView), null, BindingMode.TwoWay, null, OnMapCenterChanged);
        private bool _settingLocationInternal;
        private bool _settingPinsFromLocations;
        private TKMapViewModel _viewModel;
		private bool _moveToMapCenter;

		public TKMapView()
        {
            InitializeComponent();
            _viewModel = new TKMapViewModel();
            _viewModel.Pins.CollectionChanged += ViewModel_Pins_CollectionChanged;
            MyMap.BindingContext = _viewModel;
			MyMap.MapReady += MyMap_MapReady;
		}

		private void MyMap_MapReady(object sender, System.EventArgs e)
		{
			//BUG: Map does not move to position when loaded
			if (_moveToMapCenter)
			{
				var position = new Position(MapCenter.Latitude, MapCenter.Longitude);
				MyMap.MoveToMapRegion(MapSpan.FromCenterAndRadius(position, Distance.FromMiles(2))); //zoom level is lost when loaded, so re-zoom
			}
		}

		public ObservableCollection<GeoLocation> Locations
        {
            get { return (ObservableCollection<GeoLocation>)GetValue(LocationsProperty); }
            set
            {
                if (Locations != null)
                {
                    Locations.CollectionChanged -= Locations_CollectionChanged;
                }
                SetValue(LocationsProperty, value);
                if (Locations != null)
                {
                    Locations.CollectionChanged += Locations_CollectionChanged;
                }
                UpdatePinsFromLocations();
            }
        }

        public GeoLocation MapCenter
        {
            get { return (GeoLocation)GetValue(MapCenterProperty); }
            set
            {
                SetValue(MapCenterProperty, value);
                _viewModel.MapCenter = new TK.CustomMap.Position(MapCenter.Latitude, MapCenter.Longitude);
            }
        }

        private static void OnLocationsChanged(BindableObject instance, object oldValue, object newValue)
        {
            TKMapView map = (TKMapView)instance;
            map.Locations = (ObservableCollection<GeoLocation>)newValue;
        }

        private static void OnMapCenterChanged(BindableObject instance, object oldValue, object newValue)
        {
			TKMapView map = (TKMapView)instance;
			map.MapCenter = (GeoLocation)newValue;
			var position = new Position(map.MapCenter.Latitude, map.MapCenter.Longitude);
			if (map.MyMap.Height > 0)
			{
				map.InvalidateLayout(); //map will not center if it's still loading the view - call invalidate to force move after setting value
				map.MyMap.MoveToMapRegion(MapSpan.FromCenterAndRadius(position, Distance.FromMiles(2))); //zoom level is lost when loaded, so re-zoom
			}
			else
			{
				map._moveToMapCenter = true;
			}
		}

        private void Locations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_settingLocationInternal)
                return;

            UpdatePinsFromLocations();
        }

		public void MoveToPins()
		{
			if (_viewModel.Pins.Count > 0)
			{
				var positions = new List<Position>();
				foreach (var pin in MyMap.Pins)
				{
					positions.Add(pin.Position);
				}
				if (positions.Count > 1)
				{
					MyMap.FitMapRegionToPositions(positions, true);
				}
				else
				{
					MyMap.MoveToMapRegion(MapSpan.FromCenterAndRadius(positions[0], Distance.FromMiles(2)), true);
				}
			}
		}

		private void UpdatePinsFromLocations()
        {
			_settingPinsFromLocations = true;
            _viewModel.Pins.Clear();
            //var positions = Locations.Select(x => new TK.CustomMap.Position(x.Latitude, x.Longitude));
            foreach (var location in Locations)
            {
                _viewModel.AddPin(location.Latitude, location.Longitude, location.Description);
            }
            //if (Locations.Count > 0)
            //{
            //    MyMap.FitMapRegionToPositions(positions, true);
            //}
            _settingPinsFromLocations = false;
        }

        private void ViewModel_Pins_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_settingPinsFromLocations) //only adjust the locations collection if added from the map
                return;

            _settingLocationInternal = true;
            Locations.Clear();
            foreach (var pin in _viewModel.Pins)
            {
                Locations.Add(new GeoLocation { Latitude = pin.Position.Latitude, Longitude = pin.Position.Longitude });
            }
            _settingLocationInternal = false;
        }
    }
}