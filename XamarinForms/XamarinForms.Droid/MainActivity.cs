using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Plugin.Permissions;
using TK.CustomMap.Droid;
using Xamarin.Forms.Platform.Android;
using Permission = Android.Content.PM.Permission;

namespace XamarinForms.Droid
{
    [Activity(Label = "Xamarin Forms Maps",
              Icon = "@drawable/icon",
              Theme = "@style/MyTheme",
              MainLauncher = true,
              LaunchMode = LaunchMode.SingleInstance,
              ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
    {
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            //required part of the media plugin (android M)
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());

            TKGoogleMaps.Init(this, bundle);
        }
    }
}