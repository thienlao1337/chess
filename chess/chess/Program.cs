using System;
using System.Text;

namespace ChessProject
{
    public enum PieceType { King, Queen, Rook, Knight, Bishop, Pawn }

    public enum PieceColor { White, Black }

    // A single chess piece
    public class Piece
    {
        public PieceType Type { get; }
        public PieceColor Color { get; }

        public Piece(PieceType type, PieceColor color)
        {
            Type = type;
            Color = color;
        }

        public override string ToString()
        {
            // Short notation: uppercase for White, lowercase for Black
            string symbol = Type switch
            {
                PieceType.King => "K",
                PieceType.Queen => "Q",
                PieceType.Rook => "R",
                PieceType.Knight => "N",
                PieceType.Bishop => "B",
                PieceType.Pawn => "P",
                _ => "?"
            };
            return Color == PieceColor.White ? symbol : symbol.ToLower();
        }
    }

    // The chess board
    public class Board
    {
        private readonly Piece?[,] squares = new Piece?[8, 8]; // 8x8 grid holding the pieces

        // Place all pieces in their starting positions
        public void InitializeBoard()
        {
            for (int i = 0; i < 8; i++)
            {
                squares[i, 1] = new Piece(PieceType.Pawn, PieceColor.White);
                squares[i, 6] = new Piece(PieceType.Pawn, PieceColor.Black);
            }

            PieceType[] backRank =
            {
                PieceType.Rook, PieceType.Knight, PieceType.Bishop, PieceType.Queen,
                PieceType.King, PieceType.Bishop, PieceType.Knight, PieceType.Rook
            };
            for (int i = 0; i < 8; i++)
            {
                squares[i, 0] = new Piece(backRank[i], PieceColor.White);
                squares[i, 7] = new Piece(backRank[i], PieceColor.Black);
            }
        }

        // Convert chess notation (e.g. "e2") to array coordinates
        private static (int x, int y) ConvertChessNotation(string notation)
        {
            if (notation.Length == 2 &&
                notation[0] >= 'a' && notation[0] <= 'h' &&
                notation[1] >= '1' && notation[1] <= '8')
            {
                int x = notation[0] - 'a'; // file a-h -> column 0-7
                int y = notation[1] - '1'; // rank 1-8 -> row 0-7
                return (x, y);
            }
            throw new ArgumentException($"Invalid square '{notation}'. Use a file a-h and a rank 1-8, e.g. e2.");
        }

        // Move a piece of the given color. Returns true if the move was made.
        public bool MovePiece(string start, string end, PieceColor currentPlayer)
        {
            try
            {
                var (startX, startY) = ConvertChessNotation(start);
                var (endX, endY) = ConvertChessNotation(end);

                Piece? piece = squares[startX, startY];
                if (piece == null)
                {
                    Console.WriteLine("There is no piece on that square.");
                    return false;
                }

                if (piece.Color != currentPlayer)
                {
                    Console.WriteLine($"It's {currentPlayer}'s turn — you can't move the opponent's piece.");
                    return false;
                }

                if (startX == endX && startY == endY)
                {
                    Console.WriteLine("The start and end squares are the same.");
                    return false;
                }

                Piece? target = squares[endX, endY];
                if (target != null && target.Color == piece.Color)
                {
                    Console.WriteLine("You can't capture your own piece.");
                    return false;
                }

                squares[endX, endY] = piece;
                squares[startX, startY] = null;

                string capture = target != null ? $", capturing {target}" : "";
                Console.WriteLine($"Moved {piece} from {start} to {end}{capture}");
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
            Console.WriteLine("  a b c d e f g h");
            for (int y = 7; y >= 0; y--)
            {
                Console.Write($"{y + 1} ");
                for (int x = 0; x < 8; x++)
                {
                    Console.Write(squares[x, y] != null ? $"{squares[x, y]} " : ". ");
                }
                Console.WriteLine($"{y + 1}");
            }
            Console.WriteLine("  a b c d e f g h");
        }
    }

    // Game loop: reads moves from the console and alternates turns
    public class Game
    {
        private readonly Board board = new Board();
        private PieceColor currentPlayer = PieceColor.White;

        public void Initialize()
        {
            Console.WriteLine("Setting up the board...");
            board.InitializeBoard();
        }

        public void Start()
        {
            Console.WriteLine("Game started.");
            board.DisplayBoard();

            while (true)
            {
                Console.WriteLine($"{currentPlayer} to move (e.g. 'e2 e4'), or 'exit' to quit:");
                string? input = Console.ReadLine();

                // ReadLine returns null when input is closed (e.g. Ctrl+Z / Ctrl+D)
                if (input == null || input.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Game over. Thanks for playing!");
                    break;
                }

                string[] parts = input.Trim().ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                {
                    Console.WriteLine("Invalid input. Enter two squares separated by a space, e.g. 'e2 e4'.");
                    continue;
                }

                if (board.MovePiece(parts[0], parts[1], currentPlayer))
                {
                    currentPlayer = currentPlayer == PieceColor.White ? PieceColor.Black : PieceColor.White;
                }
                else
                {
                    Console.WriteLine("Move not made. Try again.");
                }
                board.DisplayBoard();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Game chessGame = new Game();
            chessGame.Initialize();
            chessGame.Start();
        }
    }
}
