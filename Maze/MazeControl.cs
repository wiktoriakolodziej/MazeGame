using MazeGame.Graphics;
using MazeGame.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Color = Microsoft.Xna.Framework.Color;

namespace MazeGame.Maze
{
    public record CollidingCell(bool IsTrap, Rectangle Position);
    public class MazeControl(Maze maze, Sprite movingObject, SpriteBatch spriteBatch, Rectangle screen)
    {
        private readonly Maze _maze = maze;
        private readonly PhysicsService _physicsService = new PhysicsService();
        private readonly Sprite _movingObject = movingObject;
        private readonly SpriteBatch _spriteBatch = spriteBatch;
        private readonly Rectangle _screen = screen;
        private readonly int _cellDim = screen.Width / maze.Columns;
        //private readonly int _topOffset = (screen.Height - screen.Width) / 2;
        private Vector2 _cellScale => new Vector2((float)_cellDim/_maze.Grid[0, 0].Texture.Width, (float)_cellDim/ _maze.Grid[0, 0].Texture.Height);
        public Rectangle startPosition => CalculateStartRectangle();

        public bool ResolveCollisions()
        {
            if (CheckEnd())
            {
                _movingObject.Position = new Vector2(startPosition.X, startPosition.Y);
                _movingObject.Velocity = Vector2.Zero;
                return true;
            }
            var (mObjectXIndex, mObjectYIndex) = GetCellIndices(_movingObject.Position);
            var adjacentCells = GetAdjacentCells(mObjectXIndex, mObjectYIndex);
            _physicsService.OutsideBounce(_movingObject, adjacentCells, startPosition);
            return false;
        }

        private (int, int) GetCellIndices(Vector2 position)
        {
            var yIndex = (int)(position.X / _cellDim);
            var xIndex = (int)(((position.Y - _screen.Y) / _cellDim));
            return (xIndex, yIndex);
        }
        private bool CheckEnd()
        {
            var endPoint = _maze.EndPosition;
            if (!endPoint.HasValue)
                return false;
            var (endX, endY) = endPoint.Value;
            var endRect = new Rectangle(endY * _cellDim, endX * _cellDim + _screen.Y, _cellDim, _cellDim);
            var ballBounds = new Rectangle(
                (int)movingObject.Position.X,
                (int)movingObject.Position.Y,
                (int)movingObject.Width,
                (int)movingObject.Height
            );
            return endRect.Intersects(ballBounds);
        }

        private List<CollidingCell> GetAdjacentCells(int xIndex, int yIndex, int radius = 1)
        {
            var adjacentCells = new List<CollidingCell>();
            for (var x = xIndex - radius; x <= xIndex + radius; x++)
            {
                for (var y = yIndex - radius; y <= yIndex + radius; y++)
                {
                    if (x >= 0 && x < _maze.Rows && y >= 0 && y < _maze.Columns && (_maze.Grid[x,y].Type == CellType.Wall || _maze.Grid[x, y].Type == CellType.Trap))
                        adjacentCells.Add(new CollidingCell(_maze.Grid[x, y].Type == CellType.Trap, new Rectangle(y * _cellDim, x * _cellDim + _screen.Y, _cellDim, _cellDim)));
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
                    var source = new Rectangle(y * _cellDim, x * _cellDim, _cellDim, _cellDim);
                    Draw(_maze.Grid[x,y].Texture, new Vector2(y * _cellDim, x * _cellDim + _screen.Y), _maze.Grid[x,y].Color);
                }
            }
        }
        private void Draw(Texture2D texture, Vector2 position, Color color)
        {
            spriteBatch.Draw(texture, position, null, color, 0.0f, Vector2.Zero, _cellScale, SpriteEffects.None, 0.0f);
        }

        public Rectangle CalculateStartRectangle()
        {
            var startPoint = _maze.StartPosition;
            var scale = _cellDim * 0.7f;

            if (!startPoint.HasValue)
                return new Rectangle(0, 0, (int)scale, (int)scale);
            
            var (x, y) = startPoint.Value;
            // Wspolrzedne w pozostalych funkcjach wydaja sie ok, a tu trzeba je zamienic
            return new Rectangle(y * _cellDim + 3, x * _cellDim + _screen.Y + 3, (int)scale, (int)scale);
        }
    }
}
