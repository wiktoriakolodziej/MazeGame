using System;
using MazeGame.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.Devices.Sensors;

namespace MazeGame
{

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private static Accelerometer _accelSensor;
        private Sprite _ball;
        private Rectangle spriteBounds;
        private Vector3 _accVec;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            //spriteBounds = new Rectangle(
            //    (int)_ball.X,
            //    (int)_ball.Y,
            //    (int)_ball.Width,
            //    (int)_ball.Height
            //);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            _accelSensor ??= new Accelerometer();
            _accelSensor.CurrentValueChanged += ChangeVelocity;
            _accelSensor.Start();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _ball = new Sprite(Content.Load<Texture2D>("Images/ball"), 
                new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2));
            _ball.CenterOrigin();
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        private void ChangeVelocity(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            Console.WriteLine($"X: {e.SensorReading.Acceleration.X}, Y: {e.SensorReading.Acceleration.Y}, Z: {e.SensorReading.Acceleration.Z}");
            _ball.Velocity += 0.1f * new Vector2(e.SensorReading.Acceleration.X, e.SensorReading.Acceleration.Y);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            var vec = _ball.Position - _ball.Velocity;
            _ball.Draw(_spriteBatch, vec);

            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
