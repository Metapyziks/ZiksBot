namespace Game
{
    class Agent
    {
        private static int stNextID = 0;

        public readonly Team Team;
        public readonly int ID;

        public Direction Direction { get; private set; }
        public Position Position { get; private set; }

        public Order Order { get; set; }

        public bool Dead { get; set; }

        public Agent( Team team )
        {
            Team = team;
            ID = stNextID++;
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
