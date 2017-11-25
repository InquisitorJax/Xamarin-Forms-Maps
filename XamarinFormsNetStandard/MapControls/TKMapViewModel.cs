using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TK.CustomMap;
using Xamarin.Forms;
using Position = TK.CustomMap.Position;

namespace XamarinForms.Maps.MapControls
{
    public class TKMapViewModel : BindableBase
    {
        private Position _mapCenter;

        private ObservableCollection<TKCustomMapPin> _pins;

        private TKCustomMapPin _selectedPin;

        public TKMapViewModel()
        {
            MapClickedCommand = new Command<Position>(OnMapClicked);
            PinSelectedCommand = new Command(OnPinSelected);
        }

        public Position MapCenter
        {
            get { return this._mapCenter; }
            set { SetProperty(ref _mapCenter, value); }
        }

        public ICommand MapClickedCommand { get; private set; }

        public ObservableCollection<TKCustomMapPin> Pins
        {
            get
            {
                if (_pins == null)
                    _pins = new ObservableCollection<TKCustomMapPin>();
                return _pins;
            }
            set { SetProperty(ref _pins, value); }
        }

        public ICommand PinSelectedCommand { get; private set; }

        public TKCustomMapPin SelectedPin
        {
            get { return this._selectedPin; }
            set { SetProperty(ref _selectedPin, value); }
        }

        public void AddPin(double latitude, double longitude, string title)
        {
            Position position = new Position(latitude, longitude);
            AddPin(position, title);
        }

        public void AddPin(Position position, string title)
        {
            TKCustomMapPin pin = new TKCustomMapPin
            {
                Position = position,
                IsDraggable = true,
                ShowCallout = true,
                Title = title,
            };

            Pins.Add(pin);
        }

        private void OnMapClicked(Position position)
        {
            string title = "my location";
            var existingPin = Pins.FirstOrDefault();
            if (existingPin != null)
            {
                title = existingPin.Title;
            }
            Pins.Clear(); //TODO: Add PinSelectionMode.Single / Multiple
            AddPin(position, title);
        }

        private void OnPinSelected()
        {
            MapCenter = SelectedPin.Position;
        }
    }
}