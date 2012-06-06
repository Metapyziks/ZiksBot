using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DumbBot
{
    static class GameState
    {
        public static Random Random;
        public static bool FogOfWar;

        public static Tile[ , ] Map;

        public static List<Bot> MyBots;
        public static List<Bot> EnemyBots;

        public static int MapWidth;
        public static int MapHeight;

        public static void ResetMap()
        {
            Map = new Tile[ MapWidth, MapHeight ];
        }

        public static bool IsWall( Vector loc )
        {
            return Map[ loc.X, loc.Y ] == Tile.Wall;
        }
    }
}
