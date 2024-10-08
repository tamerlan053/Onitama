using Onitama.Core.Util.Contracts;

namespace Onitama.Core.Util;

internal class CoordinateFactory : ICoordinateFactory
{
    public ICoordinate Create(int row, int column)
    {
        // Returning an instance of the Coordinate class
        return new Coordinate(row, column);
    }
}