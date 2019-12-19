using System;
using System.Collections.Generic;
using System.Text;

namespace Cosinuss.Interfaces
{
    interface ICosinussDeviceManager
    {
        event EventHandler<ICosinussDevice> OnDeviceFound;

        void StartScanningForDevices();

        void StopScanningForDevices();
    }
}
