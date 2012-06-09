using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    class Agent
    {
        public readonly Team Team;

        public Direction Direction { get; private set; }
        public Position Position { get; private set; }

        public Order Order { get; set; }

        public Agent( Team team )
        {
            Team = team;
        }

        public Agent( Team team, Position pos, Direction dir )
            : this( team )
        {
            Position = pos;
            Direction = dir;
        }

        public void FinishTurn()
        {
            switch ( Order )
            {
                case Order.MoveForward:
                    Position += Direction; break;
                case Order.TurnLeft:
                    Direction = Direction.Left; break;
                case Order.TurnRight:
                    Direction = Direction.Right; break;
            }

            Order = Order.None;
        }

        public bool CanMove()
        {
            Position nextPos = Position + Direction;
            return GameState.Map[ nextPos.X, nextPos.Y ] != Tile.Wall;
        }
    }
}
