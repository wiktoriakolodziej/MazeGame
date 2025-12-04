using MazeGame.Graphics;
using MazeGame.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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
        private readonly int _cellWidth = screen.Width / maze.Columns;
        private readonly int _cellHeight = screen.Height / maze.Rows;
        private Vector2 _cellScale => new Vector2((float)_cellWidth/_maze.Grid[0, 0].Texture.Width, (float)_cellHeight/ _maze.Grid[0, 0].Texture.Height);

        public void ResolveCollisions()
        {
            if (CheckEnd())
            {
                var ballStartPoint = GetStartRectangle();
                _movingObject.Position = new Vector2(ballStartPoint.X, ballStartPoint.Y);
                _movingObject.Velocity = Vector2.Zero;
            }
            var (mObjectXIndex, mObjectYIndex) = GetCellIndices(_movingObject.Position);
            var adjacentCells = GetAdjacentCells(mObjectXIndex, mObjectYIndex);
            _physicsService.OutsideBounce(_movingObject, adjacentCells);
        }

        private (int, int) GetCellIndices(Vector2 position)
        {
            var screenWidth = _screen.Width / _maze.Columns;
            var cellHeight = _screen.Height / _maze.Rows;
            var yIndex = (int)(position.X / screenWidth);
            var xIndex = (int)(position.Y / cellHeight);
            return (xIndex, yIndex);
        }
        private bool CheckEnd()
        {
            var endPoint = _maze.EndPosition;
            if (!endPoint.HasValue)
                return false;
            var (endX, endY) = endPoint.Value;
            var endRect = new Rectangle(endY * _cellWidth, endX * _cellHeight, _cellWidth, _cellHeight);
            var ballBounds = new Rectangle(
                (int)movingObject.Position.X,
                (int)movingObject.Position.Y,
                (int)movingObject.Width,
                (int)movingObject.Height
            );
            return endRect.Intersects(ballBounds);
        }

        private List<Rectangle> GetAdjacentCells(int xIndex, int yIndex, int radius = 1)
        {
            var adjacentCells = new List<Rectangle>();
            for (var x = xIndex - radius; x <= xIndex + radius; x++)
            {
                for (var y = yIndex - radius; y <= yIndex + radius; y++)
                {
                    if (x >= 0 && x < _maze.Rows && y >= 0 && y < _maze.Columns && _maze.Grid[x,y].Type == CellType.Wall)
                        adjacentCells.Add(new Rectangle(y * _cellWidth, x * _cellHeight, _cellWidth, _cellHeight));
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
                    var source = new Rectangle(y * _cellWidth, x * _cellHeight, _cellWidth, _cellHeight);
                    Draw(_maze.Grid[x,y].Texture, new Vector2(y * _cellWidth, x * _cellHeight));
                }
            }
        }
        private void Draw(Texture2D texture, Vector2 position)
        {
            spriteBatch.Draw(texture, position, null, Color.White, 0.0f, Vector2.Zero, _cellScale, SpriteEffects.None, 0.0f);
        }

        public Rectangle GetStartRectangle()
        {
            var startPoint = _maze.StartPosition;
            var scale = Math.Min(_cellHeight, _cellWidth) * 0.8f;

            if (!startPoint.HasValue)
                return new Rectangle(0, 0, (int)scale, (int)scale);
            
            var (x, y) = startPoint.Value;
            return new Rectangle(x * _cellWidth + 3, y * _cellHeight + 3, (int)scale, (int)scale);
        }
    }
}
