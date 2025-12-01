using MazeGame.Graphics;
using MazeGame.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MazeGame.Maze
{
    public class MazeControl(Maze maze, Sprite movingObject, SpriteBatch spriteBatch, Rectangle screen)
    {
        private readonly Maze _maze = maze;
        private readonly PhysicsService _physicsService = new PhysicsService();
        private readonly Sprite _movingObject = movingObject;
        private readonly SpriteBatch _spriteBatch = spriteBatch;
        private readonly Rectangle _screen = screen;

        public void ResolveCollisions()
        {
            
        }

        public void DrawMaze()
        {
            var cellWith = _screen.Width / _maze.Rows;
            var cellHeight = _screen.Height / _maze.Columns;
            for (var x = 0; x < _maze.Rows; x++)
            {
                for (var y = 0; y < _maze.Columns; y++)
                {
                    var source = new Rectangle(x * cellWith, y * cellHeight, (int)cellWith, (int)cellHeight);
                    Draw(_maze.Grid[x,y].Texture, new Vector2(x * cellWith, y * cellHeight), source);
                }
            }
        }
        private void Draw(Texture2D texture, Vector2 position, Rectangle source)
        {
            spriteBatch.Draw(texture, position, source, Color.White);
        }

    }
}
