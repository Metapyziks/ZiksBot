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

        static int myMapWidth;
        static int myMapHeight;

        static Team[] myTeams;

        static void Main( string[] args )
        {
            myTeamCount = args.Length;

            if ( myTeamCount < 2 )
                return;

            // Would load a map now, but using an empty one until then
            myMapWidth = 32;
            myMapHeight = 32;

            myLogStream = new FileStream( "game.log", FileMode.Create, FileAccess.Write );
            myLogWriter = new StreamWriter( myLogStream );
            
            myTeams = new Team[ myTeamCount ];

            for ( int i = 0; i < myTeamCount; ++i )
                myTeams[ i ] = new Team( args[ i ] );

            if ( StartPrograms() )
            {
                SendSetup();
            }

            StopPrograms();
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
            int seed = (int) DateTime.UtcNow.ToBinary();

            foreach ( Team team in myTeams )
            {
                team.WriteLine( "setup" );
                team.WriteLine( "seed", seed );
                team.WriteLine( "width", myMapWidth );
                team.WriteLine( "height", myMapHeight );
                team.WriteLine( "fow", true );
                team.WriteLine( "end" );
            }
        }

        static void StopPrograms()
        {
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
        }

        static void LogComment( params object[] values )
        {
            Log( values );
        }
    }
}
