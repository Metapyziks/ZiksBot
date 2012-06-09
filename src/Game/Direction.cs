using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    struct Direction
    {
        public static readonly Direction North = new Direction( 0, -1 );
        public static readonly Direction East  = new Direction( 1, 0 );
        public static readonly Direction South = new Direction( 0, 1 );
        public static readonly Direction West  = new Direction( -1, 0 );
        public static readonly Direction None  = new Direction( 0, 0 );

        public static Direction Parse( String str )
        {
            switch ( str )
            {
                case "n":
                    return North;
                case "s":
                    return South;
                case "e":
                    return East;
                case "w":
                    return West;
            }

            throw new FormatException( str + " cannot be parsed as a Direction" );
        }

        public static bool TryParse( String str, out Direction dir )
        {
            switch ( str )
            {
                case "n":
                    dir = North; return true;
                case "s":
                    dir = South; return true;
                case "e":
                    dir = East; return true;
                case "w":
                    dir = West; return true;
            }

            dir = Direction.None;
            return false;
        }

        public readonly int X;
        public readonly int Y;

        public Direction Left
        {
            get
            {
                return new Direction( Y, -X );
            }
        }

        public Direction Right
        {
            get
            {
                return new Direction( -Y, X );
            }
        }

        private Direction( int x, int y )
        {
            X = x;
            Y = y;
        }

        public static Position operator +( Position vec, Direction dir )
        {
            return new Position( vec.X + dir.X, vec.Y + dir.Y ).Wrap();
        }

        public static Position operator -( Position vec, Direction dir )
        {
            return new Position( vec.X - dir.X, vec.Y - dir.Y ).Wrap();
        }

        public static Position operator *( Position vec, Direction dir )
        {
            if ( dir.Equals( North ) )
                return vec;
            if ( dir.Equals( East ) )
                return new Position( -vec.Y, vec.X );
            if ( dir.Equals( South ) )
                return new Position( -vec.X, -vec.Y );
            if ( dir.Equals( West ) )
                return new Position( vec.Y, -vec.X );

            return new Position();
        }

        public override string ToString()
        {
            if ( X == -1 )
                return "w";
            if ( X == 1 )
                return "e";
            if ( Y == -1 )
                return "n";

            return "s";
        }

        public override bool Equals( object obj )
        {
            if ( obj is Direction )
            {
                Direction dir = (Direction) obj;
                return dir.X == X && dir.Y == Y;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Y == -1 ? 0 : X == 1 ? 1 : Y == 1 ? 2 : 3;
        }
    }
}
