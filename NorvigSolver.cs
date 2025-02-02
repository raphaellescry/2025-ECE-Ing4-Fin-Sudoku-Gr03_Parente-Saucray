using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Sudoku.Norvig
{ 
public class NorvigSolver
{
    private const string Digits = "123456789";
    private readonly Dictionary<string, string> _grid;
    private readonly Dictionary<string, List<List<string>>> _units;
    private readonly Dictionary<string, HashSet<string>> _peers;
    
    public NorvigSolver()
    {
        _grid = new Dictionary<string, string>();
        _units = new Dictionary<string, List<List<string>>>();
        _peers = new Dictionary<string, HashSet<string>>();
        Initialize();
    }

    private void Initialize()
    {
        var rows = "ABCDEFGHI";
        var cols = Digits;
        var squares = (from r in rows from c in cols select $"{r}{c}").ToList();

        foreach (var s in squares)
        {
            _grid[s] = Digits;
        }

        foreach (var s in squares)
        {
            var unitList = new List<List<string>>
            {
                squares.Where(x => x[0] == s[0]).ToList(), // Ligne
                squares.Where(x => x[1] == s[1]).ToList(), // Colonne
                squares.Where(x => (x[0] - 'A') / 3 == (s[0] - 'A') / 3 && (x[1] - '1') / 3 == (s[1] - '1') / 3).ToList() // Bloc 3x3
            };

            _units[s] = unitList;
            _peers[s] = new HashSet<string>(unitList.SelectMany(u => u).Where(x => x != s));
        }
    }
    public static string ConvertBoardToString(int[,] board)
    {
        var rows = board.GetLength(0);
        var sb = new StringBuilder();
        
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                sb.Append(board[i, j]);
                if (j < board.GetLength(1) - 1) sb.Append(" ");
            }
            sb.AppendLine();
        }

        return sb.ToString().Trim();
    }
    public bool Solve(string puzzle)
    {
        if (!ParseGrid(puzzle)) return false;
        return Search();
    }

    private bool ParseGrid(string puzzle)
    {
        var squares = _grid.Keys.ToList();
        for (int i = 0; i < squares.Count; i++)
        {
            if (Digits.Contains(puzzle[i]))
            {
                if (!Assign(squares[i], puzzle[i].ToString()))
                    return false;
            }
        }
        return true;
    }

    private bool Assign(string square, string value)
    {
        var otherValues = _grid[square].Replace(value, "");
        foreach (var v in otherValues)
        {
            if (!Eliminate(square, v.ToString())) return false;
        }
        return true;
    }

    private bool Eliminate(string square, string value)
    {
        if (!_grid[square].Contains(value)) return true;

        _grid[square] = _grid[square].Replace(value, "");

        if (_grid[square].Length == 0)
            return false;
        if (_grid[square].Length == 1)
        {
            var singleValue = _grid[square];
            foreach (var peer in _peers[square])
            {
                if (!Eliminate(peer, singleValue)) return false;
            }
        }

        foreach (var unit in _units[square])
{
    var places = unit.Where(s => _grid[s].Contains(value.ToString())).ToList();
    
    if (places.Count == 0)
        return false;

    if (places.Count == 1)
    {
        if (!Assign(places[0], value.ToString()))  // Conversion de 'value' en string ici
            return false;
    }
}
return true;

    }

    private bool Search()
    {
        if (_grid.Values.All(v => v.Length == 1)) return true;

        var square = _grid.Where(kv => kv.Value.Length > 1).OrderBy(kv => kv.Value.Length).First().Key;
        foreach (var value in _grid[square])
        {
            var backup = new Dictionary<string, string>(_grid);
            if (Assign(square, value.ToString()) && Search())
                return true;
            _grid.Clear();
            foreach (var kvp in backup) _grid[kvp.Key] = kvp.Value;
        }
        return false;
    }
    public int[,] GetSolvedBoard()
{
    var solvedBoard = new int[9, 9];
    var squares = _grid.Keys.ToList();

    for (int i = 0; i < squares.Count; i++)
    {
        var value = _grid[squares[i]];
        if (value.Length == 1)
        {
            int row = squares[i][0] - 'A';
            int col = int.Parse(squares[i][1].ToString()) - 1;
            solvedBoard[row, col] = int.Parse(value);
        }
    }

    return solvedBoard;
}

}

}
    
