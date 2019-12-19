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

        public void StartScanningForDevices()
        {
            if (CrossBleAdapter.Current.Status != AdapterStatus.PoweredOn)
            {
                // TODO: throw an exception
            }

            // TODO: scan only for devices with cosinuss services

            CrossBleAdapter.Current.Scan().Subscribe(scanResult => {
                if (scanResult.AdvertisementData.LocalName != "earconnect")
                {
                    return; // if the device name does not match the expected device name of cosinuss devices return immediatly
                }

                OnDeviceFound?.Invoke(this, null);
            });
        }

        public void StopScanningForDevices()
        {
            CrossBleAdapter.Current.StopScan();
        }
    }
}
