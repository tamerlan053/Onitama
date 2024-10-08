using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util;
using Onitama.Core.Util.Contracts;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;

namespace Onitama.Core.SchoolAggregate;

/// <inheritdoc cref="ISchool"/>
internal class School : ISchool
{
    /// <summary>
    /// Creates a school that is a copy of another school.
    /// </summary>
    /// <remarks>
    /// This is an EXTRA. Not needed to implement the minimal requirements.
    /// To make the mini-max algorithm for an AI game play strategy work, this constructor should be implemented.
    /// </remarks>
    public IPawn Master { get; }

    public IPawn[] Students { get; }

    public IPawn[] AllPawns { get; }

    public ICoordinate TempleArchPosition { get ; set ; }

    public School(ISchool otherSchool)
    {
        Master = otherSchool.Master;
        AllPawns = otherSchool.AllPawns;
        Students = otherSchool.Students;
        TempleArchPosition = otherSchool.TempleArchPosition;
    }

    public School(IPawn master, IPawn[] students, IPawn[] allPawns, ICoordinate templeArchPosition)
    {
        Master = master;
        Students = students;
        AllPawns = allPawns;
        TempleArchPosition = templeArchPosition;
    }

    public IPawn GetPawn(Guid id)
    {
        // Search for the pawn with the specified ID in the AllPawns collection
        return AllPawns.FirstOrDefault(pawn => pawn.Id == id);
    }
}