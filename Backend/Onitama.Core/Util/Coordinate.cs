using System.Numerics;
using Onitama.Core.PlayMatAggregate;
using Onitama.Core.PlayMatAggregate.Contracts;
using Onitama.Core.Util.Contracts;

namespace Onitama.Core.Util;

/// <inheritdoc cref="ICoordinate"/>
internal class Coordinate : ICoordinate
{
    public int Row { get; }
    public int Column { get; }

    public Coordinate(int row, int column)
    {
        Row = row;
        Column = column;
    }
    
    //Do not change this method
    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        return obj is ICoordinate other && Equals(other);
    }

    //Do not change this method
    protected bool Equals(ICoordinate other)
    {
        return Row == other.Row && Column == other.Column;
    }

    //Do not change this method
    public override int GetHashCode()
    {
        return HashCode.Combine(Row, Column);
    }

    //Do not change this method
    public override string ToString()
    {
        return $"({Row}, {Column})";
    }

    public bool IsOutOfBounds(int playMatSize)
    {
        return Row < 0 || Row >= playMatSize || Column < 0 || Column >= playMatSize;
    }

    public ICoordinate GetNeighbor(Direction direction)
    {
        int newRow = Row + direction.YStep;
        int newColumn = Column + direction.XStep;
        return new Coordinate(newRow, newColumn);
    }

    public ICoordinate RotateTowards(Direction direction)
    {
        int newRow, newColumn;

        if (direction == Direction.North)
        {
            newRow = Row;
            newColumn = Column;
        }
        else if (direction == Direction.East)   
        {
            newRow = -Column;
            newColumn = Row;
        }
        else if (direction == Direction.South)
        {
            newRow = -Row;
            newColumn = -Column;
        }
        else if (direction == Direction.West)
        {
            newRow = Column;
            newColumn = -Row;
        }
        else
        {
            throw new ArgumentException("Invalid direction!!!"); // gooi argument exception zoals in Direction  
        }

        return new Coordinate(newRow, newColumn);
    }

    public int GetDistanceTo(ICoordinate other)
    {
        int rowDifference = other.Row - Row;
        int columnDifference = other.Column - Column;
        return Math.Max(Math.Abs(rowDifference), Math.Abs(columnDifference));
    }
}