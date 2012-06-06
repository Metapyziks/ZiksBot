using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    static class GameState
    {
        public static int Timeout;

        public static int Seed;
        public static bool FogOfWar;

        public static int Turn;
        public static int TurnLimit;

        public static Tile[ , ] Map;

        public static int MapWidth;
        public static int MapHeight;

        public static void ResetMap()
        {
            Map = new Tile[ MapWidth, MapHeight ];
        }

        public static bool IsWall( Position loc )
        {
            return Map[ loc.X, loc.Y ] == Tile.Wall;
        }
    }
}
