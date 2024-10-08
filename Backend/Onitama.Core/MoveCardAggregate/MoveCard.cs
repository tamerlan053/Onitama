using System.Drawing;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.SchoolAggregate;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util;
using Onitama.Core.Util.Contracts;

namespace Onitama.Core.MoveCardAggregate;

/// <inheritdoc cref="IMoveCard"/>
internal class MoveCard : IMoveCard
{
    public string Name { get; }

    public MoveCardGridCellType[,] Grid { get; }

    public Color StampColor { get; }

    public MoveCard(string name, MoveCardGridCellType[,] grid, Color stampColor)
    {
        Name = name;
        Grid = grid;
        StampColor = stampColor;
    }

    //Do not change this method, it makes sure that two MoveCard instances are equal if their names are equal
    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        return obj is IMoveCard other && Equals(other);
    }

    //Do not change this method
    protected bool Equals(IMoveCard other)
    {
        return Name == other.Name;
    }

    //Do not change this method
    public override int GetHashCode()
    {
        return (Name != null ? Name.GetHashCode() : 0);
    }

    public IReadOnlyList<ICoordinate> GetPossibleTargetCoordinates(ICoordinate startCoordinate, Direction playDirection, int matSize)
    {
        List<Coordinate> possibleTargets = new List<Coordinate>();

        int centerRow = 2;
        int centerColumn = 2;

        for (int indexRow = 0; indexRow < 5; indexRow++)
        {
            for (int indexColumn = 0; indexColumn < 5; indexColumn++)
            {
                if (Grid[indexRow, indexColumn] == MoveCardGridCellType.Target)
                {
                    int offsetRow = indexRow - centerRow;
                    int offsetColumn = indexColumn - centerColumn;

                    int targetRow = startCoordinate.Row + playDirection.YStep * offsetRow - playDirection.XStep * offsetColumn;
                    int targetColumn = startCoordinate.Column + playDirection.XStep * offsetRow + playDirection.YStep * offsetColumn;

                    Coordinate possibleCoordinate = new Coordinate(targetRow, targetColumn);

                    if (!possibleCoordinate.IsOutOfBounds(matSize))
                    {
                        possibleTargets.Add(possibleCoordinate);
                    }
                }
            }
        }
        return possibleTargets;
    }
}
