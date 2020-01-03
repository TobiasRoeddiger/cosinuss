using cosinuss;
using Cosinuss.Library.Device.Interfaces;
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

        public void StartScanning()
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

                _isScanning = false;
                CrossBleAdapter.Current.StopScan();

                OnDeviceFound?.Invoke(this, new CosinussDevice(scanResult.Device));            
            });
        }

        public void StopScanning()
        {
            if (!_isScanning) return;

            CrossBleAdapter.Current.StopScan();
            while (CrossBleAdapter.Current.IsScanning) { }
            _isScanning = false;
        }
    }
}
