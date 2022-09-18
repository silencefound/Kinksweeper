using System;
using System.Collections.Generic;
using System.Linq;

namespace Kinksweeper.Models;

public class InternalMineField
{
    private Position[,] _minefield;
    
    public InternalMineField(int rows, int cols, int mines)
    {
        if (mines > rows * cols) { mines = rows * cols; }

        _minefield = new Position[rows, cols];
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                _minefield[i, j] = new Position();
            }
        }

        var rng = new Random();
        var values = Enumerable.Range(0, rows * cols).OrderBy(_ => rng.Next()).Take(mines).ToArray();
        foreach (var i in values)
        {
            var cur_row = i / rows;
            var cur_col = i % cols;
            _minefield[cur_row, cur_col].hasMine = true;
            _minefield[cur_row, cur_col].minesAround = -1;

            foreach (var tuple in GetSurroundingsInField(cur_row, cur_col))
            {
                IncreaseField(tuple.Item1, tuple.Item2);
            }
        }
    }

    private void IncreaseField(int row, int col)
    {
        if (_minefield[row, col].hasMine) { return; }
        _minefield[row, col].minesAround++;
    }

    public Position this[int i, int j] => _minefield[i, j];

    private List<Tuple<int, int>> GetSurroundingsInField(int row, int col)
    {
        return new List<Tuple<int, int>>
        {
            new(row - 1, col - 1),
            new(row + 0, col - 1),
            new(row + 1, col - 1),
            new(row - 1, col + 0),
            new(row + 1, col + 0),
            new(row - 1, col + 1),
            new(row + 0, col + 1),
            new(row + 1, col + 1),
        }
            .Where(tuple => !PointOutOfBorders(tuple.Item1, tuple.Item2))
            .ToList();
    }

    private bool PointOutOfBorders(int row, int col)
    {
        return row < 0 || row >= _minefield.GetLength(0) || 
               col < 0 || col >= _minefield.GetLength(1);
    }
    public HashSet<Tuple<int, int>> ReactToOpenField(int row, int column)
    {
        var result = new HashSet<Tuple<int, int>> { new(row, column) };
        if (_minefield[row, column].minesAround != 0) { return result; }

        var stack = new Stack<Tuple<int, int>>();
        GetSurroundingsInField(row, column).ForEach(tuple => stack.Push(tuple));
        
        while (stack.Count > 0)
        {
            var point = stack.Pop();
            if (result.Contains(point)) { continue; }
            
            result.Add(point);
            if (_minefield[point.Item1, point.Item2].minesAround != 0) { continue; }
            
            GetSurroundingsInField(point.Item1, point.Item2).ForEach(tuple => stack.Push(tuple));
        }

        return result;
    }

    public bool Finished()
    {
        return _minefield.Cast<Position?>().All(position => position!.state == PositionState.OPEN || position.hasMine);
    }
}