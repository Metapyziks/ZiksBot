﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    struct Direction
    {
        static readonly Direction North = new Direction( 0, -1 );
        static readonly Direction East  = new Direction( 1, 0 );
        static readonly Direction South = new Direction( 0, 1 );
        static readonly Direction West  = new Direction( -1, 0 );

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
    }
}