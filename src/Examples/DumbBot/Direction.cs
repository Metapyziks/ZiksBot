using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DumbBot
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
    }
}
