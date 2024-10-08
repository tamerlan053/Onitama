using Onitama.Core.PlayMatAggregate.Contracts;
using Onitama.Core.SchoolAggregate;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util.Contracts;

namespace Onitama.Core.Util;

/// <summary>
/// Indicates a direction (e.g. North, South, ...)  
/// </summary>
public class Direction
{
    public static Direction North = new Direction(0, 1); //Do not change
    public static Direction East = new Direction(1, 0); //Do not change
    public static Direction South = new Direction(0, -1); //Do not change
    public static Direction West = new Direction(-1, 0); //Do not change

    /// <summary>
    /// Array of the 4 main directions (North, South, East, West)
    /// </summary>
    public static Direction[] MainDirections => new[] { North, South, East, West }; //Do not change

    /// <summary>
    /// Horizontal direction.
    /// -1 = left
    /// 1 = right
    /// 0 = no horizontal direction 
    /// </summary>
    public int XStep { get; } //Do not change

    /// <summary>
    /// Vertical direction.
    /// 1 = up
    /// -1 = down
    /// 0 = no vertical direction 
    /// </summary>
    public int YStep { get; } //Do not change

    /// <summary>
    /// Angle in radians.
    /// </summary>
    public double AngleInRadians => Math.Atan2(XStep, YStep); //Do not change

    /// <summary>
    /// Get the direction perpendicular (loodrecht) to this direction
    /// </summary>

    public Direction PerpendicularDirection
    {
        get
        {
            if (XStep == 0 && YStep == 1)       // Current direction is North
                return East;                    // Perpendicular to North is East
            else if (XStep == 0 && YStep == -1) // Current direction is South
                return West;                    // Perpendicular to South is West
            else if (XStep == 1 && YStep == 0)  // Current direction is East
                return South;                   // Perpendicular to East is South
            else if (XStep == -1 && YStep == 0) // Current direction is West
                return North;                   // Perpendicular to West is North
            else if (XStep == 1 && YStep == 1)  // Current direction is NorthEast
                return South.CombineWith(East);               // Perpendicular to NorthEast is SouthEast
            else if (XStep == -1 && YStep == 1) // Current direction is NorthWest
                return North.CombineWith(East);               // Perpendicular to NorthWest is NorthEast
            else if (XStep == 1 && YStep == -1) // Current direction is SouthEast
                return North.CombineWith(East);               // Perpendicular to SouthEast is NorthEast
            else if (XStep == -1 && YStep == -1)// Current direction is SouthWest
                return North.CombineWith(West);               // Perpendicular to SouthWest is NorthWest
            else
                throw new InvalidOperationException("Unsupported direction.");
        }
    }

    //Do not change
    private Direction(int xStep, int yStep)
    {
        XStep = xStep;
        YStep = yStep;
    }

    //Implicit conversion operator to construct a direction from a string. Do not change
    public static implicit operator Direction(string direction)
    {
        return direction.ToLower() switch
        {
            "north" => North,
            "east" => East,
            "south" => South,
            "west" => West,
            "northeast" => North.CombineWith(East),
            "southeast" => South.CombineWith(East),
            "southwest" => South.CombineWith(West),
            "northwest" => North.CombineWith(West),
            _ => throw new System.ArgumentException("Invalid direction", nameof(direction)),
        };
    }

    /// <summary>
    /// Combines two directions into one (e.g. South + East = SouthEast)
    /// </summary>
    public Direction CombineWith(Direction other)
    {
        int combinedXStep = XStep + other.XStep;
        int combinedYStep = YStep + other.YStep;
        return new Direction(combinedXStep, combinedYStep);
    }

    /// <summary>
    /// Returns the start coordinate on a <see cref="IPlayMat"/> associated with this direction
    /// E.g. for the North direction, the start coordinate is in the middle of the bottom row (0, playMatSize).
    /// E.g. for the NorthEast direction, the start coordinate is at the left of the bottom row (0,0).
    /// E.g. for the SouthWest direction, the start coordinate is at the right of the top row (playMatSize,playMatSize).
    /// </summary>
    /// <param name="playMatSize">The size of the <see cref="IPlayMat"/>. Typically, 5.</param>
    public ICoordinate GetStartCoordinate(int playMatSize)
    {
        int row = 0;
        int column = 0;

        // Determine the starting coordinate based on the direction
        if (this == Direction.North || this == Direction.South)
        {
            row = (this == Direction.North) ? 0 : playMatSize - 1;  // North starts at row 0, South starts at last row
            column = playMatSize / 2;  // Both North and South start at the middle column
        }
        else if (this == Direction.East || this == Direction.West)
        {
            row = playMatSize / 2;  // Both East and West start at the middle row
            column = (this == Direction.East) ? 0 : playMatSize - 1;  // East starts at first column, West starts at last column
        }
        else if (this == Direction.North.CombineWith(Direction.East))
        {
            row = 0;  // NorthEast starts at the top row
            column = 0;  // NorthEast starts at the first column
        }
        else if (this == Direction.South.CombineWith(Direction.West))
        {
            row = playMatSize - 1;  // SouthWest starts at the last row
            column = playMatSize - 1;  // SouthWest starts at the last column
        }
        else if (this == Direction.South.CombineWith(Direction.East))
        {
            row = playMatSize - 1;  // SouthEast starts at the last row
            column = 0;  // SouthEast starts at the first column
        }
        else if (this == Direction.North.CombineWith(Direction.West))
        {
            row = 0;  // NorthWest starts at the top row
            column = playMatSize - 1;  // NorthWest starts at the last column
        }
        else
        {
            // If the direction is unsupported, throw an ArgumentException
            throw new ArgumentException("Unsupported direction.", nameof(this.ToString));
        }

        // Create and return the Coordinate object representing the computed starting coordinate
        return new Coordinate(row, column);
    }

    //Do not change
    public override string ToString()
    {
        switch ((XStep, YStep))
        {
            case (0, 1):
                return "North";
            case (0, -1):
                return "South";
            case (1, 0):
                return "East";
            case (-1, 0):
                return "West";
            case (1, 1):
                return "NorthEast";
            case (-1, 1):
                return "NorthWest";
            case (1, -1):
                return "SouthEast";
            case (-1, -1):
                return "SouthWest";
            default:
                throw new InvalidOperationException("Unsupported direction combination.");
        }

    }

    #region Equality overrides

    //Do not change
    public override bool Equals(object obj)
    {
        return Equals(obj as Direction);
    }

    //Do not change
    protected bool Equals(Direction other)
    {
        if (ReferenceEquals(other, null)) return false;
        return XStep == other.XStep && YStep == other.YStep;
    }

    //Do not change
    public static bool operator ==(Direction a, Direction b)
    {
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null)) return true;
        if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;
        return a.Equals(b);
    }

    //Do not change
    public static bool operator !=(Direction a, Direction b)
    {
        return !(a == b);
    }

    //Do not change
    public override int GetHashCode()
    {
        return HashCode.Combine(XStep, YStep);
    }

    #endregion
}