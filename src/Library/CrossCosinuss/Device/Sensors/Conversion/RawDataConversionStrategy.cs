using Cosinuss.Library.Device.Sensors;
using Cosinuss.Library.Device.Sensors.Conversion.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cross.Device.Sensors.Conversion
{
    internal class RawDataConversionStrategy : IConversionStrategy<DecodedRawData>
    {
        private int _eventConversionCounter = 0; // only return step frequency after every 50th event

        private const int ACC_X_INDEX = 18;
        private const int ACC_Y_INDEX = 16;
        private const int ACC_Z_INDEX = 14; //correct

        private const int COUNT_INDEX = 12;

        private const int STEP_FREQUENCY_INDEX = 7;

        public DecodedRawData Convert(byte[] byteArray)
        {
            var max = 30;
            var min = 255;
            _eventConversionCounter++;

            Console.WriteLine(BitConverter.ToString(new byte[] { byteArray[14] }) + ", " + BitConverter.ToString(new byte[] { byteArray[16] }) + ", " + BitConverter.ToString(new byte[] { byteArray[18] }));

            var acc_x = byteArray[ACC_X_INDEX] & 255; //-1f + (byteArray[ACC_X_INDEX] / 128f);
            var acc_y = byteArray[ACC_Y_INDEX] & 255; //-1f + (byteArray[ACC_Y_INDEX] / 128f);
            var acc_z = byteArray[ACC_Z_INDEX] & 255; //-1f + (byteArray[ACC_Z_INDEX] / 128f);
            var accelerometer = new Accelerometer(acc_x, acc_y, acc_z);

            if (_eventConversionCounter >= 50)
            {
                _eventConversionCounter = 0;

                var stepFrequency = byteArray[STEP_FREQUENCY_INDEX] & 255;
                return new DecodedRawData(accelerometer, (byte)stepFrequency);
            }

            return new DecodedRawData(accelerometer);
        }
    }

    internal class DecodedRawData
    {
        public byte? StepFrequency { get; private set; }
        public Accelerometer Accelerometer { get; private set; } = null;

        public DecodedRawData(Accelerometer accelerometer, byte? stepFrequency = null)
        {
            this.Accelerometer = accelerometer;
            this.StepFrequency = stepFrequency;
        }
    }
}
