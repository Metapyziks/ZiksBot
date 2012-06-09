using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Game
{
    class Team
    {
        public readonly int ID;

        private Process myProcess;

        public List<Position> Bases;
        public List<Agent> Agents;

        public bool Eliminated;

        public bool ProgramRunning
        {
            get { return myProcess != null && !myProcess.HasExited; }
        }

        public bool HasError
        {
            get { return /*!myProcess.StandardError.EndOfStream*/ false; }
        }

        public Team( int id )
        {
            ID = id;
            Bases = new List<Position>();
            Agents = new List<Agent>();
        }

        public void StartProgram( String exePath )
        {
            Eliminated = true;
            if ( File.Exists( exePath ) )
            {
                ProcessStartInfo info = new ProcessStartInfo( exePath );
                info.CreateNoWindow = true;
                info.UseShellExecute = false;
                info.RedirectStandardInput = true;
                info.RedirectStandardOutput = true;
                info.RedirectStandardError = true;
                info.WorkingDirectory = Directory.GetCurrentDirectory();

                myProcess = Process.Start( info );
                Eliminated = false;
            }
            else
            {
                throw new FileNotFoundException( "Executable not found at " + exePath );
            }
        }

        public void SendSetup()
        {
            WriteLine( "setup" );
            WriteLine( "seed", GameState.Seed );
            WriteLine( "width", GameState.MapWidth );
            WriteLine( "height", GameState.MapHeight );
            WriteLine( "fow", GameState.FogOfWar );
            WriteLine( "turns", GameState.TurnLimit );
            WriteLine( "timeout", GameState.Timeout );
            WriteLine( "teams", GameState.TeamCount );
            WriteLine( "end" );
        }

        public void SendGameState()
        {
            WriteLine( "turn" );
            WriteLine( "t", GameState.Turn );
            foreach ( Agent agent in GameState.Dead )
                WriteLine( "d", ( agent.Team.ID - ID + GameState.TeamCount ) % GameState.TeamCount,
                    agent.Position.X, agent.Position.Y );
            foreach( Team team in GameState.Teams )
            {
                int relID = ( team.ID - ID + GameState.TeamCount ) % GameState.TeamCount;
                foreach ( Position pos in team.Bases )
                    WriteLine( "b", relID, pos.X, pos.Y );
                foreach ( Agent agent in team.Agents )
                    WriteLine( "a", relID, agent.Position.X, agent.Position.Y, agent.Direction );
            }
            foreach ( Position pos in GameState.Packages )
                WriteLine( "p", pos.X, pos.Y );
            WriteLine( "end" );
        }

        public void TakeTurn()
        {
            Program.Log( "bot", ID );

            Stopwatch timer = new Stopwatch();
            timer.Start();

            while ( timer.ElapsedMilliseconds <= GameState.Timeout )
            {
                if ( HasError )
                {
                    Program.LogComment( "=== error ===" );
                    while( HasError )
                        Program.LogComment( myProcess.StandardError.ReadLine() );
                    Eliminated = true;
                    return;
                }

                if ( myProcess.StandardOutput.EndOfStream )
                {
                    Thread.Yield();
                    continue;
                }

                String line = ReadLine().Trim().ToLower();

                Program.Log( line );

                if ( line == "go" )
                    break;

                String[] split = line.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

                if ( split.Length != 3 )
                {
                    Eliminated = true;
                    return;
                }

                int x, y;
                if ( !int.TryParse( split[ 0 ], out x ) || !int.TryParse( split[ 1 ], out y ) )
                {
                    Eliminated = true;
                    return;
                }

                String order = split[ 2 ];

                Position pos = new Position( x, y );
                Agent agent = null;

                foreach ( Agent ag in Agents )
                    if ( ag.Position.Equals( pos ) )
                        agent = ag;

                if ( agent == null )
                {
                    Eliminated = true;
                    return;
                }

                switch ( order )
                {
                    case "f":
                        agent.Order = Order.MoveForward; break;
                    case "l":
                        agent.Order = Order.TurnLeft; break;
                    case "r":
                        agent.Order = Order.TurnRight; break;
                    default:
                        Eliminated = true; break;
                }
            }
        }

        public String ReadLine()
        {
            return myProcess.StandardOutput.ReadLine();
        }

        public void WriteLine( params object[] values )
        {
            for ( int i = 0; i < values.Length; ++i )
            {
                String val = values[ i ].ToString() + ( i < values.Length - 1 ? " " : "" );
                myProcess.StandardInput.Write( val );
            }
            myProcess.StandardInput.WriteLine();
        }

        public void StopProgram()
        {
            if ( ProgramRunning )
                myProcess.Kill();
        }
    }
}
