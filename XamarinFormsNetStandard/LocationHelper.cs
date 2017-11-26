using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XamarinForms.Maps
{
    public static class LocationHelper
    {
        public static async Task SelectLocation(GeoLocation currentLocation, string sourceId, string locationSearch = "")
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string location = JsonConvert.SerializeObject(currentLocation);
            parameters.Add(Constants.Parameters.Location, location);
            parameters.Add(Constants.Parameters.MessageId, sourceId);

            if (!string.IsNullOrWhiteSpace(locationSearch))
            {
                parameters.Add(Constants.Parameters.LocationSearch, locationSearch);
            }

            var page = new LocationSelectionPage();
            await App.Current.MainPage.Navigation.PushModalAsync(page);
            await page.ViewModel.InitializeAsync(parameters);
        }
    }
}