using cosinuss;
using Cosinuss.Interfaces;
using Plugin.BluetoothLE;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cosinuss
{
    public class CosinussDeviceManager : ICosinussDeviceManager
    {
        public event EventHandler<ICosinussDevice> OnDeviceFound;

        private const string COSINUSS_DEVICE_NAME = "earconnect";

        private bool _isScanning = false;

        public void StartScanningForDevices()
        {
            if (CrossBleAdapter.Current.Status != AdapterStatus.PoweredOn)
            {
                // TODO: throw an exception
            }

            _isScanning = true;

            CrossBleAdapter.Current.Scan().Subscribe(scanResult => {
                if (!_isScanning || scanResult.AdvertisementData.LocalName != COSINUSS_DEVICE_NAME)
                {
                    return; // if the device name does not match the expected name return immediatly
                }

                OnDeviceFound?.Invoke(this, new CosinussDevice(scanResult.Device));            
            });
        }

        public void StopScanningForDevices()
        {
            _isScanning = false;
            CrossBleAdapter.Current.StopScan(); // TODO: somehow this does not seem to work
        }
    }
}
