using System;
using System.Collections.Generic;
using System.Text;

namespace Cosinuss.Library.Device.Sensors.Conversion
{
    internal static class ConversionExtensions
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
