using MazeGame.Graphics;
using MazeGame.Maze;
using MazeGame.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
        private readonly PhysicsService _physicsService;
        private SpriteBatch _spriteBatch;
        private readonly AccelerometerService _accelerometerService;
        private Sprite _ball;
        private Rectangle _screenBounds;
        private MazeControl _mazeControl;
        private Texture2D _debugLine; // dbg

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);    
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.IsFullScreen = true;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.ApplyChanges();
            _physicsService = new PhysicsService();
            _accelerometerService = new AccelerometerService();
        }

        protected override void Initialize()
        {
            base.Initialize();
            _screenBounds = new Rectangle(
                0,
                0,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight
            );
            var maze = Maze.Maze.CreateFromFile("maze.txt", Content);
            _mazeControl = new MazeControl(maze, _ball, _spriteBatch, _screenBounds);
            var ballStartPoint = _mazeControl.GetStartRectangle();
            _ball.Position = new Vector2(ballStartPoint.X, ballStartPoint.Y);
            //_ball.Scale = new Vector2(0.8f);
            _ball.TexColor = Color.Aqua;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _ball = new Sprite(Content.Load<Texture2D>("Images/ball"),
                new Vector2(10, 10));
            _accelerometerService.SetObject(_ball);
            _debugLine = Content.Load<Texture2D>("Images/debug_line"); // dbg


            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _physicsService.InsideBounce(_ball, _screenBounds);
            _mazeControl.ResolveCollisions();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _mazeControl.DrawMaze();
            _ball.Draw(_spriteBatch);
            _spriteBatch.Draw(_debugLine, new Vector2((float)(_ball.Position.X + 0.5*_ball.Width), (float)(_ball.Position.Y + 0.5*_ball.Height)), null, Color.White, (float)Math.Atan2(_accelerometerService.accReading.Y, _accelerometerService.accReading.X), Vector2.Zero, Vector2.One, SpriteEffects.None, 0.0f); // dbg

            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
