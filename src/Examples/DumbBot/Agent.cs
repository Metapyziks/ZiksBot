using System;

namespace DumbBot
{
    class Agent
    {
        // If Team is 0, this agent is yours
        public readonly int Team;

        public Direction Direction { get; private set; }
        public Position Position { get; private set; }

        // Set this during your turn to make an order
        public Order Order { get; set; }

        public Agent( int team )
        {
            Team = team;
        }

        public Agent( int team, Position pos, Direction dir )
            : this( team )
        {
            Position = pos;
            Direction = dir;
        }

        // Sends this agent's order to the server
        public void CommitMove()
        {
            if ( Order != Order.None )
            {
                Console.Write( "{0} {1} ", Position.X, Position.Y );

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

        public bool CanMove()
        {
            Position nextPos = Position + Direction;
            return GameState.Map[ nextPos.X, nextPos.Y ] != Tile.Wall;
        }
    }
}
