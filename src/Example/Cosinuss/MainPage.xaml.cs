using Cosinuss.Library.Device.Constants;
using Cosinuss.Library.Device.Interfaces;
using Cosinuss.Library.Device.Sensors;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace Cosinuss.Example
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
                    Cosinuss.Library.Cross.Current.OnDeviceFound += Current_OnDeviceFound;
                    Cosinuss.Library.Cross.Current.StartScanning();
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

        private void Current_OnDeviceFound(object sender, ICosinussDevice cosinussDevice)
        {
            // stop scanning after any cosinuss device was found
            Cosinuss.Library.Cross.Current.StopScanning();
            Cosinuss.Library.Cross.Current.OnDeviceFound -= Current_OnDeviceFound;

            // connect to the cosinuss device
            cosinussDevice.OnConnectionStateChanged += CosinussDevice_OnConnectionStateChanged;
            cosinussDevice.Connect();    
        }

        private void CosinussDevice_OnConnectionStateChanged(object sender, ConnectionState e)
        {
            // TODO: ensure sender type safety
            var cosinussDevice = (ICosinussDevice)sender;

            if (e == ConnectionState.CONNECTED)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.ConnectingActivityIndicator.IsRunning = false;
                    this.DeviceIdLabel.Text = cosinussDevice.Id;
                    this.DeviceTypeLabel.Text = cosinussDevice.ManufacturerName + " " + cosinussDevice.ModelNumber;
                    this.SoftwareAndFirmwareLabel.Text = cosinussDevice.SoftwareRevision + " / " + cosinussDevice.FirmwareRevision;

                    cosinussDevice.BatteryLevelChanged += CosinussDevice_BatteryLevelChanged;
                    cosinussDevice.SensorQualityIndexChanged += CosinussDevice_DataQualityIndexChanged;

                    cosinussDevice.BodyTemperatureChanged += CosinussDevice_BodyTemperatureChanged;
                    cosinussDevice.HeartRateChanged += CosinussDevice_HeartRateChanged;
                    cosinussDevice.SPO2Changed += CosinussDevice_SPO2Changed;
                    cosinussDevice.AccelerometerChanged += CosinussDevice_AccelerometerChanged;
                    cosinussDevice.StepFrequencyChanged += CosinussDevice_StepFrequencyChanged;
                });
            }
            else if (e == ConnectionState.DISCONNECTED)
            {
                cosinussDevice.BatteryLevelChanged -= CosinussDevice_BatteryLevelChanged;
                cosinussDevice.SensorQualityIndexChanged -= CosinussDevice_DataQualityIndexChanged;

                cosinussDevice.BodyTemperatureChanged -= CosinussDevice_BodyTemperatureChanged;
                cosinussDevice.HeartRateChanged -= CosinussDevice_HeartRateChanged;
                cosinussDevice.AccelerometerChanged -= CosinussDevice_AccelerometerChanged;
                cosinussDevice.SPO2Changed -= CosinussDevice_SPO2Changed;
            }
        }

        private void CosinussDevice_StepFrequencyChanged(object sender, byte e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                StepFrequencyLabel.Text = e + " spm";
            });
        }

        private void CosinussDevice_SPO2Changed(object sender, float e)
        {
            throw new NotImplementedException();
        }

        private void CosinussDevice_BatteryLevelChanged(object sender, short e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                BatteryPercentageLabel.Text = e + " %";
            });
        }

        private readonly NumberFormatInfo numberFormat = new NumberFormatInfo
        {
            NumberDecimalSeparator = ".",
        };
        private void CosinussDevice_AccelerometerChanged(object sender, Accelerometer e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                this.XProgressBar.Progress = (e.X + 1f) / 2f;
                this.YProgressBar.Progress = (e.Y + 1f) / 2f;
                this.ZProgressBar.Progress = (e.Z + 1f) / 2f;

                this.XValueLabel.Text = ((e.X >= 0) ? "+" : "") + Math.Round(e.X, 4).ToString("0.0000", numberFormat);
                this.YValueLabel.Text = ((e.Y >= 0) ? "+" : "") + Math.Round(e.Y, 4).ToString("0.0000", numberFormat);
                this.ZValueLabel.Text = ((e.Z >= 0) ? "+" : "") + Math.Round(e.Z, 4).ToString("0.0000", numberFormat);
            });
        }

        private void CosinussDevice_BodyTemperatureChanged(object sender, double e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                BodyTemperatureValueLabel.Text = e + " °C";
            });
        }

        private void CosinussDevice_DataQualityIndexChanged(object sender, byte e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                DataQualityIndexValueLabel.Text = e.ToString();
            });
        }

        private void CosinussDevice_HeartRateChanged(object sender, float e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                HeartRateValueLabel.Text = e.ToString();
            });
        }
    }
}
