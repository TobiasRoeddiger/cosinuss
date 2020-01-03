namespace Cosinuss.Library.Device.Sensors
{
    public class Accelerometer
    {
        public int X { get; private set; } = 0;

        public int Y { get; private set; } = 0;

        public int Z { get; private set; } = 0;

        public Accelerometer(int x, int y, int z)
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