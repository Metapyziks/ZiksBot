using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
        public static float ViewRange;

        public static int Turn;
        public static int TurnLimit;

        public static Tile[ , ] Map;

        public static int MapWidth;
        public static int MapHeight;

        public static bool LoadMap( String mapPath )
        {
            if ( !File.Exists( mapPath ) )
                return false;

            using ( FileStream stream = new FileStream( mapPath, FileMode.Open, FileAccess.Read ) )
            {
                StreamReader reader = new StreamReader( stream );

                String line;
                while ( !reader.EndOfStream && ( line = reader.ReadLine().Trim().ToLower() ) != "map" )
                {
                    if ( line.Length == 0 )
                        continue;

                    String[] split = line.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );
                        
                    if ( split.Length < 2 )
                        return false;

                    switch ( split[ 0 ] )
                    {
                        case "teams":
                            TeamCount = int.Parse( split[ 1 ] ); break;
                        case "width":
                            MapWidth = int.Parse( split[ 1 ] ); break;
                        case "height":
                            MapHeight = int.Parse( split[ 1 ] ); break;
                        default:
                            return false;
                    }
                }

                if( TeamCount == 0 || MapWidth == 0 || MapHeight == 0 )
                    return false;

                Program.Log( "teams", TeamCount );
                Program.Log( "width", MapWidth );
                Program.Log( "height", MapHeight );

                Map = new Tile[ MapWidth, MapHeight ];

                Teams = new Team[ TeamCount ];

                for ( int i = 0; i < GameState.TeamCount; ++i )
                    Teams[ i ] = new Team( i );

                Program.Log( "map" );

                if ( !reader.EndOfStream )
                {
                    int y = 0;
                    while ( !reader.EndOfStream && ( line = reader.ReadLine().ToLower() ).Trim() != "end" )
                    {
                        if ( line.Length < MapWidth * 2 )
                            return false;

                        line = line.Substring( 0, MapWidth * 2 );
                        Program.Log( line );

                        for ( int x = 0; x < MapWidth; ++x )
                        {
                            char c = line[ x << 1 ];
                            if( c == '#' )
                                Map[ x, y ] = Tile.Wall;
                            else if( char.IsNumber( c ) )
                            {
                                int team;
                                if ( !int.TryParse( c.ToString(), out team ) || team >= TeamCount )
                                    return false;

                                Direction dir;
                                if ( !Direction.TryParse( line[ ( x << 1 ) + 1 ].ToString(), out dir ) )
                                    return false;

                                Teams[ team ].Bases.Add( new Position( x, y ) );
                                Teams[ team ].Agents.Add( new Agent( Teams[ team ], new Position( x, y ), dir ) );
                            }
                            else
                                Map[ x, y ] = Tile.Empty;
                        }
                        ++y;
                    }
                }

                Program.Log( "end" );
            }

            Dead = new List<Agent>();
            Packages = new List<Position>();

            return true;
        }

        public static bool IsWall( Position loc )
        {
            return Map[ loc.X, loc.Y ] == Tile.Wall;
        }
    }
}
