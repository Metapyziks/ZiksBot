using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Game
{
    class Program
    {
        static FileStream myLogStream;
        static StreamWriter myLogWriter;

        static void Main( string[] args )
        {
            GameState.TeamCount = args.Length;

            if ( GameState.TeamCount < 2 )
                return;

            GameState.Timeout = 1000;
            GameState.TurnLimit = 500;
            GameState.Seed = (int) DateTime.UtcNow.ToBinary();
            GameState.FogOfWar = true;

            // Would load a map now, but using an empty one until then
            GameState.MapWidth = 32;
            GameState.MapHeight = 32;

            GameState.PostSetup();

            myLogStream = new FileStream( "game.log", FileMode.Create, FileAccess.Write );
            myLogWriter = new StreamWriter( myLogStream );

            GameState.Teams = new Team[ GameState.TeamCount ];

            for ( int i = 0; i < GameState.TeamCount; ++i )
            {
                GameState.Teams[ i ] = new Team( i, args[ i ] );
                GameState.Teams[ i ].Bases.Add( new Position( i * 4, i * 4 ) );

                foreach ( Position pos in GameState.Teams[ i ].Bases )
                    GameState.Teams[ i ].Agents.Add( new Agent( GameState.Teams[ i ], pos, Direction.North ) );
            }

            LogComment( "new game staring with properties:" );
            Log( "turns", GameState.TurnLimit );
            Log( "seed", GameState.Seed );
            Log( "width", GameState.MapWidth );
            Log( "height", GameState.MapHeight );
            Log( "fow", GameState.FogOfWar );
            Log( "timeout", GameState.Timeout );

            if ( StartPrograms() )
            {
                foreach ( Team team in GameState.Teams )
                    team.SendSetup();

                for ( GameState.Turn = 0; GameState.Turn < GameState.TurnLimit; ++ GameState.Turn )
                {
                    Log( "turn", GameState.Turn + 1 );

                    foreach ( Team team in GameState.Teams )
                    {
                        if ( !team.Eliminated )
                        {
                            team.SendGameState();
                            team.TakeTurn();

                            if ( team.Eliminated )
                            {
                                Log( "# bot", team.ID, "eliminated" );
                                team.WriteLine( "done" );
                            }
                        }
                    }

                    foreach ( Team team in GameState.Teams )
                    {
                        if ( !team.Eliminated )
                        {
                            foreach ( Agent agent in team.Agents )
                            {
                                agent.FinishTurn();
                            }
                        }
                    }
                }
            }

            StopPrograms();
            myLogWriter.Close();
            myLogStream.Close();
        }

        static bool StartPrograms()
        {
            LogComment( "starting ai programs" );

            for ( int i = 0; i < GameState.TeamCount; ++i )
            {
                try
                {
                    GameState.Teams[ i ].StartProgram();
                }
                catch
                {
                    LogComment( String.Format( "Invalid executable location given for team #{0}, aborting", i ) );
                    return false;
                }
            }

            return true;
        }

        static void StopPrograms()
        {
            LogComment( "stopping ai programs" );

            for ( int i = 0; i < GameState.TeamCount; ++i )
                if ( GameState.Teams[ i ] != null )
                    GameState.Teams[ i ].StopProgram();
        }

        public static void Log( params object[] values )
        {
            for ( int i = 0; i < values.Length; ++i )
            {
                String val = values[ i ].ToString() + ( i < values.Length - 1 ? " " : "" );
                Console.Write( val );
                myLogWriter.Write( val );
            }

            Console.WriteLine();
            myLogWriter.WriteLine();
        }

        public static void LogComment( params object[] values )
        {
            Console.Write( "#" );
            myLogWriter.Write( "#" );

            for ( int i = 0; i < values.Length; ++i )
            {
                String val = " " + values[ i ].ToString();
                Console.Write( val );
                myLogWriter.Write( val );
            }

            Console.WriteLine();
            myLogWriter.WriteLine();
        }
    }
}
