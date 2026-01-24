using System;
using Android.Telecom;
using MazeGame.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Framework.Devices.Sensors;

namespace MazeGame.Services
{
    public class AccelerometerService : IDisposable
    {
        private Accelerometer _accelSensor;
        private Sprite? _movingObject;
        private readonly float _sensitivity;

        public AccelerometerService(float sensitivity = 0.15f)
        {
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
            _movingObject.Velocity += _sensitivity * new Vector2(-e.SensorReading.Acceleration.X, e.SensorReading.Acceleration.Y);
        }

        public void Dispose()
        {
            _accelSensor.Dispose();
            _accelSensor = null;
        }
    }
}
