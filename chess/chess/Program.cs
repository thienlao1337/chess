using System;
using System.Collections.Generic;
using System.Text;

namespace ChessProject
{
    // Клас шахової фігури
    public class Piece
    {
        public string Type { get; private set; } // Тип фігури, наприклад, "Пішак", "Король"
        public string Color { get; private set; } // Колір фігури, наприклад, "Білий" або "Чорний"

        public Piece(string type, string color)
        {
            Type = type;
            Color = color;
        }

        public override string ToString()
        {
            // Скорочене позначення фігури
            string symbol = Type switch
            {
                "Король" => "K",
                "Ферзь" => "Q",
                "Тура" => "R",
                "Кінь" => "N",
                "Слон" => "B",
                "Пішак" => "P",
                _ => "?"
            };
            return Color == "Білий" ? symbol : symbol.ToLower(); // Маленькі літери для чорних фігур
        }
    }

    // Клас шахової дошки
    public class Board
    {
        private Piece[,] squares = new Piece[8, 8]; // Масив 8х8 для зберігання фігур на дошці

        // Ініціалізувати дошку з початковими позиціями фігур
        public void InitializeBoard()
        {
            // Розміщення пішаків
            for (int i = 0; i < 8; i++)
            {
                squares[i, 1] = new Piece("Пішак", "Білий");
                squares[i, 6] = new Piece("Пішак", "Чорний");
            }

            // Розміщення інших фігур
            string[] pieceOrder = { "Тура", "Кінь", "Слон", "Ферзь", "Король", "Слон", "Кінь", "Тура" };
            for (int i = 0; i < 8; i++)
            {
                squares[i, 0] = new Piece(pieceOrder[i], "Білий");
                squares[i, 7] = new Piece(pieceOrder[i], "Чорний");
            }
        }

        // Перетворення шахової нотації в координати масиву
        private (int x, int y) ConvertChessNotation(string notation)
        {
            if (notation.Length == 2 &&
                notation[0] >= 'a' && notation[0] <= 'h' &&
                notation[1] >= '1' && notation[1] <= '8')
            {
                int x = notation[0] - 'a'; // Перетворення букви в колонку (0-7)
                int y = notation[1] - '1'; // Перетворення цифри в рядок (0-7)
                return (x, y);
            }
            throw new ArgumentException("Неправильний формат шахової нотації.");
        }

        public bool MovePiece(string start, string end)
        {
            try
            {
                var (startX, startY) = ConvertChessNotation(start);
                var (endX, endY) = ConvertChessNotation(end);

                Piece piece = squares[startX, startY];
                if (piece == null)
                {
                    Console.WriteLine("На цій позиції немає фігури!");
                    return false;
                }

                squares[endX, endY] = piece;
                squares[startX, startY] = null;
                Console.WriteLine($"Переміщено {piece} з {start} на {end}");
                return true;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public void DisplayBoard()
        {
            Console.WriteLine("  a  b  c  d  e  f  g  h");
            for (int y = 7; y >= 0; y--)
            {
                Console.Write($"{y + 1} ");
                for (int x = 0; x < 8; x++)
                {
                    if (squares[x, y] != null)
                        Console.Write($"{squares[x, y]} ");
                    else
                        Console.Write(". ");
                }
                Console.WriteLine($" {y + 1}");
            }
            Console.WriteLine("  a  b  c  d  e  f  g  h");
        }
    }

    // Клас гри
    public class Game
    {
        private Board board;

        public Game()
        {
            board = new Board();
        }

        public void Initialize()
        {
            Console.WriteLine("Ініціалізація гри...");
            board.InitializeBoard(); // Ініціалізуємо дошку з початковими позиціями
        }

        public void Start()
        {
            Console.WriteLine("Початок гри...");
            board.DisplayBoard();

            while (true)
            {
                Console.WriteLine("Введіть хід (формат: стартова_позиція кінцева_позиція, наприклад 'e2 e4'), або 'вихід' для завершення:");
                string input = Console.ReadLine();

                if (input.ToLower() == "вихід")
                {
                    Console.WriteLine("Гра завершена. Дякуємо за гру!");
                    break;
                }

                string[] parts = input.Split(' ');
                if (parts.Length != 2)
                {
                    Console.WriteLine("Неправильний формат вводу. Спробуйте ще раз.");
                    continue;
                }

                if (!board.MovePiece(parts[0], parts[1]))
                {
                    Console.WriteLine("Хід неможливий. Спробуйте ще раз.");
                }
                board.DisplayBoard();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Встановлення UTF-8 кодування для консолі
            Console.OutputEncoding = Encoding.UTF8;

            // Створення нової гри
            Game chessGame = new Game();

            // Ініціалізація гри
            chessGame.Initialize();

            // Запуск гри
            chessGame.Start();
        }
    }
}
