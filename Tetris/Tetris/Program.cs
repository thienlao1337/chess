using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TetrisConsole
{
    // Клас блоку, який формує ігрові фігури
    public class Block
    {
        public int X { get; set; } // X-координата блоку на полі
        public int Y { get; set; } // Y-координата блоку на полі

        public Block(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    // Клас фігури
    public class Shape
    {
        public List<Block> Blocks { get; private set; } // Список блоків, які утворюють фігуру

        public Shape(List<Block> blocks)
        {
            Blocks = blocks;
        }

        // Оновлення позиції фігури (переміщення вниз на 1)
        public void MoveDown()
        {
            foreach (var block in Blocks)
            {
                block.Y += 1;
            }
        }

        // Переміщення вліво
        public void MoveLeft()
        {
            foreach (var block in Blocks)
            {
                block.X -= 1;
            }
        }

        // Переміщення вправо
        public void MoveRight()
        {
            foreach (var block in Blocks)
            {
                block.X += 1;
            }
        }
    }

    // Клас ігрового поля
    public class GameBoard
    {
        private const int Width = 10;
        private const int Height = 20;
        private Block[,] grid = new Block[Width, Height];
        private Shape currentShape;
        private Random random = new Random();

        // Ініціалізувати нову фігуру
        public void SpawnNewShape()
        {
            currentShape = GetRandomShape();
            if (!IsPositionValid(currentShape.Blocks))
            {
                Console.WriteLine("Гра завершена!");
                Environment.Exit(0);
            }
        }

        // Отримати випадкову фігуру
        private Shape GetRandomShape()
        {
            int type = random.Next(0, 3);
            List<Block> blocks = type switch
            {
                0 => new List<Block> { new Block(4, 0), new Block(5, 0), new Block(4, 1), new Block(5, 1) }, // Квадрат
                1 => new List<Block> { new Block(5, 0), new Block(4, 0), new Block(6, 0), new Block(5, 1) }, // Т-подібна
                _ => new List<Block> { new Block(4, 0), new Block(5, 0), new Block(6, 0), new Block(7, 0) }  // Лінія
            };
            return new Shape(blocks);
        }

        // Оновити стан гри (переміщення поточної фігури вниз)
        public void UpdateGame()
        {
            currentShape.MoveDown();
            if (!IsPositionValid(currentShape.Blocks))
            {
                currentShape.MoveDown();
                AddShapeToGrid();
                ClearCompletedRows();
                SpawnNewShape();
            }
        }

        // Додати фігуру до сітки
        private void AddShapeToGrid()
        {
            foreach (var block in currentShape.Blocks)
            {
                grid[block.X, block.Y] = block;
            }
        }

        // Перевірка на валідність позиції
        private bool IsPositionValid(List<Block> blocks)
        {
            foreach (var block in blocks)
            {
                if (block.X < 0 || block.X >= Width || block.Y >= Height || (block.Y >= 0 && grid[block.X, block.Y] != null))
                {
                    return false;
                }
            }
            return true;
        }

        // Очищення завершених рядків
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
                    for (int x = 0; x < Width; x++)
                    {
                        grid[x, y] = null;
                    }

                    // Зсув усіх рядків вниз
                    for (int yy = y; yy > 0; yy--)
                    {
                        for (int xx = 0; xx < Width; xx++)
                        {
                            grid[xx, yy] = grid[xx, yy - 1];
                        }
                    }
                    y++; // Перевірити той самий рядок ще раз
                }
            }
        }

        // Відображення ігрового поля
        public void Display()
        {
            Console.Clear();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (grid[x, y] != null || currentShape.Blocks.Any(b => b.X == x && b.Y == y))
                        Console.Write("■ ");
                    else
                        Console.Write(". ");
                }
                Console.WriteLine();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            GameBoard board = new GameBoard();
            board.SpawnNewShape();

            while (true)
            {
                board.UpdateGame();
                board.Display();
                System.Threading.Thread.Sleep(500); // Затримка для уповільнення падіння фігур
            }
        }
    }
}
