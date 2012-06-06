using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DumbBot
{
    struct Vector
    {
        public int X;
        public int Y;

        public Vector( int x, int y )
        {
            X = x;
            Y = y;
        }

        public static Vector operator +( Vector a, Vector b )
        {
            return new Vector( a.X + b.X, a.Y + b.Y );
        }

        public static Vector operator -( Vector a, Vector b )
        {
            return new Vector( a.X - b.X, a.Y - b.Y );
        }

        public Vector BestVector( Vector pos )
        {
            Vector dist = pos.Wrap() - this.Wrap();

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

        public Vector Wrap()
        {
            return new Vector(
                X - (int) Math.Floor( (double) X / GameState.MapWidth ) * GameState.MapWidth,
                Y - (int) Math.Floor( (double) Y / GameState.MapHeight ) * GameState.MapHeight
            );
        }
    }
}
