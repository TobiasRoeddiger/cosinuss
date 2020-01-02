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
        private readonly Guid DATA_QUALITY_INDEX_UUID = Guid.Parse("0000a002-1212-EFDE-1523-785FEABCD123");
        private readonly Guid HEART_RATE_UUID = Guid.Parse("00002a37-0000-1000-8000-00805f9b34fb");
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

        private byte _dataQualityIndex;
        public byte DataQualityIndex
        {
            get
            {
                return _dataQualityIndex;
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
        public event EventHandler<byte> DataQualityIndexChanged;

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
                                BatteryLevel = BLEByteArrayToByte(result.Data);
                            });


                            if (characteristic.Uuid == this.BATTERY_LEVEL_UUID)
                            {
                                characteristic.RegisterAndNotify().Subscribe(result =>
                                {
                                    BatteryLevel = BLEByteArrayToByte(result.Data);
                                });
                            }
                            else if (characteristic.Uuid == this.HEART_RATE_UUID)
                            {
                                characteristic.RegisterAndNotify().Subscribe(result =>
                                {
                                    ushort bpm = 0;
                                    if ((result.Data[0] & 0x01) == 0)
                                    {
                                        bpm = result.Data[1];
                                    }
                                    else
                                    {
                                        bpm = result.Data[1];
                                        bpm = (ushort)(((bpm >> 8) & 0xFF) | ((bpm << 8) & 0xFF00));
                                    }

                                    HeartRate = bpm;
                                });
                            }
                            else if (characteristic.Uuid == this.TEMPERATURE_MEASUREMENT_UUID)
                            {
                                Console.WriteLine("Registering Body Temperature");
                                characteristic.RegisterAndNotify(true).Subscribe(result =>
                                {
                                    Console.WriteLine("Body Temperature");
                                    BodyTemperature = BLEByteArrayToTemperature(result.Data) / 100d;
                                });
                            }
                            else if (characteristic.Uuid == this.DATA_QUALITY_INDEX_UUID)
                            {
                                Console.WriteLine("Registering DQ");
                                characteristic.RegisterAndNotify().Subscribe(result =>
                                {
                                    DataQualityIndex = BLEByteArrayToByte(result.Data);
                                });
                            }
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

        private byte BLEByteArrayToByte(byte[] byteArray)
        {
            return (byteArray != null) ? (byteArray[0]) : new byte();
        }

        private short BLEByteArrayToShort(byte[] byteArray)
        {
            return (byteArray != null) ? BitConverter.ToInt16(byteArray, 0) : (short)0;
        }

        private double BLEByteArrayToTemperature(byte[] byteArray)
        {
            byte flag = byteArray[0];

            double temperature = TwosComplimentOfNegativeMantissa(((byteArray[3].ToUShort() << 16) | (byteArray[2].ToUShort() << 8) | byteArray[1].ToUShort()) & 16777215);

            if ((flag & 1) != 0)
            {
                return ((98.6d * temperature) - 32d) * (5 / 9); // convert Fahrenheit to Celsius
            }

            return temperature;
        }

        private int TwosComplimentOfNegativeMantissa(int mantissa)
        {
            if ((4194304 & mantissa) != 0)
            {
                return (((mantissa ^ -1) & 16777215) + 1) * -1;
            }

            return mantissa;
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

    public static class Conversion
    {
        public static ushort ToUShort(this Byte octet)
        {
            if (octet < 0)
            {
                return (ushort)(octet & 255);
            }
            return octet;
        }
    }
}
