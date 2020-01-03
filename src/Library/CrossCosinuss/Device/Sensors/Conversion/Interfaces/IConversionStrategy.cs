using System;
using System.Collections.Generic;
using System.Text;

namespace Cosinuss.Library.Device.Sensors.Conversion.Interfaces
{
    internal interface IConversionStrategy<T>
    {
        T Convert(byte[] byteArray);
    }
}
