using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.Util;
using Onitama.Core.Util.Contracts;

namespace Onitama.Core.GameAggregate;

internal class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;

    public GameService(IGameRepository gameRepository)
    {
        if (gameRepository == null)
        {
            throw new ArgumentNullException(nameof(gameRepository));
        }
        else
        {
            _gameRepository = gameRepository;
        }
    }

    public IGame GetGame(Guid gameId)
    {
        IGame game = _gameRepository.GetById(gameId);
        if (game == null)
        {
            throw new KeyNotFoundException($"Game with ID {gameId} not found.");
        }
        return game;
    }

    public IReadOnlyList<IMove> GetPossibleMovesForPawn(Guid gameId, Guid playerId, Guid pawnId, string moveCardName)
    {
        IGame game = GetGame(gameId);

        IPlayer player = game.GetPlayerById(playerId);
        if (player == null)
        {
            throw new ArgumentException($"Player with ID {playerId} not found in game with ID {gameId}.");
        }

        IPawn pawn = null;
        foreach (IPawn p in player.School.AllPawns)
        {
            if (p.Id == pawnId)
            {
                pawn = p;
                break;
            }
        }

        if (pawn == null)
        {
            throw new ArgumentException($"Pawn with ID {pawnId} not found for player with ID {playerId}.");
        }

        IMoveCard moveCard = null;
        foreach (IMoveCard card in player.MoveCards)
        {
            if (card.Name == moveCardName)
            {
                moveCard = card;
                break;
            }
        }

       // if (moveCard == null)
       // {
       //     throw new ArgumentException($"Move card with name {moveCardName} not found for player with ID {playerId}.");
       // }

        return game.PlayMat.GetValidMoves(pawn, moveCard, player.Direction);
    }

    public void MovePawn(Guid gameId, Guid playerId, Guid pawnId, string moveCardName, ICoordinate to)
    {
        IGame game = GetGame(gameId);

        IPlayer player = game.GetPlayerById(playerId);
        if (player == null)
        {
            throw new ArgumentException($"Player with ID {playerId} not found in game with ID {gameId}.");
        }

        IPawn pawn = null;
        foreach (IPawn schoolPawn in player.School.AllPawns)
        {
            if (schoolPawn.Id == pawnId)
            {
                pawn = schoolPawn;
                break;
            }
        }

        if (pawn == null)
        {
            throw new ArgumentException($"Pawn with ID {pawnId} not found for player with ID {playerId}.");
        }

        IMoveCard moveCard = null;
        foreach (IMoveCard card in player.MoveCards)
        {
            if (card.Name == moveCardName)
            {
                moveCard = card;
                break;
            }
        }

        if (moveCard == null)
        {
            throw new ArgumentException($"Move card with name {moveCardName} not found for player with ID {playerId}.");
        }

        game.MovePawn(playerId, pawnId, moveCardName, to);

        IMoveCard extraMoveCard = game.ExtraMoveCard;
        player.MoveCards.Remove(moveCard);
        player.MoveCards.Add(extraMoveCard);
    }


    public void SkipMovementAndExchangeCard(Guid gameId, Guid playerId, string moveCardName)
    {
        IGame game = GetGame(gameId);

        IPlayer player = game.GetPlayerById(playerId);
        if (player == null)
        {
            throw new ArgumentException($"Player with ID {playerId} not found in game with ID {gameId}.");
        }

        IMoveCard moveCard = null;
        foreach (IMoveCard card in player.MoveCards)
        {
            if (card.Name == moveCardName)
            {
                moveCard = card;
                break;
            }
        }

        if (moveCard == null)
        {
            throw new ArgumentException($"Move card with name {moveCardName} not found for player with ID {playerId}.");
        }

        game.SkipMovementAndExchangeCard(playerId, moveCardName);
    }
}