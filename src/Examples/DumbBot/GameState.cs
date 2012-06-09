using System;
using System.Collections.Generic;

namespace DumbBot
{
    // Contains information about the map
    // layout, object positions and general
    // game properties
    static class GameState
    {
        public static int Timeout;

        public static int TeamCount;

        public static Random Random;

        public static bool FogOfWar;
        public static float ViewRange;

        public static int Turn;
        public static int TurnLimit;

        public static Tile[ , ] Map;

        // These arrays have one entry per team
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

        // Clear any object positions, ready to be
        // told new locations by the server
        public static void PreTurn()
        {
            for ( int i = 0; i < TeamCount; ++i )
            {
                Agents[ i ].Clear();
                Dead[ i ].Clear();
                Bases[ i ].Clear();
            }

            Packages.Clear();
        }

        public static bool IsWall( Position loc )
        {
            return Map[ loc.X, loc.Y ] == Tile.Wall;
        }
    }
}
