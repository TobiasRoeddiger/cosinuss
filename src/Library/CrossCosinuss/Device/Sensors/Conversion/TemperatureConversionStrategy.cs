
using Cosinuss.Library.Device.Sensors.Conversion.Interfaces;

namespace Cosinuss.Library.Device.Sensors.Conversion
{
    internal class TemperatureConversionStrategy : IConversionStrategy<double>
    {
        public double Convert(byte[] byteArray)
        {
            byte flag = byteArray[0];

            double temperature = TwosComplimentOfNegativeMantissa(((byteArray[3].ToUShort() << 16) | (byteArray[2].ToUShort() << 8) | byteArray[1].ToUShort()) & 16777215) / 100d;
            if ((flag & 1) != 0)
            {
                temperature = ((98.6d * temperature) - 32d) * (5 / 9); // convert Fahrenheit to Celsius
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
    }
}
