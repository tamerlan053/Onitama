using Onitama.Core.GameAggregate;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate;
using Onitama.Core.PlayMatAggregate.Contracts;
using Onitama.Core.SchoolAggregate;
using Onitama.Core.SchoolAggregate.Contracts;
using System.Data.Common;
using Onitama.Core.Util;
using Onitama.Core.Util.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;

namespace Onitama.Core.PlayMatAggregate;

/// <inheritdoc cref="IPlayMat"/>
internal class PlayMat : IPlayMat
{
    /// <summary>
    /// Creates a play mat that is a copy of another play mat
    /// </summary>
    /// <param name="otherPlayMat">The play mat to copy</param>
    /// <param name="copiedPlayers">
    /// Copies of the players (with their school)
    /// that should be used to position pawn on the copy of the <paramref name="otherPlayMat"/>.</param>
    /// <remarks>
    /// This is an EXTRA. Not needed to implement the minimal requirements.
    /// To make the mini-max algorithm for an AI game play strategy work, this constructor should be implemented.
    /// </remarks>
    ///

    public PlayMat(IPlayMat otherPlayMat, IPlayer[] copiedPlayers)
    {
        throw new NotImplementedException();
        // DONT TOUCH THIS 
    }

    public PlayMat(IPawn[,] grid, int size)
    {
        Grid = grid;
        Size = size;
    }

    public IPawn[,] Grid { get; }

    public int Size { get; }

    public void ExecuteMove(IMove move, out IPawn capturedPawn)
    {
        if (move == null)
        {
            throw new ArgumentNullException(nameof(move));
        }

        IReadOnlyList<IMove> possibleMoves = GetValidMoves(move.Pawn, move.Card, move.PlayerDirection);
        if (possibleMoves == null)
        {
            throw new ArgumentException("Invalid move.");
        }

        bool isValidMove = false;
        foreach (IMove possibleMove in possibleMoves)
        {
            if (possibleMove.To.Equals(move.To))
            {
                isValidMove = true;
                break;
            }
        }

        if (!isValidMove)
        {
            throw new ArgumentException("Invalid move.");
        }

        capturedPawn = null;

        if (move.Pawn == null || move.To == null || move.Card == null)
        {
            throw new ArgumentException("Invalid move.");
        }

        int row = move.To.Row;
        int column = move.To.Column;

        IPawn targetPawn = Grid[row, column];
        if (targetPawn != null && targetPawn.OwnerId != move.Pawn.OwnerId)
        {
            capturedPawn = targetPawn;
        }

        Grid[move.Pawn.Position.Row, move.Pawn.Position.Column] = null;
        move.Pawn.Position = move.To;
        Grid[row, column] = move.Pawn;
    }

    public IReadOnlyList<IMove> GetValidMoves(IPawn pawn, IMoveCard card, Direction playerDirection)
    {
        if (pawn == null)
        {
            throw new ArgumentNullException(nameof(pawn));
        }

        if (card == null)
        {
            throw new ArgumentNullException(nameof(card));
        }

        List<IMove> validMoves = new List<IMove>();
        ICoordinate startCoordinate = pawn.Position;

        IReadOnlyList<ICoordinate> possibleTargetCoordinates = card.GetPossibleTargetCoordinates(startCoordinate, playerDirection, Size);

        foreach (ICoordinate targetCoordinate in possibleTargetCoordinates)
        {
            if (IsWithinBounds(targetCoordinate.Row, targetCoordinate.Column))
            {
                if (IsMoveValid(pawn, targetCoordinate))
                {
                    validMoves.Add(new Move(card, pawn, playerDirection, targetCoordinate));
                }
            }
        }

        return validMoves.AsReadOnly();
    }

    private bool IsWithinBounds(int row, int column)
    {
        return row >= 0 && row < Size && column >= 0 && column < Size;
    }

    private bool IsMoveValid(IPawn pawn, ICoordinate to)
    {
        IPawn targetPawn = Grid[to.Row, to.Column];
        if (targetPawn == null)
        {
            return true;
        }

        if (targetPawn.OwnerId != pawn.OwnerId)
        {
            return true;
        }

        return false;
    }

    public void PositionSchoolOfPlayer(IPlayer player)
    {
        if (player.School == null)
        {
            throw new InvalidOperationException("Player school is null");
        }

        if (player.School.AllPawns == null)
        {
            throw new InvalidOperationException("School pawns are null!");
        }

        int matSize = Size;
        int row = player.Direction == Direction.North ? 0 : matSize - 1;

        for (int i = 0; i < player.School.AllPawns.Length; i++)
        {
            int column = (matSize - player.School.AllPawns.Length) / 2 + i;

            player.School.AllPawns[i].Position = new Coordinate(row, column);

            Grid[row, column] = player.School.AllPawns[i];
        }
    }
 }