namespace Cosinuss.Library.Device.Sensors
{
    public class Accelerometer
    {
        public float X { get; private set; } = 0;

        public float Y { get; private set; } = 0;

        public float Z { get; private set; } = 0;

        public Accelerometer(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Accelerometer()
        {

        }
    }
}