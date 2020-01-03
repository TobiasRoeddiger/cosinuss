using Cosinuss.Library.Device.Interfaces;
using System;

namespace Cosinuss.Library
{
    public class Cross
    {
        private static readonly Lazy<ICosinussDeviceManager> implementation = new Lazy<ICosinussDeviceManager>(CreateCosinussDeviceManager, System.Threading.LazyThreadSafetyMode.PublicationOnly);
        public static ICosinussDeviceManager Current
        {
            get
            {
                var toReturn = implementation.Value;

                if (toReturn == null)
                {
                    throw new NotImplementedException();
                }

                return toReturn;
            }
        }

        private static ICosinussDeviceManager CreateCosinussDeviceManager()
        {
            // could possibly check for preconditions here in the future (if needed)
            return new CosinussDeviceManager();
        }

        private Cross()
        {

        }
    }
}
