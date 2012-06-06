using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    class Bot
    {
        public readonly Team Team;

        public Direction Direction { get; private set; }
        public Position Position { get; private set; }

        public Order Order { get; set; }

        public Bot( Team team )
        {
            Team = team;
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
    }
}
