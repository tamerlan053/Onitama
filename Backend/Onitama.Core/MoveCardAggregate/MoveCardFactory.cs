using System.Drawing;
using Onitama.Core.MoveCardAggregate.Contracts;

namespace Onitama.Core.MoveCardAggregate;

/// <inheritdoc cref="IMoveCardFactory"/>
internal class MoveCardFactory : IMoveCardFactory
{
    private readonly Random _random;

    public MoveCardFactory()
    {
        _random = new Random();
    }

    public IMoveCard Create(string name, MoveCardGridCellType[,] grid, Color[] possibleStampColors)
    {
        Color randomColor = possibleStampColors[_random.Next(possibleStampColors.Length)];

        return new MoveCard(name, grid, randomColor);
    }
}