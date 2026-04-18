using System;
using System.Collections.Generic;

class Board
{
    private char[,] grid;

    public Board()
    {
        grid = new char[3, 3];
        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 3; c++)
            {
                grid[r, c] = ' ';
            }
        }
    }

    public void Display(IEnumerable<(int row, int col)>? highlight = null)
    {
        var highlightSet = highlight != null ? new HashSet<(int, int)>(highlight) : new HashSet<(int, int)>();
        Console.WriteLine("  1 2 3");
        for (int r = 0; r < 3; r++)
        {
            Console.Write((r + 1) + " ");
            for (int c = 0; c < 3; c++)
            {
                if (highlightSet.Contains((r, c)))
                {
                    Console.Write('-');
                }
                else
                {
                    Console.Write(grid[r, c]);
                }
                if (c < 2) Console.Write("|");
            }
            Console.WriteLine();
            if (r < 2) Console.WriteLine("  -----");
        }
    }

    public bool MakeMove(int row, int col, char symbol)
    {
        if (grid[row, col] == ' ')
        {
            grid[row, col] = symbol;
            return true;
        }
        return false;
    }

    public bool IsFull()
    {
        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 3; c++)
            {
                if (grid[r, c] == ' ') return false;
            }
        }
        return true;
    }

    public List<(int row, int col)>? GetWinningLine(char symbol)
    {
        for (int r = 0; r < 3; r++)
        {
            if (grid[r, 0] == symbol && grid[r, 1] == symbol && grid[r, 2] == symbol)
            {
                return new List<(int, int)> { (r, 0), (r, 1), (r, 2) };
            }
        }
        for (int c = 0; c < 3; c++)
        {
            if (grid[0, c] == symbol && grid[1, c] == symbol && grid[2, c] == symbol)
            {
                return new List<(int, int)> { (0, c), (1, c), (2, c) };
            }
        }
        if (grid[0, 0] == symbol && grid[1, 1] == symbol && grid[2, 2] == symbol)
        {
            return new List<(int, int)> { (0, 0), (1, 1), (2, 2) };
        }
        if (grid[0, 2] == symbol && grid[1, 1] == symbol && grid[2, 0] == symbol)
        {
            return new List<(int, int)> { (0, 2), (1, 1), (2, 0) };
        }
        return null;
    }

    public bool CheckWinner(char symbol)
    {
        return GetWinningLine(symbol) != null;
    }
}

class Player
{
    public string Name { get; }
    public char Symbol { get; }

    public Player(string name, char symbol)
    {
        Name = name;
        Symbol = symbol;
    }

    public (int, int) GetMove()
    {
        while (true)
        {
            Console.Write($"{Name} ({Symbol}), podaj ruch jako wiersz i kolumna (np. 2 3): ");
            string? line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                Console.WriteLine("Podaj dwie liczby.");
                continue;
            }

            string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                Console.WriteLine("Podaj dwie liczby oddzielone spacją.");
                continue;
            }

            if (!int.TryParse(parts[0], out int row) || !int.TryParse(parts[1], out int col))
            {
                Console.WriteLine("Wprowadź liczby.");
                continue;
            }

            if (row < 1 || row > 3 || col < 1 || col > 3)
            {
                Console.WriteLine("Wiersz i kolumna muszą być z zakresu 1-3.");
                continue;
            }

            return (row - 1, col - 1);
        }
    }
}

class Game
{
    private Board board;
    private readonly Player player1;
    private readonly Player player2;
    private Player currentPlayer;

    public Game()
    {
        board = new Board();
        player1 = new Player("Gracz 1", 'X');
        player2 = new Player("Gracz 2", 'O');
        currentPlayer = player1;
    }

    private void Reset()
    {
        board = new Board();
        currentPlayer = player1;
    }

    private bool AskReplay()
    {
        while (true)
        {
            Console.Write("Czy chcesz rozpocząć nową grę? (t/n): ");
            string? answer = Console.ReadLine()?.Trim().ToLower();
            if (answer == "t" || answer == "tak") return true;
            if (answer == "n" || answer == "nie") return false;
            Console.WriteLine("Wpisz 't' lub 'n'.");
        }
    }

    private void PlayRound()
    {
        while (true)
        {
            board.Display();
            if (board.IsFull())
            {
                Console.WriteLine("Remis!");
                break;
            }

            var (row, col) = currentPlayer.GetMove();
            if (!board.MakeMove(row, col, currentPlayer.Symbol))
            {
                Console.WriteLine("To pole jest zajęte. Spróbuj jeszcze raz.");
                continue;
            }

            var winningLine = board.GetWinningLine(currentPlayer.Symbol);
            if (winningLine != null)
            {
                board.Display(winningLine);
                Console.WriteLine($"{currentPlayer.Name} wygrał!");
                break;
            }

            if (board.IsFull())
            {
                board.Display();
                Console.WriteLine("Remis!");
                break;
            }

            SwitchPlayer();
        }
    }

    private void SwitchPlayer()
    {
        currentPlayer = currentPlayer == player1 ? player2 : player1;
    }

    public void Run()
    {
        while (true)
        {
            PlayRound();
            if (!AskReplay())
            {
                break;
            }
            Reset();
        }
    }
}

class Program
{
    static void Main()
    {
        var game = new Game();
        game.Run();
    }
}
