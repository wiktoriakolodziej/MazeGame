using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using static Java.Interop.JniEnvironment;

namespace MazeGame.Maze
{
    public enum CellType
    {
        Wall,
        Path,
        Start,
        End
    }
    public record MazeElement(CellType Type, Texture2D Texture);
    public class Maze
    {
        public int Rows { get; }
        public int Columns { get; }
        public (int, int)? StartPosition { get; }
        public (int, int)? EndPosition { get;  }
        public MazeElement[,] Grid { get; }

        public Maze(MazeElement[,] grid)
        {
            Grid = grid;
            Rows = grid.GetLength(0);
            Columns = grid.GetLength(1);
            StartPosition = FindCell(CellType.Start);
            EndPosition = FindCell(CellType.End);
        }

        private (int, int)? FindCell(CellType type)
        {
            for(var x = 0; x < Rows; x++)
            {
                for(var y = 0; y < Columns; y++)
                {
                    if(Grid[x, y].Type == type)
                        return (x, y);
                }
            }
            return null;
        }

        public static Maze CreateFromFile(string path, ContentManager contentManager)
        {
            var mazeTexture2D = contentManager.Load<Texture2D>("Images/maze_wall");
            var emptyTexture2D = contentManager.Load<Texture2D>("Images/maze_path");
           
            var wall  = new MazeElement(CellType.Wall, mazeTexture2D);
            var empty  = new MazeElement(CellType.Path, emptyTexture2D);

            using var stream = Android.App.Application.Context.Assets.Open("maze.txt");
            using var reader = new StreamReader(stream);
            string[] lines = reader.ReadToEnd().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var grid = new MazeElement[lines.Length, lines[0].Length];

            for (var x = 0; x < lines.Length; x++)
            {
                for(var y = 0; y < lines[x].Length; y++)
                {
                    grid[x, y] = lines[x][y] switch
                    {
                        '#' => wall,
                        '.' => empty,
                        'S' => new MazeElement(CellType.Start, emptyTexture2D),
                        'E' => new MazeElement(CellType.End, emptyTexture2D),
                        _ => throw new System.Exception("Invalid character in maze file.")
                    };
                }
            }
            return new Maze(grid);
        }

    }
}
