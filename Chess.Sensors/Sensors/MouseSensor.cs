using System;
using System.Drawing;
using System.Windows.Forms;

namespace Chess.Sensors
{
    public class MouseSensor : PassiveSensor
    {
        public Control BoardControl { get; private set; }

        public MouseSensor(Control boardControl)
            : base(10)
        {
            this.BoardControl = boardControl;
        }

        protected override SensorData GetSensorData()
        {
            var point = (Point)this.BoardControl.Invoke((Delegate)(Func<object>)(() => this.BoardControl.PointToClient(Cursor.Position)));
            var square = point.ToSquare();
            if ((int)square == -1)
                return null;

            return new MouseSensorData(point);
        }

        public static SensorData Deserialize(System.IO.BinaryReader reader)
        {
            return new MouseSensorData(new Point(reader.ReadInt16(), reader.ReadInt16()));
        }
    }

    public class MouseSensorData : SensorData
    {
        public Point Location { get; private set; }

        public MouseSensorData(Point location)
        {
            this.Location = location;
        }

        public override void Serialize(System.IO.BinaryWriter writer)
        {
            writer.Write((short)this.Location.X);
            writer.Write((short)this.Location.Y);
        }

        public override string ToString()
        {
            return this.Location.ToSquare().ToString();
            //return new Point(this.X, this.Y).ToString();
        }

        public override Type GetSensorType()
        {
            return typeof(MouseSensor);
        }
    }
}