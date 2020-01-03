using Cosinuss.Library.Device.Sensors.Conversion.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cosinuss.Library.Device.Sensors.Conversion
{
    internal class HeartRateConversionStrategy : IConversionStrategy<ushort>
    {
        public ushort Convert(byte[] byteArray)
        {
            ushort bpm = byteArray[1];
            if (!((byteArray[0] & 0x01) == 0))
                bpm = (ushort)(((bpm >> 8) & 0xFF) | ((bpm << 8) & 0xFF00));

            return bpm;
        }
    }
}
