using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TetrisConsole
{
    // A single cell that makes up a shape
    public class Block
    {
        public int X { get; set; } // column on the board
        public int Y { get; set; } // row on the board

        public Block(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    // A falling shape made of several blocks
    public class Shape
    {
        public List<Block> Blocks { get; }

        public Shape(List<Block> blocks)
        {
            Blocks = blocks;
        }

        public void Move(int dx, int dy)
        {
            foreach (var block in Blocks)
            {
                block.X += dx;
                block.Y += dy;
            }
        }

        public void MoveDown() => Move(0, 1);
        public void MoveUp() => Move(0, -1);
        public void MoveLeft() => Move(-1, 0);
        public void MoveRight() => Move(1, 0);
    }

    // The playing field and game rules
    public class GameBoard
    {
        private const int Width = 10;
        private const int Height = 20;
        private readonly Block?[,] grid = new Block?[Width, Height];
        private readonly Random random = new Random();
        private Shape currentShape = null!;

        public int Score { get; private set; }
        public bool IsGameOver { get; private set; }

        // Spawn a new shape at the top; the game ends if it doesn't fit
        public void SpawnNewShape()
        {
            currentShape = GetRandomShape();
            if (!IsPositionValid(currentShape.Blocks))
            {
                IsGameOver = true;
            }
        }

        private Shape GetRandomShape()
        {
            int type = random.Next(0, 3);
            List<Block> blocks = type switch
            {
                0 => new List<Block> { new Block(4, 0), new Block(5, 0), new Block(4, 1), new Block(5, 1) }, // square
                1 => new List<Block> { new Block(5, 0), new Block(4, 0), new Block(6, 0), new Block(5, 1) }, // T-shape
                _ => new List<Block> { new Block(3, 0), new Block(4, 0), new Block(5, 0), new Block(6, 0) }  // line
            };
            return new Shape(blocks);
        }

        // One gravity tick: move the shape down, or lock it if it has landed
        public void UpdateGame()
        {
            currentShape.MoveDown();
            if (!IsPositionValid(currentShape.Blocks))
            {
                currentShape.MoveUp(); // undo the move that collided
                AddShapeToGrid();
                ClearCompletedRows();
                SpawnNewShape();
            }
        }

        public void TryMoveLeft()
        {
            currentShape.MoveLeft();
            if (!IsPositionValid(currentShape.Blocks)) currentShape.MoveRight();
        }

        public void TryMoveRight()
        {
            currentShape.MoveRight();
            if (!IsPositionValid(currentShape.Blocks)) currentShape.MoveLeft();
        }

        private void AddShapeToGrid()
        {
            foreach (var block in currentShape.Blocks)
            {
                grid[block.X, block.Y] = block;
            }
        }

        private bool IsPositionValid(List<Block> blocks)
        {
            foreach (var block in blocks)
            {
                if (block.X < 0 || block.X >= Width || block.Y >= Height ||
                    (block.Y >= 0 && grid[block.X, block.Y] != null))
                {
                    return false;
                }
            }
            return true;
        }

        // Remove full rows and shift everything above them down
        private void ClearCompletedRows()
        {
            for (int y = Height - 1; y >= 0; y--)
            {
                bool rowComplete = true;
                for (int x = 0; x < Width; x++)
                {
                    if (grid[x, y] == null)
                    {
                        rowComplete = false;
                        break;
                    }
                }

                if (rowComplete)
                {
                    for (int yy = y; yy > 0; yy--)
                    {
                        for (int xx = 0; xx < Width; xx++)
                        {
                            grid[xx, yy] = grid[xx, yy - 1];
                        }
                    }
                    for (int xx = 0; xx < Width; xx++)
                    {
                        grid[xx, 0] = null;
                    }

                    Score += 100;
                    y++; // re-check the same row after shifting
                }
            }
        }

        public void Display()
        {
            var sb = new StringBuilder();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    bool filled = grid[x, y] != null || currentShape.Blocks.Any(b => b.X == x && b.Y == y);
                    sb.Append(filled ? "■ " : ". ");
                }
                sb.AppendLine();
            }
            sb.AppendLine($"Score: {Score}");
            sb.AppendLine("←/→ move   ↓ drop faster   Q quit");

            Console.SetCursorPosition(0, 0); // redraw in place instead of clearing to avoid flicker
            Console.Write(sb.ToString());
        }
    }

    class Program
    {
        private const int TickMs = 500; // time between gravity steps
        private const int FrameMs = 50; // input polling interval

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;
            Console.Clear();

            var board = new GameBoard();
            board.SpawnNewShape();
            int elapsed = 0;

            while (!board.IsGameOver)
            {
                while (Console.KeyAvailable)
                {
                    switch (Console.ReadKey(true).Key)
                    {
                        case ConsoleKey.LeftArrow: board.TryMoveLeft(); break;
                        case ConsoleKey.RightArrow: board.TryMoveRight(); break;
                        case ConsoleKey.DownArrow: board.UpdateGame(); break;
                        case ConsoleKey.Q:
                            Console.CursorVisible = true;
                            return;
                    }
                }

                elapsed += FrameMs;
                if (elapsed >= TickMs)
                {
                    board.UpdateGame();
                    elapsed = 0;
                }

                if (!board.IsGameOver) board.Display();
                Thread.Sleep(FrameMs);
            }

            Console.CursorVisible = true;
            Console.WriteLine($"Game over! Final score: {board.Score}");
        }
    }
}
