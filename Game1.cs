using MazeGame.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.Devices.Sensors;
using System;
using System.Collections.Generic;

namespace MazeGame
{

    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private static Accelerometer _accelSensor;
        private Sprite _ball;
        private Sprite _obstacle;
        private readonly List<Rectangle> _obstacles = [];

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);    
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.IsFullScreen = true;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();
            _accelSensor ??= new Accelerometer();
            _accelSensor.CurrentValueChanged += ChangeVelocity;
            _accelSensor.Start();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _ball = new Sprite(Content.Load<Texture2D>("Images/ball"),
                new Vector2(0,0));
            _obstacle = new(Content.Load<Texture2D>("Images/obstacle"),
                new Vector2(GraphicsDevice.Viewport.Width / 2f, GraphicsDevice.Viewport.Height / 2f));
            _obstacle.TexColor = Color.Red;
            _obstacles.Add(new Rectangle((int)_obstacle.Position.X, (int) _obstacle.Position.Y, (int)_obstacle.Width, (int)_obstacle.Height));
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            SetVelocity();

            base.Update(gameTime);
        }

        private void ChangeVelocity(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            Console.WriteLine($"X: {e.SensorReading.Acceleration.X}, Y: {e.SensorReading.Acceleration.Y}, Z: {e.SensorReading.Acceleration.Z}");
            _ball.Velocity += 0.1f * new Vector2(-e.SensorReading.Acceleration.X, e.SensorReading.Acceleration.Y);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _ball.Draw(_spriteBatch);
            _obstacle.Draw(_spriteBatch);

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
            Rectangle screenBounds = new Rectangle(
                0,
                0,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight
            );
            int ballRadius = (int)(_ball.Width / 2);
            Vector2 ballCenter = new Vector2((int)_ball.Position.X + ballRadius, (int)_ball.Position.Y + ballRadius);

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


            foreach (var element in _obstacles)
            {
                if (element.Intersects(ballBounds))
                {
                    // Ball would move outside the screen
                    // First find the distance from the edge of the ball to each edge of the screen.
                    float distanceLeft = Math.Abs(element.Left - ballCenter.X);
                    float distanceRight = Math.Abs(element.Right - ballCenter.X);
                    float distanceTop = Math.Abs(element.Top - ballCenter.Y);
                    float distanceBottom = Math.Abs(element.Bottom - ballCenter.Y);

                    // Determine which screen edge is the closest.
                    float minDistanceX = Math.Min(distanceLeft, distanceRight);
                    float minDistanceY = Math.Min(distanceTop, distanceBottom);
                    double distance = Math.Sqrt((minDistanceX * minDistanceX) + (minDistanceY * minDistanceY));
                    //float minDistance = Math.Min(
                    //    Math.Min(distanceLeft, distanceRight),
                    //    Math.Min(distanceTop, distanceBottom)
                    //);

                    

                    // Determine the normal vector based on which screen edge is the closest.
                    if (distance <= ballRadius)
                    {
                        Vector2 normal = new Vector2(ballCenter.X - element.Center.X, ballCenter.Y - element.Center.Y);
                        normal.Normalize();
                        if (distance < ballRadius)
                        {
                            // TODO chyba dobry kierunek przesunięcia, ale za bardzo się przesuwa
                            newPosition.X = (float)(ballBounds.X + distance*normal.X);
                            newPosition.Y = (float)(ballBounds.Y + distance*normal.Y);
                        }
                        // Reflect the velocity about the normal.
                        _ball.Velocity = Vector2.Reflect(_ball.Velocity, normal);
                        //if (minDistanceX == distanceLeft)
                        //{
                        //    // Closest to the left edge.
                        //    //normal = -Vector2.UnitX;
                        //    newPosition.X = element.Left - _ball.Width;
                        //}
                        //else
                        //{
                        //    // Closest to the right edge.
                        //    //normal = Vector2.UnitX;
                        //    newPosition.X = element.Right;
                        //}
                        //if (minDistanceY == distanceTop)
                        //{
                        //    // Closest to the top edge.
                        //    //normal = -Vector2.UnitY;
                        //    newPosition.Y = element.Top - _ball.Height;
                        //}
                        //else
                        //{
                        //    // Closest to the bottom edge.
                        //    //normal = Vector2.UnitY;
                        //    newPosition.Y = element.Bottom;
                        //}
                    }
                    

                }
            }

            // Set the new position of the ball.
            _ball.Position = newPosition;
            // Apply friction to the ball's velocity.
            _ball.Velocity *= 0.99f;

        }
    }
}
