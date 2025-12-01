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
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _ball = new Sprite(Content.Load<Texture2D>("Images/ball"),
                new Vector2(10, 10));
            _accelerometerService.SetObject(_ball);
            

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _physicsService.InsideBounce(_ball, _screenBounds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _mazeControl.DrawMaze();
            _ball.Draw(_spriteBatch);

            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
