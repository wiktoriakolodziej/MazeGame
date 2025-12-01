using Java.Lang;
using MazeGame.Graphics;
using MazeGame.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Color = Microsoft.Xna.Framework.Color;

namespace MazeGame.Maze
{
    public class MazeControl(Maze maze, Sprite movingObject, SpriteBatch spriteBatch, Rectangle screen)
    {
        private readonly Maze _maze = maze;
        private readonly PhysicsService _physicsService = new PhysicsService();
        private readonly Sprite _movingObject = movingObject;
        private readonly SpriteBatch _spriteBatch = spriteBatch;
        private readonly Rectangle _screen = screen;
        private readonly int _cellWidth = screen.Width / maze.Rows;
        private readonly int _cellHeight = screen.Height / maze.Columns;

        public void ResolveCollisions()
        {
            var (mObjectXIndex, mObjectYIndex) = GetCellIndices(_movingObject.Position);
            var adjacentCells = GetAdjacentCells(mObjectXIndex, mObjectYIndex);
            _physicsService.OutsideBounce(_movingObject, adjacentCells);
        }

        private (int, int) GetCellIndices(Vector2 position)
        {
            var cellWith = _screen.Width / _maze.Rows;
            var cellHeight = _screen.Height / _maze.Columns;
            var xIndex = (int)(position.X / cellWith);
            var yIndex = (int)(position.Y / cellHeight);
            return (xIndex, yIndex);
        }

        private List<Rectangle> GetAdjacentCells(int xIndex, int yIndex, int radius = 2)
        {
            var adjacentCells = new List<Rectangle>();
            for (var x = xIndex - radius; x <= xIndex + radius; x++)
            {
                for (var y = yIndex - radius; y <= yIndex + radius; y++)
                {
                    if (x >= 0 && x < _maze.Rows && y >= 0 && y < _maze.Columns && (x == xIndex || y == yIndex) && _maze.Grid[x,y].Type == CellType.Wall)
                        adjacentCells.Add(new Rectangle(x * _cellWidth, y * _cellHeight, _cellWidth, _cellHeight));
                }
            }
            return adjacentCells;
        }

        public void DrawMaze()
        {
            for (var x = 0; x < _maze.Rows; x++)
            {
                for (var y = 0; y < _maze.Columns; y++)
                {
                    var source = new Rectangle(x * _cellWidth, y * _cellHeight, _cellWidth, _cellHeight);
                    Draw(_maze.Grid[x,y].Texture, new Vector2(x * _cellWidth, y * _cellHeight), source);
                }
            }
        }
        private void Draw(Texture2D texture, Vector2 position, Rectangle source)
        {
            spriteBatch.Draw(texture, position, source, Color.White);
        }

        public Rectangle GetStartRectangle()
        {
            var startPoint = _maze.StartPosition;
            var scale = Math.Min(_cellHeight, _cellWidth) * 0.8f;

            if (!startPoint.HasValue)
                return new Rectangle(0, 0, (int)scale, (int)scale);
            
            var (x, y) = startPoint.Value;
            return new Rectangle(x * _cellWidth, y * _cellHeight, (int)scale, (int)scale);
        }
    }
}
