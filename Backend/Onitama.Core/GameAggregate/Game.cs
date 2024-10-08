using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.MoveCardAggregate;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.PlayMatAggregate;
using Onitama.Core.PlayMatAggregate.Contracts;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util;
using Onitama.Core.Util.Contracts;

namespace Onitama.Core.GameAggregate;

/// <inheritdoc cref="IGame"/>
internal class Game : IGame
{
    public Guid Id { get; }

    public IPlayMat PlayMat { get; }

    public IMoveCard ExtraMoveCard { get; private set; }

    public IPlayer[] Players { get; }
    public Guid PlayerToPlayId { get; private set; }

    public Guid WinnerPlayerId { get; private set; }
    public string WinnerMethod { get; private set; }

    /// <summary>
    /// Creates a new game and determines the player to play first.
    /// </summary>
    /// <param name="id">The unique identifier of the game</param>
    /// <param name="playMat">
    /// The play mat
    /// (with the schools of the player already positioned on it)
    /// </param>
    /// <param name="players">
    /// The 2 players that will play the game
    /// (with 2 move cards each)
    /// </param>
    /// <param name="extraMoveCard">
    /// The fifth card used to exchange cards after the first move
    /// </param>
    public Game(Guid id, IPlayMat playMat, IPlayer[] players, IMoveCard extraMoveCard)
    {
        Id = id;
        PlayMat = playMat;
        Players = players;
        ExtraMoveCard = extraMoveCard;
        PlayerToPlayId = players[0].Id; // Assume the first player plays first
    }

    /// <summary>
    /// Creates a game that is a copy of another game.
    /// </summary>
    /// <remarks>
    /// This is an EXTRA. Not needed to implement the minimal requirements.
    /// To make the mini-max algorithm for an AI game play strategy work, this constructor should be implemented.
    /// </remarks>
    public Game(IGame otherGame)
    {
        Id = otherGame.Id;
        PlayMat = otherGame.PlayMat;
        Players = otherGame.Players.Select(player => new PlayerBase(player)).ToArray();
        ExtraMoveCard = otherGame.ExtraMoveCard;
        PlayerToPlayId = otherGame.PlayerToPlayId;
        WinnerPlayerId = otherGame.WinnerPlayerId;
        WinnerMethod = otherGame.WinnerMethod;
    }

    public IReadOnlyList<IMove> GetPossibleMovesForPawn(Guid playerId, Guid pawnId, string moveCardName)
    {
        IPlayer player = GetPlayerById(playerId);
        if (player == null)
            throw new InvalidOperationException($"Invalid player id: {playerId}");

        IPawn pawn = player.School.GetPawn(pawnId);
        if (pawn == null)
            throw new InvalidOperationException($"Invalid pawn id: {pawnId}");

        IMoveCard card = null;
        foreach (IMoveCard moveCard in player.MoveCards)
        {
            if (moveCard.Name == moveCardName)
            {
                card = moveCard;
                break;
            }
        }

        if (card == null)
            throw new ApplicationException($"Player does not possess the card '{moveCardName}'");

        return PlayMat.GetValidMoves(pawn, card, player.Direction);
    }

    public IReadOnlyList<IMove> GetAllPossibleMovesFor(Guid playerId)
    {
        List<IMove> allMoves = new List<IMove>();

        foreach (IPawn pawn in GetPlayerById(playerId).School.AllPawns)
        {
            foreach (IMoveCard card in GetPlayerById(playerId).MoveCards)
            {
                IReadOnlyList<IMove> validMoves = PlayMat.GetValidMoves(pawn, card, GetPlayerById(playerId).Direction);
                allMoves.AddRange(validMoves);
            }
        }

        return allMoves;
    }

    public void MovePawn(Guid playerId, Guid pawnId, string cardName, ICoordinate to)
    {
        // Check if it's the player's turn
        if (playerId != PlayerToPlayId)
        {
            throw new ApplicationException("It's not your turn.");
        }

        IPlayer player = GetPlayerById(playerId);
        if (player == null)
        {
            throw new ApplicationException("Player is null");
        }

        IMoveCard card = player.MoveCards.FirstOrDefault(c => c.Name == cardName);
        if (card == null)
        {
            throw new ApplicationException("Card is null");
        }

        // Get the pawn
        IPawn pawn = player.School.GetPawn(pawnId);
        if (pawn == null)
        {
            throw new ApplicationException("Pawn is null");
        }

        // Create the move
        IMove move = new Move(card, pawn, player.Direction, to);
        if (move == null)
        {
            throw new ApplicationException("Move is null");
        }

        // Execute the move
        IPawn capturedPawn;
        PlayMat.ExecuteMove(move, out capturedPawn);
        ExtraMoveCard = card;

        Console.WriteLine($"Move executed. Captured Pawn: {capturedPawn}");

        ICoordinate opponentTempleArchPosition = GetNextOpponent(playerId).School.TempleArchPosition;

        // Check if the move is a winning move
        if (to.Row == opponentTempleArchPosition.Row && to.Column == opponentTempleArchPosition.Column)
        {
            WinnerPlayerId = playerId;
            WinnerMethod = "Way of the Stream";
        }
        else if (capturedPawn != null && capturedPawn == GetNextOpponent(playerId).School.Master)
        {
            WinnerPlayerId = playerId;
            WinnerMethod = "Way of the Stone";
        }

        // Switch the player to play
        PlayerToPlayId = GetNextOpponent(playerId).Id;
    }

    public void SkipMovementAndExchangeCard(Guid playerId, string moveCardName)
    {
        if (playerId != PlayerToPlayId)
        {
            throw new ApplicationException("It's not your turn.");
        }

        IPlayer player = GetPlayerById(playerId);
        if (player == null)
        {
            throw new ApplicationException("Player is null");
        }
        IMoveCard card = null;

        foreach (IMoveCard moveCard in player.MoveCards)
        {
            if (moveCard.Name == moveCardName)
            {
                card = moveCard;
                break;
            }
        }

        if (card == null)
        {
            throw new ApplicationException($"Player does not possess the card '{moveCardName}'");
        }

        foreach (IPawn pawn in player.School.AllPawns)
        {
            foreach (IMoveCard moveCard in player.MoveCards)
            {
                IReadOnlyList<IMove> validMoves = PlayMat.GetValidMoves(pawn, moveCard, player.Direction);
                if (validMoves.Count > 0)
                {
                    throw new ApplicationException("A valid move is possible, so you cannot skip your turn.");
                }
            }
        }

        player.MoveCards.Remove(card);
        player.MoveCards.Add(ExtraMoveCard);
        ExtraMoveCard = card;

        PlayerToPlayId = GetNextOpponent(playerId).Id;
    }

    public IPlayer GetNextOpponent(Guid playerId)
    {
        if (Players[0].Id == playerId)
            return Players[1];
        if (Players[1].Id == playerId)
            return Players[0];
        return null;
    }

    public IPlayer GetPlayerById(Guid playerId)
    {
        foreach (IPlayer player in Players)
        {
            if (player.Id == playerId)
            {
                return player;
            }
        }
        throw new InvalidOperationException($"Player not found with ID: {playerId}");
    }
}