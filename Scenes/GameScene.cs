using Gum.Converters;
using Gum.DataTypes;
using Gum.Forms.Controls;
using MazeGame.Graphics;
using MazeGame.Maze;
using MazeGame.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using System;
using System.Diagnostics;

namespace MazeGame.Scenes;

public class GameScene(Rectangle screen, string levelName) : Scene, IDisposable
{
    private readonly AccelerometerService _accelerometerService = new AccelerometerService();
    private readonly PhysicsService _physicsService = new PhysicsService();
    private Sprite _ball;
    private readonly Rectangle _screenBounds = new Rectangle(0, (int)((screen.Height - screen.Width) / 2), screen.Width, screen.Width); // Telefon na stałe ustawiony w pionie (szerokość < wysokość)
    private MazeControl _mazeControl;
    private System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
    private readonly string levelName = levelName;
    private Panel _pausePanel;

    public GameScene(Rectangle screen, string levelName, Sprite ball, Stopwatch watch)
        : this(screen, levelName)
    {
        _ball = ball;
        this.watch = watch;
        this.watch.Start();
        var maze = Maze.Maze.CreateFromFile(levelName, Content);
        _mazeControl = new MazeControl(maze, _ball, SpriteBatch, _screenBounds);
    }
    public override void Initialize()
    {
        base.Initialize();
        GumService.Default.Root.Children.Clear();

        if (_mazeControl is null)
        {
            var maze = Maze.Maze.CreateFromFile(levelName, Content);
            _mazeControl = new MazeControl(maze, _ball, SpriteBatch, _screenBounds);
            var ballStartPoint = _mazeControl.startPosition;
            _ball.Position = new Vector2(ballStartPoint.X, ballStartPoint.Y);
            _ball.Scale = new Vector2((float)ballStartPoint.Width / (float)_ball.Texture.Width, (float)ballStartPoint.Height / (float)_ball.Texture.Height);
            _ball.TexColor = ColorService.BallColor;
        }

        InitializePausePanel();
    }

    public override void LoadContent()
    {
        if (_ball is null)
        {
            _ball = new Sprite(Content.Load<Texture2D>("Images/ball"),
                new Vector2(10, 10));
        }
        else
        {
            _ball.Velocity = Vector2.Zero;
            _ball.Texture = Content.Load<Texture2D>("Images/ball");
        }
        _accelerometerService.SetObject(_ball);
    }

    public override void Update(GameTime gameTime)
    {
        GumService.Default.Update(gameTime);
        _physicsService.InsideBounce(_ball, _screenBounds);
        if (_mazeControl.ResolveCollisions())
        {
            watch.Stop();
            Game1.timeScore = watch.ElapsedMilliseconds;
            Console.WriteLine(Game1.timeScore);
            var size = levelName.Split("/")[1];
            RaiseSceneChanged(ScreenType.LevelFinished, new () {{"mazeSize", size}});
        }
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(ColorService.MenuBgColor);
        SpriteBatch.Begin();

        _mazeControl.DrawMaze();
        _ball.Draw(SpriteBatch);
        
        SpriteBatch.End();
        GumService.Default.Draw();
    }

    private void InitializePausePanel()
    {
        _pausePanel = new Panel();
        _pausePanel.Dock(Gum.Wireframe.Dock.Top);
        _pausePanel.WidthUnits = DimensionUnitType.PercentageOfParent;
        _pausePanel.HeightUnits = DimensionUnitType.Absolute;
        _pausePanel.Width = 100;
        _pausePanel.Height = 200;
        _pausePanel.AddToRoot();

        var pauseButton = new Button();
        pauseButton.Text = "Pause";
        pauseButton.Anchor(Gum.Wireframe.Anchor.TopLeft);
        pauseButton.Visual.XUnits = GeneralUnitType.Percentage;
        pauseButton.Visual.YUnits = GeneralUnitType.Percentage;
        pauseButton.Visual.WidthUnits = DimensionUnitType.PercentageOfParent;
        pauseButton.Visual.HeightUnits = DimensionUnitType.PercentageOfParent;
        pauseButton.Visual.X = 10;
        pauseButton.Visual.Y = 10;
        pauseButton.Visual.Width = 40f;
        pauseButton.Visual.Height = 20f;
        pauseButton.Click += OnPauseButtonClicked;
        _pausePanel.AddChild(pauseButton);
    }

    private void OnPauseButtonClicked(object sender, EventArgs e)
    {
        watch.Stop();
        RaiseSceneChanged(ScreenType.Pause, new () { {"ball", _ball}, {"levelName", levelName}, {"watch", watch}});
    }

    public override void Dispose()
    {
        base.Dispose();
        _accelerometerService.Dispose();
    }
}
