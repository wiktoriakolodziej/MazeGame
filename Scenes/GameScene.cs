using MazeGame.Graphics;
using MazeGame.Maze;
using MazeGame.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MazeGame.Scenes;

public class GameScene(Rectangle screen, string levelName) : Scene
{
    private readonly AccelerometerService _accelerometerService = new AccelerometerService();
    private readonly PhysicsService _physicsService = new PhysicsService();
    private Sprite _ball;
    private Rectangle _screenBounds = screen;
    private MazeControl _mazeControl;
    private Texture2D _debugLine; // dbg
    private System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
    private readonly string levelName = levelName;

    public override void Initialize()
    {
        base.Initialize();
        var maze = Maze.Maze.CreateFromFile(levelName, Content);
        _mazeControl = new MazeControl(maze, _ball, SpriteBatch, _screenBounds);
        var ballStartPoint = _mazeControl.GetStartRectangle();
        _ball.Position = new Vector2(ballStartPoint.X, ballStartPoint.Y);
        _ball.Scale = new Vector2((float)ballStartPoint.Width / (float)_ball.Texture.Width, (float)ballStartPoint.Height / (float)_ball.Texture.Height);
        _ball.TexColor = Color.Aqua;
    }

    public override void LoadContent()
    {
        _ball = new Sprite(Content.Load<Texture2D>("Images/ball"),
            new Vector2(10, 10));
        _accelerometerService.SetObject(_ball);
        _debugLine = Content.Load<Texture2D>("Images/debug_line"); // dbg
    }

    public override void Update(GameTime gameTime)
    {
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
        GraphicsDevice.Clear(Color.AliceBlue);
        SpriteBatch.Begin();

        _mazeControl.DrawMaze();
        _ball.Draw(SpriteBatch);
        SpriteBatch.Draw(_debugLine, new Vector2((float)(_ball.Position.X + 0.5 * _ball.Width), (float)(_ball.Position.Y + 0.5 * _ball.Height)), null, Color.White, (float)Math.Atan2(_accelerometerService.accReading.Y, _accelerometerService.accReading.X), Vector2.Zero, Vector2.One, SpriteEffects.None, 0.0f); // dbg

        SpriteBatch.End();
    }
}
