using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Onitama.Core.MoveCardAggregate.Contracts;
using System.Numerics;
using Onitama.Core.PlayerAggregate;
using Onitama.Core.PlayerAggregate.Contracts;
using Onitama.Core.TableAggregate.Contracts;
using Onitama.Core.UserAggregate;
using Onitama.Core.Util;
using Onitama.Core.SchoolAggregate.Contracts;
using Onitama.Core.SchoolAggregate;
using Onitama.Core.Util.Contracts;

namespace Onitama.Core.TableAggregate
{
    internal class Table : ITable
    {
        private readonly List<IPlayer> _seatedPlayers;
        private static readonly Color[] PossibleColors = { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Orange };

        public Table(Guid id, TablePreferences preferences)
        {
            Id = id;
            Preferences = preferences;
            _seatedPlayers = new List<IPlayer>();
            GameId = Guid.Empty;
        }

        public Guid Id { get; }
        public TablePreferences Preferences { get; }

        public Guid OwnerPlayerId
        {
            get
            {
                if (_seatedPlayers.FirstOrDefault() != null)
                    return _seatedPlayers.FirstOrDefault().Id;
                else
                    return Guid.Empty;
            }
        }

        public IReadOnlyList<IPlayer> SeatedPlayers => _seatedPlayers;

        public bool HasAvailableSeat
        {
            get
            {
                if (_seatedPlayers.Count < Preferences.NumberOfPlayers)
                    return true;
                else
                    return false;
            }
        }

        public Guid GameId { get; set; }

        public void FillWithArtificialPlayers(IGamePlayStrategy gamePlayStrategy)
        {
            throw new NotImplementedException();
        }

        public bool HasAvailableSeats()
        {
            return _seatedPlayers.Count < Preferences.NumberOfPlayers;
        }

        public void Join(User user)
        {
            if (_seatedPlayers.Any(p => p.Id == user.Id))
                throw new InvalidOperationException($"User {user.Id} is already seated at the table.");

            if (!HasAvailableSeat)
                throw new InvalidOperationException("Table is full.");

            Color color = GetAvailableColor();
            Direction direction;
            IList<IMoveCard> moveCards = new List<IMoveCard>();

            if (_seatedPlayers.Count == 0)
            {
                direction = Direction.North;

                IPawn master = new Pawn(Guid.NewGuid(), user.Id, PawnType.Master, new Coordinate(0, 2));
                IPawn student1 = new Pawn(Guid.NewGuid(), user.Id, PawnType.Student, new Coordinate(0, 0));
                IPawn student2 = new Pawn(Guid.NewGuid(), user.Id, PawnType.Student, new Coordinate(0, 1));
                IPawn student3 = new Pawn(Guid.NewGuid(), user.Id, PawnType.Student, new Coordinate(0, 3));
                IPawn student4 = new Pawn(Guid.NewGuid(), user.Id, PawnType.Student, new Coordinate(0, 4));
                IPawn[] students = new IPawn[] { student1, student2, student3, student4 };
                IPawn[] allPawns = new IPawn[] { student1, student2, master, student3, student4 };
                ICoordinate templeArchPosition = master.Position;
                ISchool school = new School(master, students, allPawns, templeArchPosition);
                IPlayer player = new HumanPlayer(user.Id, user.WarriorName, color, direction, school, moveCards);
                _seatedPlayers.Add(player);
            }
            else
            {
                direction = Direction.South;

                IPawn master = new Pawn(Guid.NewGuid(), user.Id, PawnType.Master, new Coordinate(4, 2));
                IPawn student1 = new Pawn(Guid.NewGuid(), user.Id, PawnType.Student, new Coordinate(4, 0));
                IPawn student2 = new Pawn(Guid.NewGuid(), user.Id, PawnType.Student, new Coordinate(4, 1));
                IPawn student3 = new Pawn(Guid.NewGuid(), user.Id, PawnType.Student, new Coordinate(4, 3));
                IPawn student4 = new Pawn(Guid.NewGuid(), user.Id, PawnType.Student, new Coordinate(4, 4));
                IPawn[] students = new IPawn[] { student1, student2, student3, student4 };
                IPawn[] allPawns = new IPawn[] { student1, student2, master, student3, student4 };
                ICoordinate templeArchPosition = master.Position;
                ISchool school = new School(master, students, allPawns, templeArchPosition);
                IPlayer player = new HumanPlayer(user.Id, user.WarriorName, color, direction, school, moveCards);
                _seatedPlayers.Add(player);
            }      
        }


        public void Leave(Guid userId)
        {
            IPlayer player = _seatedPlayers.FirstOrDefault(p => p.Id == userId);
            if (player == null)
                throw new InvalidOperationException($"User {userId} is not seated at the table.");

            _seatedPlayers.Remove(player);
        }

        private Color GetAvailableColor()
        {
            List<Color> usedColors = _seatedPlayers.Select(p => p.Color).ToList();
            List<Color> availableColors = PossibleColors.Except(usedColors).ToList();

            Random random = new Random();
            int index = random.Next(0, availableColors.Count);

            return availableColors[index];
        }
    }
}