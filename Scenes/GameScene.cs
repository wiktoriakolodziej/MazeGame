using MazeGame.Graphics;
using MazeGame.Maze;
using MazeGame.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGame.Scenes;

public class GameScene : Scene
{
    private readonly AccelerometerService _accelerometerService;
    private readonly PhysicsService _physicsService;
    private Sprite _ball;
    private Rectangle _screenBounds;
    private MazeControl _mazeControl;
    private Texture2D _debugLine; // dbg
    public GameScene(ContentManager gameContentManager, GraphicsDevice gameGraphicsDevice, SpriteBatch spriteBatch) : base(gameContentManager, gameGraphicsDevice, spriteBatch)
    {
        _physicsService = new PhysicsService();
        _accelerometerService = new AccelerometerService();
    }

    public override void Initialize()
    {
        base.Initialize();
        _screenBounds = new Rectangle(
                0,
                0,
                Device.PresentationParameters.BackBufferWidth,
                Device.PresentationParameters.BackBufferHeight
            );
        var maze = Maze.Maze.CreateFromFile("maze.txt", Content);
        _mazeControl = new MazeControl(maze, _ball, _spriteBatch, _screenBounds);
        var ballStartPoint = _mazeControl.GetStartRectangle();
        _ball.Position = new Vector2(ballStartPoint.X, ballStartPoint.Y);
        //_ball.Scale = new Vector2(0.8f);
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
        _mazeControl.ResolveCollisions();
    }

    public override void Draw(GameTime gameTime)
    {

        Device.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        _mazeControl.DrawMaze();
        _ball.Draw(_spriteBatch);
        _spriteBatch.Draw(_debugLine, new Vector2((float)(_ball.Position.X + 0.5 * _ball.Width), (float)(_ball.Position.Y + 0.5 * _ball.Height)), null, Color.White, (float)Math.Atan2(_accelerometerService.accReading.Y, _accelerometerService.accReading.X), Vector2.Zero, Vector2.One, SpriteEffects.None, 0.0f); // dbg

        _spriteBatch.End();
    }
}
