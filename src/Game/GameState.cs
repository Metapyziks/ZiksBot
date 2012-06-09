using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    static class GameState
    {
        public static int Timeout;

        public static int TeamCount;
        public static Team[] Teams;

        public static List<Position> Packages;
        public static List<Agent> Dead;

        public static int Seed;
        public static bool FogOfWar;

        public static int Turn;
        public static int TurnLimit;

        public static Tile[ , ] Map;

        public static int MapWidth;
        public static int MapHeight;

        public static void PostSetup()
        {
            Map = new Tile[ MapWidth, MapHeight ];
            Dead = new List<Agent>();
            Packages = new List<Position>();
        }

        public static bool IsWall( Position loc )
        {
            return Map[ loc.X, loc.Y ] == Tile.Wall;
        }
    }
}
