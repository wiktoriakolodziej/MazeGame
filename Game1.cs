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
                new Vector2(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height / 2f));
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
            SetVelocity();

            _spriteBatch.Begin();

            _ball.Draw(_spriteBatch);

            _spriteBatch.End();


            base.Draw(gameTime);
        }

        private void SetVelocity()
        {
            // Calculate the new position of the ball based on the velocity.
            Vector2 newPosition = _ball.Position + _ball.Velocity;

            // Get the bounds of the ball as a rectangle.
            Rectangle ballBounds = new Rectangle(
                (int)_ball.Position.X,
                (int)_ball.Position.Y,
                (int)_ball.Width,
                (int)_ball.Height
            );

            // Get the bounds of the screen as a rectangle.
            Rectangle screenBounds = new Rectangle(
                0,
                0,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight
            );

            // Detect if the ball object is within the screen bounds.
            if (!screenBounds.Contains(ballBounds))
            {
                // Ball would move outside the screen
                // First find the distance from the edge of the ball to each edge of the screen.
                float distanceLeft = Math.Abs(screenBounds.Left - ballBounds.Left);
                float distanceRight = Math.Abs(screenBounds.Right - ballBounds.Right);
                float distanceTop = Math.Abs(screenBounds.Top - ballBounds.Top);
                float distanceBottom = Math.Abs(screenBounds.Bottom - ballBounds.Bottom);

                // Determine which screen edge is the closest.
                float minDistance = Math.Min(
                    Math.Min(distanceLeft, distanceRight),
                    Math.Min(distanceTop, distanceBottom)
                );

                // Determine the normal vector based on which screen edge is the closest.
                Vector2 normal;
                if (minDistance == distanceLeft)
                {
                    // Closest to the left edge.
                    normal = Vector2.UnitX;
                    newPosition.X = 0;
                }
                else if (minDistance == distanceRight)
                {
                    // Closest to the right edge.
                    normal = -Vector2.UnitX;
                    newPosition.X = screenBounds.Right - _ball.Width;
                }
                else if (minDistance == distanceTop)
                {
                    // Closest to the top edge.
                    normal = Vector2.UnitY;
                    newPosition.Y = 0;
                }
                else
                {
                    // Closest to the bottom edge.
                    normal = -Vector2.UnitY;
                    newPosition.Y = screenBounds.Bottom - _ball.Height;
                }

                // Reflect the velocity about the normal.
                _ball.Velocity = Vector2.Reflect(_ball.Velocity, normal);
            }

            // Set the new position of the ball.
            _ball.Position = newPosition;

        }
    }
}
