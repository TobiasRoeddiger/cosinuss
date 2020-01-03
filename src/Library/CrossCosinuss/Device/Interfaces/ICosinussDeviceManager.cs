using System;
using System.Collections.Generic;
using System.Text;

namespace Cosinuss.Library.Device.Interfaces
{
    public interface ICosinussDeviceManager
    {
        event EventHandler<ICosinussDevice> OnDeviceFound;

        void StartScanning();

        void StopScanning();
    }
}
