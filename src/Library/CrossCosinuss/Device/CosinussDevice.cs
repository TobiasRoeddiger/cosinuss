
using Cosinuss.Library.Device.Constants;
using Cosinuss.Library.Device.Interfaces;
using Cosinuss.Library.Device.Sensors;
using Cosinuss.Library.Device.Sensors.Conversion;
using Cross.Device.Sensors.Conversion;
using Plugin.BluetoothLE;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace cosinuss
{
    public class CosinussDevice : ICosinussDevice
    {
        private string _id;
        public string Id {
            get
            {
                return _id;
            }
            private set
            {
                _id = value;
                CheckConnected();
            }
        }

        private string _manufacturerName;
        public string ManufacturerName
        {
            get
            {
                return _manufacturerName;
            }
            private set
            {
                _manufacturerName = value;
                CheckConnected();
            }
        }

        private string _modelNumber;
        public string ModelNumber {
            get
            {
                return _modelNumber;
            }
            private set
            {
                _modelNumber = value;
                CheckConnected();
            }
        }

        private string _hardwareRevision;
        public string HardwareRevision
        {
            get
            {
                return _hardwareRevision;
            }
            private set
            {
                _hardwareRevision = value;
                CheckConnected();
            }
        }

        private string _firmwareRevision;
        public string FirmwareRevision {
            get
            {
                return _firmwareRevision;
            }
            private set
            {
                _firmwareRevision = value;
                CheckConnected();
            }
        }

        private string _softwareRevision;
        public string SoftwareRevision
        {
            get
            {
                return _softwareRevision;
            }
            private set
            {
                _softwareRevision = value;
                CheckConnected();
            }
        }

        private ConnectionState _connectionState = ConnectionState.DISCONNECTED;
        public ConnectionState ConnectionState
        {
            get
            {
                return _connectionState;
            }
            private set
            {
                if (_connectionState != value)
                {
                    _connectionState = value;
                    OnConnectionStateChanged?.Invoke(this, _connectionState);
                }
            }
        }
        public event EventHandler<ConnectionState> OnConnectionStateChanged;

        private short _batteryLevel;
        public short BatteryLevel {
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
        public event EventHandler<short> BatteryLevelChanged;

        private byte _sensorQualityIndex;
        public byte SensorQualityIndex
        {
            get
            {
                return _sensorQualityIndex;
            }
            private set
            {
                if (_sensorQualityIndex != value)
                {
                    _sensorQualityIndex = value;
                    SensorQualityIndexChanged?.Invoke(this, _sensorQualityIndex);
                }
            }
        }
        public event EventHandler<byte> SensorQualityIndexChanged;

        private double _bodyTemperature;
        public double BodyTemperature
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
        public event EventHandler<double> BodyTemperatureChanged;

        private float _heartRate = 0.0f;
        public float HeartRate
        {
            get
            {
                return _heartRate;
            }
            private set
            {
                if (_heartRate != value)
                {
                    _heartRate = value;
                    HeartRateChanged?.Invoke(this, _heartRate);
                }
            }
        }
        public event EventHandler<float> HeartRateChanged;

        private float _spo2 = 0.0f;
        public float SPO2
        {
            get
            {
                return _spo2;
            }
            private set
            {
                if (_spo2 != value)
                {
                    _spo2 = value;
                    SPO2Changed?.Invoke(this, _spo2);
                }
            }
        }
        public event EventHandler<float> SPO2Changed;

        private Accelerometer _accelerometer = new Accelerometer();
        public Accelerometer Accelerometer
        {
            get
            {
                return this._accelerometer;
            }
            set
            {
                if (_accelerometer != value)
                {
                    this._accelerometer = value;
                    AccelerometerChanged?.Invoke(this, _accelerometer);
                }
            }
        }
        public event EventHandler<Accelerometer> AccelerometerChanged;

        private byte _stepFrequency;
        public byte StepFrequency
        {
            get
            {
                return this._stepFrequency;
            }
            set
            {
                if (_stepFrequency != value)
                {
                    this._stepFrequency = value;
                    StepFrequencyChanged?.Invoke(this, _stepFrequency);
                }
            }
        }
        public event EventHandler<byte> StepFrequencyChanged;

        /*
         * CONVERSION STRATEGIES
         */
        private readonly StringConversionStrategy _stringConversion = new StringConversionStrategy();
        private readonly HexStringConversionStrategy _hexStringConversion = new HexStringConversionStrategy();
        private readonly TemperatureConversionStrategy _temperatureConversion = new TemperatureConversionStrategy();
        private readonly HeartRateConversionStrategy _heartRateConversion = new HeartRateConversionStrategy();
        private readonly ByteConversionStrategy _byteConversion = new ByteConversionStrategy();
        private readonly RawDataConversionStrategy _rawConversion = new RawDataConversionStrategy();

        /*
         * INTERNAL BLUETOOTH COMMUNICATION
         */
        private readonly IDevice _internalDevice;
        private bool _bluetoothObserversConfigured = false;
        private readonly byte[] RAW_DATA_KEY = new byte[] { 0x32, 0x31, 0x39, 0x32, 0x37, 0x34, 0x31, 0x30, 0x35, 0x39, 0x35, 0x35, 0x30, 0x32, 0x34, 0x35 };

        public CosinussDevice(IDevice internalDevice)
        {
            this._internalDevice = internalDevice;
        }


        public void Connect()
        {
            if (this.ConnectionState == ConnectionState.CONNECTED)
            {
                return; // already connected, nothing to do
            } 
            else if (this.ConnectionState != ConnectionState.CONNECTING)
            {
                this.ConnectionState = ConnectionState.CONNECTING;
            }

            if (!_bluetoothObserversConfigured)
            {
                ConfigureBluetoothObservers();
            }

            _internalDevice.Connect();
        }

        private void ConfigureBluetoothObservers()
        {
            _internalDevice.WhenStatusChanged().Subscribe(connectionStatus =>
            {
                if (connectionStatus == ConnectionStatus.Disconnected)
                    ConnectionState = ConnectionState.DISCONNECTED;
                else if (connectionStatus == ConnectionStatus.Disconnecting)
                    ConnectionState = ConnectionState.DISCONNECTING;
                else if (connectionStatus == ConnectionStatus.Connecting)
                    ConnectionState = ConnectionState.CONNECTING;
                else if (connectionStatus == ConnectionStatus.Connected)
                {
                    _internalDevice.DiscoverServices().Subscribe(gattService =>
                    {
                        gattService.DiscoverCharacteristics().Subscribe(characteristic => RegisterCharacteristicObservers(characteristic));
                    });
                }
                
            });
            _bluetoothObserversConfigured = true;
        }

        private void RegisterCharacteristicObservers(IGattCharacteristic characteristic)
        {
            /*
             * Read Once Characteristics
             */
            if (characteristic.Uuid == GattConstants.SYSTEM_ID_UUID)
                characteristic.Read().Subscribe(result => { Id = _hexStringConversion.Convert(result.Data); });
            else if (characteristic.Uuid == GattConstants.MANUFACTURER_NAME_UUID)
                characteristic.Read().Subscribe(result => { ManufacturerName = _stringConversion.Convert(result.Data); });
            else if (characteristic.Uuid == GattConstants.MODEL_NUMBER_UUID)
                characteristic.Read().Subscribe(result => { ModelNumber = _stringConversion.Convert(result.Data); });
            else if (characteristic.Uuid == GattConstants.HARDWARE_REVISION_UUID)
                characteristic.Read().Subscribe(result => { HardwareRevision = _stringConversion.Convert(result.Data); });
            else if (characteristic.Uuid == GattConstants.FIRMWARE_REVISION_UUID)
                characteristic.Read().Subscribe(result => { FirmwareRevision = _stringConversion.Convert(result.Data).Replace('-', '.'); });
            else if (characteristic.Uuid == GattConstants.SOFTWARE_REVISION_UUID)
                characteristic.Read().Subscribe(result => { SoftwareRevision = _stringConversion.Convert(result.Data).Replace('-', '.'); });
            else if (characteristic.Uuid == GattConstants.BATTERY_LEVEL_UUID)
                characteristic.Read().Subscribe(result => BatteryLevel = _byteConversion.Convert(result.Data));

            /*
             * Notify Characteristics
             */
            if (characteristic.Uuid == GattConstants.BATTERY_LEVEL_UUID)
                characteristic.RegisterAndNotify().Subscribe(result => BatteryLevel = _byteConversion.Convert(result.Data));
            else if (characteristic.Uuid == GattConstants.HEART_RATE_UUID)
                characteristic.RegisterAndNotify().Subscribe(result => HeartRate = _heartRateConversion.Convert(result.Data));
            else if (characteristic.Uuid == GattConstants.SPO2_UUID)
                characteristic.RegisterAndNotify().Subscribe(result => { });
            else if (characteristic.Uuid == GattConstants.TEMPERATURE_MEASUREMENT_UUID)
                characteristic.RegisterAndNotify(true).Subscribe(result => BodyTemperature = _temperatureConversion.Convert(result.Data));
            else if (characteristic.Uuid == GattConstants.SENSOR_QUALITY_INDEX_UUID)
                characteristic.RegisterAndNotify().Subscribe(result => SensorQualityIndex = _byteConversion.Convert(result.Data));

            /*
             * Write Raw Data Characteristic and Start Notify
             */
            if (characteristic.Uuid == GattConstants.RAW_DATA_UUID)
            {
                var rawDataCharacteristic = characteristic;
                characteristic.Write(RAW_DATA_KEY).Subscribe(result =>
                {
                    rawDataCharacteristic.RegisterAndNotify().Subscribe(notifyResult =>
                    {
                        var convertedRawData = _rawConversion.Convert(notifyResult.Data);

                        if (convertedRawData.StepFrequency != null) this.StepFrequency = convertedRawData.StepFrequency ?? this.StepFrequency;
                        if (convertedRawData.Accelerometer != null) this.Accelerometer = convertedRawData.Accelerometer;
                    });
                });
            }
        }

        private void CheckConnected()
        {
            if (Id == null || ManufacturerName == null || ModelNumber == null || HardwareRevision == null || FirmwareRevision == null || SoftwareRevision == null)
            {
                this.ConnectionState = ConnectionState.CONNECTING;
            }
            else
            {
                ConnectionState = ConnectionState.CONNECTED;
            }
        }
    }


}
