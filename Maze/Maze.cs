using MazeGame.Services;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using static Java.Interop.JniEnvironment;
using Color = Microsoft.Xna.Framework.Color;

namespace MazeGame.Maze
{
    public enum CellType
    {
        Wall,
        Path,
        Start,
        End,
        Trap
    }
    public record MazeElement(CellType Type, Texture2D Texture, Color Color);
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
            var mazeTexture2D = contentManager.Load<Texture2D>("Images/maze_path");
            var emptyTexture2D = contentManager.Load<Texture2D>("Images/maze_path");
            var endTexture2D = contentManager.Load<Texture2D>("Images/maze_path");
            var trapdoorTexture2D = contentManager.Load<Texture2D>("Images/maze_trapdoor");

            var wall  = new MazeElement(CellType.Wall, mazeTexture2D, ColorService.WallColor);
            var empty  = new MazeElement(CellType.Path, emptyTexture2D, ColorService.PathColor);

            //var files = Android.App.Application.Context.Assets.List("MazeSources/size" + Game1.mazeSize + "/");
            //var rand = new Random();
            //using var stream = Android.App.Application.Context.Assets.Open("MazeSources/size" + Game1.mazeSize + "/" + files[rand.Next(files.Length)]);
            using var stream = Android.App.Application.Context.Assets.Open(path);
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
                        'S' => new MazeElement(CellType.Start, emptyTexture2D, ColorService.StartColor),
                        'E' => new MazeElement(CellType.End, endTexture2D, ColorService.EndColor),
                        'X' => new MazeElement(CellType.Trap, trapdoorTexture2D, ColorService.TrapColor),
                        _ => throw new System.Exception("Invalid character in maze file.")
                    };
                }
            }
            return new Maze(grid);
        }

    }
}
