using Android.Telecom;
using MazeGame.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Framework.Devices.Sensors;

namespace MazeGame.Services
{
    public class AccelerometerService
    {
        private readonly Accelerometer _accelSensor;
        private Sprite? _movingObject;
        private readonly float _sensitivity;
        public Vector2 accReading; // dbg

        public AccelerometerService(float sensitivity = 0.1f)
        {
            accReading = Vector2.Zero; // dbg
            _sensitivity = sensitivity;
            _accelSensor = new Accelerometer();
            _accelSensor.CurrentValueChanged += ChangeVelocity;
            _accelSensor.Start();       
        }
        public void SetObject(Sprite movingObject)
        {
            _movingObject = movingObject;
        }
        private void ChangeVelocity(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            if (_movingObject is null)
                return;
            //Console.WriteLine($"X: {e.SensorReading.Acceleration.X}, Y: {e.SensorReading.Acceleration.Y}, Z: {e.SensorReading.Acceleration.Z}");
            accReading.X = -e.SensorReading.Acceleration.X; // dbg
            accReading.Y = e.SensorReading.Acceleration.Y; // dbg
            _movingObject.Velocity += _sensitivity * new Vector2(-e.SensorReading.Acceleration.X, e.SensorReading.Acceleration.Y);
        }
    }
}
