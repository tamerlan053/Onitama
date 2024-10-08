using System.Drawing;
using System.Numerics;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.MoveCardAggregate;
using Onitama.Core.MoveCardAggregate.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.PlayMatAggregate;
using Onitama.Core.PlayMatAggregate.Contracts;
using Onitama.Core.SchoolAggregate;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.TableAggregate.Contracts;
using Onitama.Core.Util;
using Onitama.Core.Util.Contracts;

namespace Onitama.Core.GameAggregate;

internal class GameFactory : IGameFactory
{
    private readonly IMoveCardRepository _moveCardRepository;
    public GameFactory(IMoveCardRepository moveCardRepository)
    {
        _moveCardRepository = moveCardRepository;
    }

    public IGame CreateNewForTable(ITable table)
    {
        Color[] colors = table.SeatedPlayers.Select(p => p.Color).ToArray();

        IMoveCard[] allMoveCards = _moveCardRepository.LoadSet(MoveCardSet.Original, colors);

        Random random = new Random();
        IMoveCard[] chosenMoveCards = allMoveCards.OrderBy(_ => random.Next()).Take(5).ToArray();
        foreach (IPlayer player in table.SeatedPlayers)
        {
            player.MoveCards.Clear();
            player.MoveCards.Add(chosenMoveCards[0]);
            player.MoveCards.Add(chosenMoveCards[1]);
            chosenMoveCards = chosenMoveCards.Skip(2).ToArray();
        }
        IMoveCard extraMoveCard = chosenMoveCards[0];

        IGame game = new Game(Guid.NewGuid(), new PlayMat(new IPawn[table.Preferences.PlayerMatSize, table.Preferences.PlayerMatSize], table.Preferences.PlayerMatSize), table.SeatedPlayers.ToArray(), extraMoveCard);

        foreach (IPlayer player in game.Players)
        {

            game.PlayMat.PositionSchoolOfPlayer(player);
        }

        return game;
    }
}