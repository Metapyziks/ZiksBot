using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Game
{
    class Team
    {
        public readonly int ID;

        private Process myProcess;
        private bool[,] myKnown;
        private bool[,] myVisible;

        public Position InitialBasePos;
        public Direction InitialBaseDir;

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

            myKnown = new bool[ GameState.MapWidth, GameState.MapHeight ];
            myVisible = new bool[ GameState.MapWidth, GameState.MapHeight ];
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
            WriteLine( "fow", GameState.FogOfWar.ToString().ToLower() );
            if( GameState.FogOfWar )
                WriteLine( "vrange", GameState.ViewRange );
            WriteLine( "turns", GameState.TurnLimit );
            WriteLine( "timeout", GameState.Timeout );
            WriteLine( "teams", GameState.TeamCount );
            WriteLine( "end" );
        }

        private void FindVisibility()
        {
            for ( int x = 0; x < GameState.MapWidth; ++x )
                for ( int y = 0; y < GameState.MapHeight; ++y )
                    myVisible[ x, y ] = false;

            foreach ( Agent agent in Agents )
            {
                int xmin = (int) Math.Floor( agent.Position.X - GameState.ViewRange );
                int xmax = (int) Math.Ceiling( agent.Position.X + GameState.ViewRange );
                int ymin = (int) Math.Floor( agent.Position.Y - GameState.ViewRange );
                int ymax = (int) Math.Ceiling( agent.Position.Y + GameState.ViewRange );

                float squared = GameState.ViewRange * GameState.ViewRange;

                for ( int x = xmin; x <= xmax; ++x )
                {
                    for ( int y = ymin; y <= ymax; ++y )
                    {
                        Position pos = new Position( x, y ).Wrap();
                        if ( agent.Position.BestVector( pos ).LengthSquared <= squared )
                            myVisible[ pos.X, pos.Y ] = true;
                    }
                }
            }
        }

        public void SendGameState()
        {
            FindVisibility();

            WriteLine( "turn" );

            foreach ( Team team in GameState.Teams )
            {
                int relID = ( team.ID - ID + GameState.TeamCount ) % GameState.TeamCount;
                foreach ( Position pos in team.Bases.Where( x => myVisible[ x.X, x.Y ] && !myKnown[ x.X, x.Y ] ) )
                    WriteLine( "b", relID, pos.X, pos.Y );
            }

            for ( int x = 0; x < GameState.MapWidth; ++x )
            {
                for ( int y = 0; y < GameState.MapHeight; ++y )
                {
                    if ( myVisible[ x, y ] && !myKnown[ x, y ] )
                    {
                        myKnown[ x, y ] = true;

                        if( GameState.Map[ x, y ] == Tile.Wall )
                            WriteLine( "w", x, y );
                    }
                }
            }

            WriteLine( "t", GameState.Turn + 1 );
            foreach ( Agent agent in GameState.Dead.Where( x => x.Team == this ||
                myVisible[ x.Position.X, x.Position.Y ] ) )
            {
                WriteLine( "d", ( agent.Team.ID - ID + GameState.TeamCount ) % GameState.TeamCount,
                    agent.Position.X, agent.Position.Y );
            }
            foreach( Team team in GameState.Teams )
            {
                int relID = ( team.ID - ID + GameState.TeamCount ) % GameState.TeamCount;
                foreach ( Agent agent in team.Agents.Where( x => x.Team == this ||
                        myVisible[ x.Position.X, x.Position.Y ] ) )
                    WriteLine( "a", relID, agent.Position.X, agent.Position.Y, agent.Direction );
            }
            foreach ( Position pos in GameState.Packages.Where( x => myVisible[ x.X, x.Y ] ) )
                WriteLine( "p", pos.X, pos.Y );
            WriteLine( "end" );
        }

        public void TakeTurn()
        {
            Program.Log( "team", ID );

            Stopwatch timer = new Stopwatch();
            timer.Start();

            while ( true )
            {
                if ( timer.ElapsedMilliseconds > GameState.Timeout )
                {
                    Program.LogComment( "=== timeout ===" );
                    Eliminated = true;
                    return;
                }

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

                if ( line == "go" )
                {
                    Program.Log( "go" );
                    break;
                }

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
                        agent.Order = Order.MoveForward;
                        Program.Log( string.Format( "o {0} {1} {2}", agent.ID, agent.Team.ID, order ) );
                    break;
                    case "l":
                        agent.Order = Order.TurnLeft;
                        Program.Log( string.Format( "o {0} {1} {2}", agent.ID, agent.Team.ID, order ) );
                        break;
                    case "r":
                        agent.Order = Order.TurnRight;
                        Program.Log( string.Format( "o {0} {1} {2}", agent.ID, agent.Team.ID, order ) );
                        break;
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
