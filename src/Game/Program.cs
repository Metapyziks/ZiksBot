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

        static int myTeamCount;
        static Team[] myTeams;

        static void Main( string[] args )
        {
            myTeamCount = args.Length;

            if ( myTeamCount < 2 )
                return;

            GameState.Timeout = 1000;
            GameState.TurnLimit = 500;
            GameState.Seed = (int) DateTime.UtcNow.ToBinary();
            GameState.FogOfWar = true;

            // Would load a map now, but using an empty one until then
            GameState.MapWidth = 32;
            GameState.MapHeight = 32;

            myLogStream = new FileStream( "game.log", FileMode.Create, FileAccess.Write );
            myLogWriter = new StreamWriter( myLogStream );
            
            myTeams = new Team[ myTeamCount ];

            for ( int i = 0; i < myTeamCount; ++i )
                myTeams[ i ] = new Team( args[ i ] );

            LogComment( "new game staring with properties:" );
            LogComment( "  turns", GameState.TurnLimit );
            LogComment( "   seed", GameState.Seed );
            LogComment( "  width", GameState.MapWidth );
            LogComment( " height", GameState.MapHeight );
            LogComment( "    fow", GameState.FogOfWar );
            LogComment( "timeout", GameState.Timeout );

            if ( StartPrograms() )
            {
                SendSetup();

                for ( GameState.Turn = 0; GameState.Turn < GameState.TurnLimit; ++ GameState.Turn )
                {
                    LogComment( "turn", GameState.Turn + 1 );
                }
            }

            StopPrograms();
            myLogWriter.Close();
            myLogStream.Close();
        }

        static bool StartPrograms()
        {
            LogComment( "starting ai programs" );

            for ( int i = 0; i < myTeamCount; ++i )
            {
                try
                {
                    myTeams[ i ].StartProgram();
                }
                catch
                {
                    LogComment( String.Format( "Invalid executable location given for team #{0}, aborting", i ) );
                    return false;
                }
            }

            return true;
        }

        static void SendSetup()
        {
            foreach ( Team team in myTeams )
            {
                team.WriteLine( "setup" );
                team.WriteLine( "seed", GameState.Seed );
                team.WriteLine( "width", GameState.MapWidth );
                team.WriteLine( "height", GameState.MapHeight );
                team.WriteLine( "fow", GameState.FogOfWar );
                team.WriteLine( "turns", GameState.TurnLimit );
                team.WriteLine( "timeout", GameState.Timeout );
                team.WriteLine( "end" );
            }
        }

        static void StopPrograms()
        {
            LogComment( "stopping ai programs" );

            for ( int i = 0; i < myTeamCount; ++i )
                if ( myTeams[ i ] != null )
                    myTeams[ i ].StopProgram();
        }

        static void Log( params object[] values )
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

        static void LogComment( params object[] values )
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
