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
        // CONSTANTS - Device Attributes
        private readonly Guid SYSTEM_ID_UUID = Guid.Parse("00002a23-0000-1000-8000-00805f9b34fb");
        private readonly Guid MANUFACTURER_NAME_UUID = Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb");
        private readonly Guid MODEL_NUMBER_UUID = Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb");
        private readonly Guid HARDWARE_REVISION_UUID = Guid.Parse("00002a27-0000-1000-8000-00805f9b34fb");
        private readonly Guid FIRMWARE_REVISION_UUID = Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb");
        private readonly Guid SOFTWARE_REVISION_UUID = Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb");


        // CONSTANTS - Device State
        private readonly Guid BATTERY_LEVEL_UUID = Guid.Parse("00002a19-0000-1000-8000-00805f9b34fb");

        // CONSTANTS - Sensor Readings
        private readonly Guid HEART_RATE_UUID = Guid.Parse("00002a1c-0000-1000-8000-00805f9b34fb");
        private readonly Guid TEMPERATURE_MEASUREMENT_UUID = Guid.Parse("00002a1c-0000-1000-8000-00805f9b34fb");
        //private readonly Guid HEART_RATE_UUID = Guid.Parse("00002a1c-0000-1000-8000-00805f9b34fb");

        /**
         * DEVICE ATTRIBUTES
         */
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

        private ConnectionState _connectionState;
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

        public float HeartRate => 0.0f;
        public event EventHandler<float> HeartRateChanged;

        public Accelerometer Accelerometer => null;
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
            // TODO: should probably only register once
            _internalDevice.WhenStatusChanged().Subscribe(connectionStatus =>
            {
                if (connectionStatus == ConnectionStatus.Disconnected)
                {
                    ConnectionState = ConnectionState.DISCONNECTED;
                }
                else if (connectionStatus == ConnectionStatus.Disconnecting)
                {
                    ConnectionState = ConnectionState.DISCONNECTING;
                }
                else if (connectionStatus == ConnectionStatus.Connected)
                {
                    _internalDevice.DiscoverServices().Subscribe(gattService =>
                    {
                        gattService.DiscoverCharacteristics().Subscribe(characteristic =>
                        {
                            if (characteristic.Uuid == this.SYSTEM_ID_UUID) characteristic.Read().Subscribe(result => { Id = this.BLEByteArrayToSHexString(result.Data); });
                            else if (characteristic.Uuid == this.MANUFACTURER_NAME_UUID) characteristic.Read().Subscribe(result => { ManufacturerName = this.BLEByteArrayToString(result.Data); });
                            else if (characteristic.Uuid == this.MODEL_NUMBER_UUID) characteristic.Read().Subscribe(result => { ModelNumber = this.BLEByteArrayToString(result.Data); });
                            else if (characteristic.Uuid == this.HARDWARE_REVISION_UUID) characteristic.Read().Subscribe(result => { HardwareRevision = this.BLEByteArrayToString(result.Data); });
                            else if (characteristic.Uuid == this.FIRMWARE_REVISION_UUID) characteristic.Read().Subscribe(result => { FirmwareRevision = this.BLEByteArrayToString(result.Data).Replace('-', '.'); });
                            else if (characteristic.Uuid == this.SOFTWARE_REVISION_UUID) characteristic.Read().Subscribe(result => { SoftwareRevision = this.BLEByteArrayToString(result.Data).Replace('-', '.'); });
                            else if (characteristic.Uuid == this.BATTERY_LEVEL_UUID) characteristic.Read().Subscribe(result => 
                            { 
                                BatteryLevel = BLEByteArrayToInt(result.Data); 
                            });

                            if (characteristic.Uuid == this.BATTERY_LEVEL_UUID) {
                                characteristic.EnableNotifications();
                                characteristic.WhenNotificationReceived().Subscribe(result =>
                                {
                                    BatteryLevel = BLEByteArrayToInt(result.Data);
                                });
                            }

                            //if (characteristic.Uuid == this.BATTERY_LEVEL_UUID) characteristic.RegisterAndNotify().Subscribe(result => { BatteryLevel = BLEByteArrayToInt(result.Data); });

                            //if (characteristic.Uuid == this.HEART_RATE_UUID) characteristic.RegisterAndNotify().Subscribe(result => { HeartRate = BLEByteValueToInt(result.Data); });
                            //if (characteristic.Uuid == this.TEMPERATURE_MEASUREMENT_UUID) characteristic.RegisterAndNotify().Subscribe(result => { BodyTemperature = BLEByteValueToInt(result.Data); });
                        });
                    });
                }
                else if (connectionStatus == ConnectionStatus.Connecting)
                {
                    ConnectionState = ConnectionState.CONNECTING;
                }
            });

            _internalDevice.Connect();
        }

        public void Disconnect()
        {
            // TODO: kill all subscriptions
        }

        private string BLEByteArrayToSHexString(byte[] byteArray)
        {
            return BitConverter.ToString(byteArray).Replace("-", "");
        }

        private string BLEByteArrayToString(byte[] byteArray)
        {
            return (byteArray != null) ? Encoding.Default.GetString(byteArray) : "";
        }

        private int BLEByteArrayToInt(byte[] byteArray)
        {
            return 100;
            return (byteArray != null) ? BitConverter.ToInt32(byteArray, 0) : 0;
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
