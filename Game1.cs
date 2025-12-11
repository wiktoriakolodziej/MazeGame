using Gum.Forms;
using MazeGame.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using System;


namespace MazeGame
{
    public enum ScreenType
    {
        Title,
        Gameplay,
        Records
    }

    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch { get; private set; }
        
        private Scene _activeScene;
        private Scene _nextScene;

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
            InitializeGum();
            _activeScene.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _activeScene = new TitleScene(GraphicsDevice, _spriteBatch, Content.ServiceProvider, Content.RootDirectory);
            _activeScene.OnSceneChanged += ChangeScene;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (_nextScene != null)
                TransitionScene();

            if (_activeScene != null)
                _activeScene.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (_activeScene != null)
                _activeScene.Draw(gameTime);

            base.Draw(gameTime);
        }

        public void ChangeScene(ScreenType type)
        {
            switch (type)
            {
                case ScreenType.Title:
                    _nextScene = new TitleScene(GraphicsDevice, _spriteBatch, Content.ServiceProvider, Content.RootDirectory);
                    break;
                case ScreenType.Gameplay:
                    var _screenBounds = new Rectangle(0, 0,
                        GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                        GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
                    _nextScene = new GameScene(_screenBounds, GraphicsDevice, _spriteBatch, Content.ServiceProvider, Content.RootDirectory);
                    break;
                case ScreenType.Records:
                    break;
            }
            _nextScene.OnSceneChanged += ChangeScene;
        }

        private void TransitionScene()
        {
            if (_activeScene is not null)
                _activeScene.Dispose();

            GC.Collect();
            _activeScene = _nextScene;
            _nextScene = null;

            if (_activeScene is not null)
                _activeScene.Initialize();
        }

        private void InitializeGum()
        {
            // Initialize the Gum service. The second parameter specifies
            // the version of the default visuals to use. V2 is the latest
            // version.
            GumService.Default.Initialize(this, DefaultVisualsVersion.V2);

            // Tell the Gum service which content manager to use.  We will tell it to
            // use the global content manager from our Core.
            GumService.Default.ContentLoader.XnaContentManager = this.Content;

            // The assets created for the UI were done so at 1/4th the size to keep the size of the
            // texture atlas small.  So we will set the default canvas size to be 1/4th the size of
            // the game's resolution then tell gum to zoom in by a factor of 4.
            GumService.Default.CanvasWidth = GraphicsDevice.PresentationParameters.BackBufferWidth / 4.0f;
            GumService.Default.CanvasHeight = GraphicsDevice.PresentationParameters.BackBufferHeight / 4.0f;
            GumService.Default.Renderer.Camera.Zoom = 4.0f;
        }
    }
}
