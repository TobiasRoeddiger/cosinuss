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

        private const int ACC_MIN = 225;
        private const int ACC_MAX = 30;
        private const int ACC_ZERO = 0;

        //private const int COUNT_INDEX = 12; not needed for now

        private const int STEP_FREQUENCY_INDEX = 7;

        public DecodedRawData Convert(byte[] byteArray)
        {
            _eventConversionCounter++;

            var acc_x = ByteToRawAcceleration(byteArray[ACC_X_INDEX]) / 30f;
            var acc_y = ByteToRawAcceleration(byteArray[ACC_Y_INDEX]) / 30f;
            var acc_z = ByteToRawAcceleration(byteArray[ACC_Z_INDEX]) / 30f;
            var accelerometer = new Accelerometer(acc_x, acc_y, acc_z);

            if (_eventConversionCounter >= 50)
            {
                _eventConversionCounter = 0;

                var stepFrequency = byteArray[STEP_FREQUENCY_INDEX] & 255;
                return new DecodedRawData(accelerometer, (byte)stepFrequency);
            }

            return new DecodedRawData(accelerometer);
        }

        private float ByteToRawAcceleration(byte value)
        {
            if (value >= ACC_MIN)
                return value - ACC_MIN;

            if (value == ACC_ZERO)
                return ACC_ZERO;

            if (value <= ACC_MAX)
                return value;

            return ACC_ZERO;
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
