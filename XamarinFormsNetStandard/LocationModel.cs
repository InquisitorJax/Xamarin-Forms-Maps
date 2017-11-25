using Prism.Mvvm;

namespace XamarinForms.Maps
{
    public class LocationModel : BindableBase
    {
        private string _address;
        private string _location;

        private string _name;

        public string Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value); }
        }

        /// <summary>
        /// Well Known Text Location
        /// </summary>
        public string Location
        {
            get { return _location; }
            set { SetProperty(ref _location, value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public static LocationModel Default()
        {
            var model = new LocationModel();

            model.Name = "Locations Example";
            model.Location = new GeoLocation { Latitude = -33.849, Longitude = 18.652 }.ToWellKnownText();

            return model;
        }
    }
}