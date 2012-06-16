using System;
using System.IO;

namespace Game
{
    class Program
    {
        static FileStream myLogStream;
        static StreamWriter myLogWriter;

        static void Main( string[] args )
        {
            if ( args.Length < 3 )
                return;

            using ( myLogStream = new FileStream( args[ 0 ], FileMode.Create, FileAccess.Write ) )
            {
                myLogWriter = new StreamWriter( myLogStream );

                GameState.Timeout = 1000;
                GameState.TurnLimit = 500;
                GameState.Seed = (int) DateTime.UtcNow.ToBinary();
                GameState.Random = new Random( GameState.Seed );
                GameState.FogOfWar = true;
                GameState.ViewRange = 5.0f;

                LogComment( "new game staring with properties:" );

                Log( "turns", GameState.TurnLimit );
                Log( "seed", GameState.Seed );
                Log( "fow", GameState.FogOfWar.ToString().ToLower() );
                Log( "timeout", GameState.Timeout );
                Log( "vrange", GameState.ViewRange );

                LogComment( "loading map" );

                if ( !GameState.LoadMap( args[ 1 ] ) )
                {
                    LogComment( "Error while loading map " + args[ 1 ] );
                    myLogWriter.Close();
                    return;
                }

                if ( args.Length - 2 < GameState.TeamCount )
                {
                    LogComment( "Expected " + GameState.TeamCount + " competing programs" );
                    myLogWriter.Close();
                    return;
                }

                LogComment( "starting ai programs" );
                bool started = true;

                for ( int i = 0; i < GameState.TeamCount; ++i )
                {
                    try
                    {
                        GameState.Teams[ i ].StartProgram( args[ i + 2 ] );
                    }
                    catch
                    {
                        LogComment( String.Format( "Invalid executable location given for team #{0}, aborting", i ) );
                        started = false;
                    }
                }

                if ( started )
                {
                    foreach ( Team team in GameState.Teams )
                        team.SendSetup();

                    for ( GameState.Turn = 0; GameState.Turn <= GameState.TurnLimit; ++GameState.Turn )
                    {
                        Log( "turn", GameState.Turn );

                        if ( GameState.Turn > 0 )
                        {
                            foreach ( Team team in GameState.Teams )
                            {
                                if ( !team.Eliminated )
                                {
                                    team.SendGameState();
                                    team.TakeTurn();

                                    if ( team.Eliminated )
                                    {
                                        Log( "# team", team.ID, "eliminated - timeout" );
                                        team.WriteLine( "done" );
                                    }
                                }
                            }
                        }

                        Agent[ , ] agentGrid = new Agent[ GameState.MapWidth, GameState.MapHeight ];

                        foreach ( Team team in GameState.Teams )
                        {
                            foreach ( Agent agent in team.Agents )
                            {
                                if ( !team.Eliminated )
                                    agent.FinishTurn();

                                int x = agent.Position.X; int y = agent.Position.Y;

                                if ( agentGrid[ x, y ] != null )
                                {
                                    agentGrid[ x, y ].Dead = agent.Dead = true;
                                }
                                else
                                {
                                    agentGrid[ x, y ] = agent;
                                }
                            }
                        }

                        foreach ( Team team in GameState.Teams )
                        {
                            foreach ( Agent agent in team.Agents )
                            {
                                Position stabPos = agent.Position + agent.Direction;
                                Agent victim = agentGrid[ stabPos.X, stabPos.Y ];
                                if ( victim != null && victim.Team != agent.Team )
                                    victim.Dead = true;
                            }
                        }

                        int liveCount = 0;
                        int activeTeams = 0;
                        foreach ( Team team in GameState.Teams )
                        {
                            for ( int i = team.Agents.Count - 1; i >= 0; --i )
                            {
                                Agent agent = team.Agents[ i ];
                                if ( agent.Dead )
                                {
                                    Log( "d", agent.ID, agent.Team.ID, agent.Position.X, agent.Position.Y, agent.Direction );
                                    team.Agents.RemoveAt( i );
                                    GameState.Dead.Add( agent );
                                }
                                else
                                {
                                    ++liveCount;
                                    Log( "a", agent.ID, agent.Team.ID, agent.Position.X, agent.Position.Y, agent.Direction );
                                }
                            }

                            if ( !team.Eliminated )
                            {
                                if ( team.Agents.Count == 0 )
                                {
                                    team.Eliminated = true;
                                    Log( "# bot", team.ID, "eliminated - defeated" );
                                    team.WriteLine( "done" );
                                }
                                else
                                    ++ activeTeams;
                            }
                        }

                        bool[ , ] packageGrid = new bool[ GameState.MapWidth, GameState.MapHeight ];

                        foreach ( Position pos in GameState.Packages )
                            packageGrid[ pos.X, pos.Y ] = true;

                        for( int i = GameState.Packages.Count - 1; i >= 0; --i )
                        {
                            Position pos = GameState.Packages[ i ];
                            Position newPos = new Position();
                            bool canMove = false;
                            Agent agent = agentGrid[ pos.X, pos.Y ];
                            if ( agent != null )
                            {
                                GameState.Packages.RemoveAt( i );

                                if ( !agent.Dead )
                                {
                                    newPos = agent.SpikePos;
                                    canMove = true;
                                }
                            }
                            else
                            {
                                foreach ( Direction dir in Direction.All )
                                {
                                    Position p = pos + dir;
                                    Agent ag = agentGrid[ p.X, p.Y ];
                                    if ( ag != null && ag.LastSpikePos.Equals( pos ) &&
                                        !ag.SpikePos.Equals( ag.LastSpikePos ) )
                                    {
                                        if ( agent != null )
                                        {
                                            agent = null;
                                            break;
                                        }

                                        agent = ag;
                                    }
                                }

                                if ( agent != null )
                                {
                                    GameState.Packages.RemoveAt( i );
                                    newPos = agent.SpikePos;
                                    canMove = true;
                                }
                            }

                            if ( canMove )
                            {
                                if ( GameState.Map[ newPos.X, newPos.Y ] == Tile.Empty &&
                                    !packageGrid[ newPos.X, newPos.Y ] &&
                                    agentGrid[ newPos.X, newPos.Y ] == null )
                                {
                                    bool spawned = false;
                                    foreach ( Team team in GameState.Teams )
                                    {
                                        foreach ( Position bPos in team.Bases )
                                        {
                                            if ( bPos.Equals( newPos ) )
                                            {
                                                spawned = true;
                                                Agent newAgent = new Agent( team, newPos, agent.Direction );
                                                team.Agents.Add( newAgent );
                                                ++liveCount;
                                                Log( "a", newAgent.ID, team.ID, newPos.X, newPos.Y, agent.Direction );
                                                break;
                                            }
                                        }
                                        if ( spawned )
                                            break;
                                    }

                                    if ( !spawned )
                                        GameState.Packages.Add( newPos );
                                }
                            }
                        }

                        if ( activeTeams > 1 && liveCount + GameState.Packages.Count + GameState.TeamCount <=
                            Math.Max( GameState.TeamCount * 2, ( GameState.MapWidth * GameState.MapHeight ) / 16 ) )
                        {
                            int tries = 0;
                            while ( tries++ < 256 )
                            {
                                Position offset = new Position(
                                    GameState.Random.Next( GameState.MapWidth ),
                                    GameState.Random.Next( GameState.MapHeight )
                                );

                                bool placed = false;
                                foreach ( Team team in GameState.Teams )
                                {
                                    Position pos = ( team.InitialBasePos + offset * team.InitialBaseDir ).Wrap();
                                    bool validPos = true;

                                    foreach ( Team t in GameState.Teams )
                                    {
                                        foreach ( Position b in t.Bases )
                                        {
                                            if ( b.BestVector( pos ).LengthManhattan < 2 )
                                            {
                                                validPos = false;
                                                break;
                                            }
                                        }
                                        if ( !validPos )
                                            break;
                                    }
                                    if ( !validPos )
                                        break;

                                    if ( GameState.Map[ pos.X, pos.Y ] == Tile.Wall )
                                        break;

                                    if ( agentGrid[ pos.X, pos.Y ] != null )
                                        continue;

                                    placed = true;
                                    GameState.Packages.Add( pos );
                                }

                                if ( placed )
                                    break;
                            }
                        }

                        foreach ( Position pos in GameState.Packages )
                            Log( "p", pos.X, pos.Y );

                        if ( activeTeams <= 1 )
                            break;
                    }
                }

                LogComment( "stopping ai programs" );

                for ( int i = 0; i < GameState.TeamCount; ++i )
                    if ( GameState.Teams[ i ] != null )
                        GameState.Teams[ i ].StopProgram();

                myLogWriter.Close();
            }
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
            myLogWriter.Write( ";" );
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
            myLogWriter.Write( ";" );
        }
    }
}
