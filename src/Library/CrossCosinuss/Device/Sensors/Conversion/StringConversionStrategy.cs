using Cosinuss.Library.Device.Sensors.Conversion.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cosinuss.Library.Device.Sensors.Conversion
{
    internal class StringConversionStrategy : IConversionStrategy<string>
    {
        public string Convert(byte[] byteArray)
        {
            return (byteArray != null) ? Encoding.Default.GetString(byteArray) : "";
        }
    }
}
