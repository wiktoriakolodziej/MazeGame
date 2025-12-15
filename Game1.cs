using Gum.Forms;
using MazeGame.Scenes;
using Microsoft.Data.Sqlite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using Syncfusion.XForms.Android.Core;
using System;
using System.IO;


namespace MazeGame
{
    public enum ScreenType
    {
        Title,
        Gameplay,
        Records,
        LevelFinished
    }

    public class Game1 : Game
    {
        internal static Game1 s_instance;
        public static Game1 Instance => s_instance;

        public static GraphicsDeviceManager _graphics { get; private set; }
        public static new GraphicsDevice GraphicsDevice { get; private set; }
        public static SpriteBatch _spriteBatch { get; private set; }
        public static new ContentManager Content { get; private set; }
        public static long timeScore = 0;
        public static string mazeSize = "10x10";
        public static string sqlitePath = string.Empty;
        
        private static Scene _activeScene;
        private static Scene _nextScene;

        public Game1()
        {
            if (s_instance != null)
            {
                throw new InvalidOperationException($"Only a single Core instance can be created");
            }

            // Store reference to engine for global member access.
            s_instance = this;

            _graphics = new GraphicsDeviceManager(this);
            Content = base.Content;
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
            GraphicsDevice = base.GraphicsDevice;
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _activeScene = new TitleScene();
            _activeScene.OnSceneChanged += ChangeScene;
            InitializeGum();
            _activeScene.Initialize();

            sqlitePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Replace("/.config", ""), "scores.db");
            //File.Delete("/data/user/0/MazeGame.MazeGame/files/scores.db");
            using var connection = new SqliteConnection($"Data Source={sqlitePath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = """
                CREATE TABLE IF NOT EXISTS scores (
                    id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    maze_size TEXT NOT NULL,
                    time REAL NOT NULL
                );
                """;
            command.ExecuteNonQuery();

            //command.CommandText = "INSERT INTO scores VALUES(NULL, \"10x10\", 35.143)";
            //command.ExecuteNonQuery();

            //command.CommandText = "SELECT * FROM scores";
            //using var reader = command.ExecuteReader();

            //while (reader.Read())
            //{
            //    var name = reader.GetString(0);

            //    Console.WriteLine($"Hello, {name}!");
            //}
        }

        protected override void LoadContent()
        {
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

        public static void ChangeScene(ScreenType type)
        {
            switch (type)
            {
                case ScreenType.Title:
                    _nextScene = new TitleScene();
                    break;
                case ScreenType.Gameplay:
                    var _screenBounds = new Rectangle(0, 0,
                        GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                        GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
                    _nextScene = new GameScene(_screenBounds);
                    break;
                case ScreenType.LevelFinished: 
                    _nextScene = new LevelFinishedScene(); 
                    break;
                case ScreenType.Records:
                    _nextScene = new RecordScene();
                    break;
            }
            if(_nextScene is not null)
                _nextScene.OnSceneChanged += ChangeScene;
        }

        private static void TransitionScene()
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
            GumService.Default.ContentLoader.XnaContentManager = Game1.Content;

            // The assets created for the UI were done so at 1/4th the size to keep the size of the
            // texture atlas small.  So we will set the default canvas size to be 1/4th the size of
            // the game's resolution then tell gum to zoom in by a factor of 4.
            GumService.Default.CanvasWidth = GraphicsDevice.PresentationParameters.BackBufferWidth / 4.0f;
            GumService.Default.CanvasHeight = GraphicsDevice.PresentationParameters.BackBufferHeight / 4.0f;
            GumService.Default.Renderer.Camera.Zoom = 4.0f;
        }
    }
}
