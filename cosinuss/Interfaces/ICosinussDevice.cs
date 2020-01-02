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

        short BatteryLevel { get; }
        event EventHandler<short> BatteryLevelChanged;

        byte DataQualityIndex { get; }
        event EventHandler<byte> DataQualityIndexChanged;

        /**
         * SENSOR READINGS
         */
        double BodyTemperature { get; }
        event EventHandler<double> BodyTemperatureChanged;

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