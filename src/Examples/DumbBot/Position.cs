using System;

namespace DumbBot
{
    struct Position
    {
        public int X;
        public int Y;

        public int LengthSquared
        {
            get { return X * X + Y * Y; }
        }

        public int LengthManhattan
        {
            get { return Math.Abs( X ) + Math.Abs( Y ); }
        }

        public Position( int x, int y )
        {
            X = x;
            Y = y;
        }

        public static Position operator +( Position a, Position b )
        {
            return new Position( a.X + b.X, a.Y + b.Y );
        }

        public static Position operator -( Position a, Position b )
        {
            return new Position( a.X - b.X, a.Y - b.Y );
        }

        public Position BestVector( Position pos )
        {
            Position dist = pos.Wrap() - this.Wrap();

            if ( dist.X >= GameState.MapWidth >> 1 )
                dist.X -= GameState.MapWidth;
            else if ( dist.X < -( GameState.MapWidth >> 1 ) )
                dist.X += GameState.MapWidth;

            if ( dist.Y >= GameState.MapHeight >> 1 )
                dist.Y -= GameState.MapHeight;
            else if ( dist.Y < -( GameState.MapHeight >> 1 ) )
                dist.Y += GameState.MapHeight;

            return dist;
        }

        public Position Wrap()
        {
            return new Position(
                X - (int) Math.Floor( (double) X / GameState.MapWidth ) * GameState.MapWidth,
                Y - (int) Math.Floor( (double) Y / GameState.MapHeight ) * GameState.MapHeight
            );
        }

        public override bool Equals( object obj )
        {
            if ( obj is Position )
            {
                Position pos = (Position) obj;
                return pos.X == X && pos.Y == Y;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }
    }
}
