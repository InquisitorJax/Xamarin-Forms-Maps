using Plugin.Geolocator.Abstractions;
using System;
using System.Threading.Tasks;
using Wibci.LogicCommand;

namespace XamarinForms.Maps.Commands
{
    public interface IFetchCurrentLocationCommand : IAsyncLogicCommand<object, FetchCurrentLocationResult>
    {
    }

    public class FetchCurrentLocationCommand : AsyncLogicCommand<object, FetchCurrentLocationResult>, IFetchCurrentLocationCommand
    {
        private readonly IGeolocator _locationService;

        public FetchCurrentLocationCommand(IGeolocator locationService)
        {
            _locationService = locationService;
        }

        public override async Task<FetchCurrentLocationResult> ExecuteAsync(object request)
        {
            var retResult = new FetchCurrentLocationResult();

            if (!_locationService.IsGeolocationAvailable)
            {
                //TODO BM: Request location to be enabled?
                retResult.Notification.Add(new NotificationItem("Geolocation is not available on this device"));
            }

            if (!_locationService.IsGeolocationEnabled)
            {
                //TODO BM: Request location to be enabled?
                retResult.Notification.Add(new NotificationItem("Geolocation has not been enabled"));
            }

            if (retResult.IsValid())
            {
                try
                {
                    GeoLocation location = new GeoLocation();

                    //TODO: make part of request
                    _locationService.DesiredAccuracy = 25;
                    var timeout = TimeSpan.FromSeconds(10);

                    Plugin.Geolocator.Abstractions.Position position = await _locationService.GetPositionAsync(timeout);

                    if (position != null)
                    {
                        location = new GeoLocation
                        {
                            Latitude = position.Latitude,
                            Longitude = position.Longitude,
                            TimeStamp = position.Timestamp,
                            Description = "Current Location"
                        };
                    }

                    retResult.CurrentLocation = location;
                }
                catch
                {
                    retResult.Notification.Add("Unable to get location :(");
                }
            }

            return retResult;
        }
    }

    public class FetchCurrentLocationResult : CommandResult
    {
        public GeoLocation CurrentLocation { get; set; }
    }
}