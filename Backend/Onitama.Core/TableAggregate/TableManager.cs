using Onitama.Core.GameAggregate;
using Onitama.Core.GameAggregate.Contracts;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.TableAggregate.Contracts;
using Onitama.Core.UserAggregate;
using System;

namespace Onitama.Core.TableAggregate
{
    /// <summary>
    /// Manages operations related to game tables, such as creating new tables, joining and leaving tables,
    /// starting games, and filling tables with artificial players.
    /// </summary>
    internal class TableManager : ITableManager
    {
        private readonly ITableRepository _tableRepository;
        private readonly ITableFactory _tableFactory;
        private readonly IGameFactory _gameFactory;
        private readonly IGameRepository _gameRepository;
        private readonly IGamePlayStrategy _gamePlayStrategy;

        public TableManager(
            ITableRepository tableRepository,
            ITableFactory tableFactory,
            IGameRepository gameRepository,
            IGameFactory gameFactory,
            IGamePlayStrategy gamePlayStrategy)
        {
            _tableRepository = tableRepository;
            _tableFactory = tableFactory;
            _gameFactory = gameFactory;
            _gamePlayStrategy = gamePlayStrategy;
            _gameRepository = gameRepository;
        }

        /// <inheritdoc/>
        public ITable AddNewTableForUser(User user, TablePreferences preferences)
        {
            // Create a new table using the factory
            ITable newTable = _tableFactory.CreateNewForUser(user, preferences);

            // Add the new table to the repository
            _tableRepository.Add(newTable);

            // Return the created table
            return newTable;
        }

        /// <inheritdoc/>
        public void JoinTable(Guid tableId, User user)
        {
            // Retrieve the table from the repository
            ITable table = _tableRepository.Get(tableId);

            // Join the specified user to the table
            table.Join(user);
        }

        /// <inheritdoc/>
        public void LeaveTable(Guid tableId, User user)
        {
            // Retrieve the table from the repository
            ITable table = _tableRepository.Get(tableId);

            // Remove the user from the table
            table.Leave(user.Id);

            // If no players are seated at the table, remove the table from the repository
            if (!table.SeatedPlayers.Any())
            {
                _tableRepository.Remove(tableId);
            }
        }

        /// <inheritdoc/>
        public void FillWithArtificialPlayers(Guid tableId, User user)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IGame StartGameForTable(Guid tableId, User user)
        {
            // Retrieve the table from the repository
            ITable table = _tableRepository.Get(tableId);

            // Check if the table exists
            if (table == null)
            {
                throw new InvalidOperationException($"Table with ID '{tableId}' not found.");
            }

            // Check if the user is the owner of the table
            if (table.OwnerPlayerId != user.Id)
            {
                throw new InvalidOperationException("User is not the owner of the table.");
            }

            // Check if there are enough players seated at the table to start the game
            if (table.SeatedPlayers.Count < 2)
            {
                throw new InvalidOperationException("Error, not enough players.");
            }

            // Create a new game for the table using the game factory
            IGame game = _gameFactory.CreateNewForTable(table);

            // Assign the game ID to the table
            table.GameId = game.Id;

            // Add the game to the game repository
            _gameRepository.Add(game);

            // Return the created game
            return game;
        }
    }
}