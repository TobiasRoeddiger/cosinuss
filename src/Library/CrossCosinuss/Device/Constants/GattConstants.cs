using System;
using System.Collections.Generic;
using System.Text;

namespace Cosinuss.Library.Device.Constants
{
    internal static class GattConstants
    {
        // CONSTANTS - Device Attributes
        internal static Guid SYSTEM_ID_UUID = Guid.Parse("00002a23-0000-1000-8000-00805f9b34fb");
        internal static Guid MANUFACTURER_NAME_UUID = Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb");
        internal static Guid MODEL_NUMBER_UUID = Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb");
        internal static Guid HARDWARE_REVISION_UUID = Guid.Parse("00002a27-0000-1000-8000-00805f9b34fb");
        internal static Guid FIRMWARE_REVISION_UUID = Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb");
        internal static Guid SOFTWARE_REVISION_UUID = Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb");


        // CONSTANTS - Device State
        internal static Guid BATTERY_LEVEL_UUID = Guid.Parse("00002a19-0000-1000-8000-00805f9b34fb");

        // CONSTANTS - Sensor Readings
        internal static Guid SENSOR_QUALITY_INDEX_UUID = Guid.Parse("0000a002-1212-EFDE-1523-785FEABCD123");
        internal static Guid HEART_RATE_UUID = Guid.Parse("00002a37-0000-1000-8000-00805f9b34fb");
        internal static Guid SPO2_UUID = Guid.Parse("00002A5F-0000-1000-8000-00805f9B34fb");
        internal static Guid TEMPERATURE_MEASUREMENT_UUID = Guid.Parse("00002a1c-0000-1000-8000-00805f9b34fb");
        internal static Guid RAW_DATA_UUID = Guid.Parse("0000a001-1212-efde-1523-785feabcd123");
    }
}
