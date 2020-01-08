using Cosinuss.Library.Device.Constants;
using Cosinuss.Library.Device.Sensors;
using System;

namespace Cosinuss.Library.Device.Interfaces
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

        byte SensorQualityIndex { get; }
        event EventHandler<byte> SensorQualityIndexChanged;

        /**
         * SENSOR READINGS
         */
        double BodyTemperature { get; }
        event EventHandler<double> BodyTemperatureChanged;

        float HeartRate { get; }
        event EventHandler<float> HeartRateChanged;

        float SPO2 { get; }
        event EventHandler<float> SPO2Changed;

        Accelerometer Accelerometer { get; }
        event EventHandler<Accelerometer> AccelerometerChanged;

        byte StepFrequency { get; }
        event EventHandler<byte> StepFrequencyChanged;

        /** 
         * DEVICE CONTROL
         */
        void Connect();
    }
}