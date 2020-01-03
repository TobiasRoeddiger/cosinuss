using Cosinuss.Library.Device.Sensors.Conversion.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cosinuss.Library.Device.Sensors.Conversion
{
    class ByteConversionStrategy : IConversionStrategy<byte>
    {
        public byte Convert(byte[] byteArray)
        {
            return (byteArray != null) ? byteArray[0] : new byte();
        }
    }
}
