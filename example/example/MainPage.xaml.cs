using Cosinuss.Interfaces;
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
                    Cosinuss.CrossCosinuss.Current.StartScanningForDevices();
                }
                else if (status != PermissionStatus.Unknown)
                {
                    //location denied
                }

            } 
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Current_OnDeviceFound(object sender, Cosinuss.Interfaces.ICosinussDevice e)
        {
            // stop scanning after any cosinuss device was found
            Cosinuss.CrossCosinuss.Current.OnDeviceFound -= Current_OnDeviceFound;
            Cosinuss.CrossCosinuss.Current.StopScanningForDevices();

            // connect to the cosinuss device
            e.OnConnectionStateChanged += E_OnConnectionStateChanged;
            e.Connect();
            
        }

        private void E_OnConnectionStateChanged(object sender, Cosinuss.ConnectionState e)
        {
            // TODO: ensure sender type safety
            var cosinussDevice = (ICosinussDevice)sender;

            if (e == Cosinuss.ConnectionState.CONNECTED)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.DeviceIdLabel.Text = cosinussDevice.Id;
                    this.DeviceTypeLabel.Text = cosinussDevice.ManufacturerName + " " + cosinussDevice.ModelNumber;
                    this.SoftwareAndFirmwareLabel.Text = cosinussDevice.SoftwareRevision + " (" + cosinussDevice.FirmwareRevision + ")";
                });

                cosinussDevice.BatteryLevelChanged += E_BatteryLevelChanged;
                cosinussDevice.DataQualityIndexChanged += E_DataQualityIndexChanged;
                
                cosinussDevice.BodyTemperatureChanged += E_BodyTemperatureChanged;
                cosinussDevice.HeartRateChanged += E_HeartRateChanged;
                cosinussDevice.AccelerometerChanged += E_AccelerometerChanged;
            }
            else if (e == Cosinuss.ConnectionState.DISCONNECTED)
            {
                cosinussDevice.BatteryLevelChanged -= E_BatteryLevelChanged;
                cosinussDevice.DataQualityIndexChanged -= E_DataQualityIndexChanged;

                cosinussDevice.BodyTemperatureChanged -= E_BodyTemperatureChanged;
                cosinussDevice.HeartRateChanged -= E_HeartRateChanged;
                cosinussDevice.AccelerometerChanged -= E_AccelerometerChanged;
            }
        }

        private void E_BatteryLevelChanged(object sender, int e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                BatteryPercentageLabel.Text = e + "%";
            });
        }

        private void E_AccelerometerChanged(object sender, Cosinuss.Accelerometer e)
        {
            throw new NotImplementedException();
        }

        private void E_BodyTemperatureChanged(object sender, float e)
        {
            throw new NotImplementedException();
        }

        private void E_DataQualityIndexChanged(object sender, int e)
        {
            throw new NotImplementedException();
        }

        private void E_HeartRateChanged(object sender, float e)
        {
            throw new NotImplementedException();
        }
    }
}
