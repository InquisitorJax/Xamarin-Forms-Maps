using Prism.Events;
using Wibci.LogicCommand;

namespace XamarinForms.Maps
{
    public class LocationSelectionMessageEvent : PubSubEvent<LocationSelectionResult>
    {
        public static void Publish(TaskResult result, GeoLocation location)
        {
            var messenger = App.EventMessenger;
            var selectResult = new LocationSelectionResult(result)
            {
                Location = location
            };
            messenger.GetEvent<LocationSelectionMessageEvent>().Publish(selectResult);
        }
    }

    public class LocationSelectionResult
    {
        public LocationSelectionResult(TaskResult result)
        {
            Result = result;
        }

        public GeoLocation Location { get; set; }
        public string MessageId { get; set; }

        public TaskResult Result { get; private set; }
    }
}