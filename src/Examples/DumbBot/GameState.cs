using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DumbBot
{
    static class GameState
    {
        public static int Timeout;

        public static int TeamCount;

        public static Random Random;
        public static bool FogOfWar;

        public static int Turn;
        public static int TurnLimit;

        public static Tile[ , ] Map;

        public static List<Agent>[] Agents;
        public static List<Agent>[] Dead;
        public static List<Position>[] Bases;
        public static List<Position> Packages;

        public static int MapWidth;
        public static int MapHeight;

        public static void PostSetup()
        {
            Map = new Tile[ MapWidth, MapHeight ];

            if( GameState.Random == null )
                GameState.Random = new Random();

            Agents = new List<Agent>[ TeamCount ];
            Dead = new List<Agent>[ TeamCount ];
            Bases = new List<Position>[ TeamCount ];
            for ( int i = 0; i < TeamCount; ++i )
            {
                Agents[ i ] = new List<Agent>();
                Dead[ i ] = new List<Agent>();
                Bases[ i ] = new List<Position>();
            }
            Packages = new List<Position>();
        }

        public static void PreTurn()
        {
            foreach ( Agent agent in Agents[ 0 ] )
                agent.Confirmed = false;

            for ( int i = 0; i < TeamCount; ++i )
            {
                if ( i > 0 )
                    Agents[ i ].Clear();

                Dead[ i ].Clear();
                Bases[ i ].Clear();
            }

            Packages.Clear();
        }

        public static void TurnStart()
        {
            for ( int i = Agents[ 0 ].Count - 1; i >= 0; --i )
                if ( !Agents[ 0 ][ i ].Confirmed )
                    Agents[ 0 ].RemoveAt( i );
        }

        public static bool IsWall( Position loc )
        {
            return Map[ loc.X, loc.Y ] == Tile.Wall;
        }
    }
}
