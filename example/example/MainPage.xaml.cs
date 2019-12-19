using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace example
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private const string ON_RATIONALE_TITLE = "Location Permission Required";
        private const string ON_RATIONALE_TEXT = "Android requires location access in order to connect to Bluetooth Low Energy devices.";
        private const string ON_RATIONALE_CONFIRM = "OK";

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            RequestPermissions();
        }

        private async void RequestPermissions()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.LocationAlways);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.LocationAlways))
                    {
                        await DisplayAlert(ON_RATIONALE_TITLE, ON_RATIONALE_TEXT, ON_RATIONALE_CONFIRM);
                    }

                    status = (await CrossPermissions.Current.RequestPermissionsAsync(Permission.LocationAlways))[Permission.LocationAlways];
                }

                if (status == PermissionStatus.Granted)
                {
                    Cosinuss.CrossCosinuss.Current.OnDeviceFound += Current_OnDeviceFound;

                    Device.InvokeOnMainThreadAsync(() =>
                    {
                        Cosinuss.CrossCosinuss.Current.StartScanningForDevices();
                    }).ConfigureAwait(false);
                }
                else if (status != PermissionStatus.Unknown)
                {
                    //location denied
                }

            } 
            catch (Exception e)
            {
                // something went wrong
            }
        }

        private void Current_OnDeviceFound(object sender, Cosinuss.Interfaces.ICosinussDevice e)
        {
            Cosinuss.CrossCosinuss.Current.StopScanningForDevices();
        }
    }
}
