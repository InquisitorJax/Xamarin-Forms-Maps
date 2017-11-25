using Prism.Events;
using Xamarin.Forms;
using XamarinForms.Maps;

namespace XamarinForms
{
    public class App : Application
    {
        public App()
        {
            EventMessenger = new EventAggregator();
            // The root page of your application
            MainPage = new NavigationPage(new MainPage());
        }

        public static IEventAggregator EventMessenger { get; private set; }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }
    }
}