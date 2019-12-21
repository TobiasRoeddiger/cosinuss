using System;
using System.Collections.Generic;
using System.Text;

namespace Cosinuss.Interfaces
{
    public interface ICosinussDevice
    {
        /**
         * DEVICE ATTRIBUTES
         */
        string Id { get; }
        string ManufacturerName { get; }
        string ModelNumber { get; }
        string HardwareRevision { get; }
        string FirmwareRevision { get; }
        string SoftwareRevision { get; }

        ConnectionState ConnectionState { get; }
        event EventHandler<ConnectionState> OnConnectionStateChanged;

        int BatteryLevel { get; }
        event EventHandler<int> BatteryLevelChanged;

        int DataQualityIndex { get; }
        event EventHandler<int> DataQualityIndexChanged;

        /**
         * SENSOR READINGS
         */
        float BodyTemperature { get; }
        event EventHandler<float> BodyTemperatureChanged;

        float HeartRate { get; }
        event EventHandler<float> HeartRateChanged;

        Accelerometer Accelerometer { get; }
        event EventHandler<Accelerometer> AccelerometerChanged;

        /** 
         * DEVICE CONTROL
         */
        void Connect();

        void Disconnect();
    }
}