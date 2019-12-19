using Cosinuss;
using Cosinuss.Interfaces;
using Plugin.BluetoothLE;
using System;
using System.Collections.Generic;
using System.Text;

namespace cosinuss
{
    public class CosinussDevice : ICosinussDevice
    {
        /**
         * DEVICE ATTRIBUTES
         */
        public string Id { get; private set; }
        public string ModelNumber { get; private set; }
        public string FirmwareRevision { get; private set; }
        public string HardwareRevision { get; private set; }

        public ConnectionState ConnectionState { get; private set; }
        public event EventHandler<ConnectionState> OnConnectionStateChanged;

        private int _batteryLevel;
        public int BatteryLevel { 
            get
            {
                return _batteryLevel;
            }
            private set
            {
                if (_batteryLevel != value)
                {
                    _batteryLevel = value;
                    BatteryLevelChanged?.Invoke(this, _batteryLevel);
                }
            }
        }
        public event EventHandler<int> BatteryLevelChanged;

        private int _dataQualityIndex;
        public int DataQualityIndex
        {
            get
            {
                return _batteryLevel;
            }
            private set
            {
                if (_dataQualityIndex != value)
                {
                    _dataQualityIndex = value;
                    DataQualityIndexChanged?.Invoke(this, _dataQualityIndex);
                }
            }
        }
        public event EventHandler<int> DataQualityIndexChanged;

        private float _bodyTemperature;
        public float BodyTemperature
        {
            get
            {
                return _bodyTemperature;
            }
            private set
            {
                if (_bodyTemperature != value)
                {
                    _bodyTemperature = value;
                    BodyTemperatureChanged?.Invoke(this, _bodyTemperature);
                }
            }
        }
        public event EventHandler<float> BodyTemperatureChanged;

        public float HeartRate => throw new NotImplementedException();
        public event EventHandler<float> HeartRateChanged;

        public Accelerometer Accelerometer => throw new NotImplementedException();
        public event EventHandler<Accelerometer> AccelerometerChanged;

        /*
         * INTERNAL BLUETOOTH COMMUNICATION
         */
        private readonly IDevice _internalDevice;

        public CosinussDevice(IDevice internalDevice)
        {
            this._internalDevice = internalDevice;
        }

        public void Connect()
        {
            _internalDevice.Connect();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }
    }
}
