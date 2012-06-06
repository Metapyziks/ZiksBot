using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DumbBot
{
    class Bot
    {
        public readonly int Team;

        public bool Friendly { get { return Team == 0; } }
        public bool Enemy { get { return Team != 0; } }

        public Direction Direction { get; private set; }
        public Vector Position { get; private set; }

        public Order Order { get; set; }

        public Bot( int team )
        {
            Team = team;
        }

        public void CommitMove()
        {
            if ( Order != Order.None )
            {
                Console.Write( "o {0} {1} ", Position.X, Position.Y );

                switch ( Order )
                {
                    case Order.MoveForward:
                        Console.WriteLine( "f" ); break;
                    case Order.TurnLeft:
                        Console.WriteLine( "l" ); break;
                    case Order.TurnRight:
                        Console.WriteLine( "r" ); break;
                }
            }
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
