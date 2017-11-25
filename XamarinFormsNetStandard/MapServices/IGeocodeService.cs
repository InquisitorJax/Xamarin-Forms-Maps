using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace XamarinForms.Maps.MapServices
{
    public interface IGeocodeService
    {
        Task<GeocodeAddressResponse> FindAddressAsync(GeocodeAddressRequest request);

        Task<GeocodeLocationResponse> FindLocationAsync(GeocodeLocationRequest request);
    }

    public class GeocodeAddressRequest
    {
        public GeocodeAddressRequest(GeoLocation location)
        {
            Location = location;
        }

        public GeoLocation Location { get; private set; }
    }

    public class GeocodeAddressResponse
    {
        public GeocodeAddressResponse(IList<string> address)
        {
            Address = address;
        }

        public IList<string> Address { get; private set; }
    }

    public class GeocodeLocationRequest
    {
        public GeocodeLocationRequest(string address)
        {
            Address = address;
        }

        public string Address { get; private set; }
    }

    public class GeocodeLocationResponse
    {
        public GeocodeLocationResponse(IList<GeoLocation> locations)
        {
            Locations = locations;
        }

        public IList<GeoLocation> Locations { get; private set; }
    }

    public class GeocodeService : IGeocodeService
    {
        private Geocoder _geoCoder;

        public GeocodeService()
        {
            _geoCoder = new Geocoder();
        }

        public async Task<GeocodeAddressResponse> FindAddressAsync(GeocodeAddressRequest request)
        {
            var position = new Position(request.Location.Latitude, request.Location.Longitude);
            var result = await _geoCoder.GetAddressesForPositionAsync(position).ConfigureAwait(false);

            return new GeocodeAddressResponse(result.ToList());
        }

        public async Task<GeocodeLocationResponse> FindLocationAsync(GeocodeLocationRequest request)
        {
            var result = await _geoCoder.GetPositionsForAddressAsync(request.Address);

            return new GeocodeLocationResponse(result.Select(pos => new GeoLocation { Latitude = pos.Latitude, Longitude = pos.Longitude, Description = request.Address }).ToList());
        }
    }
}