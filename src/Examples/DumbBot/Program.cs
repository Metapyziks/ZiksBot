using System;

namespace DumbBot
{
    class Program
    {
        enum GamePhase
        {
            None,   // Waiting for a phase type
            Setup,  // Receiving game properties
            Turn,   // Receiving turn state information
            Done,   // Game ended or bot has been eliminated
        }

        static GamePhase myPhase = GamePhase.None;

        static void Main( string[] args )
        {
            Bot bot = new Bot();

            while ( myPhase != GamePhase.Done )
            {
                // Read a line, remove leading or trailing white
                // space and make all letters lower case
                String line = Console.ReadLine().Trim().ToLower();

                // If line is empty or commented, skip
                if ( line.Length == 0 || line[ 0 ] == '#' )
                    continue;

                if( myPhase == GamePhase.None )
                {
                    switch ( line )
                    {
                        case "setup":
                            myPhase = GamePhase.Setup;
                            break;
                        case "turn":
                            myPhase = GamePhase.Turn;
                            GameState.PreTurn();
                            break;
                        case "done":
                            myPhase = GamePhase.Done;
                            break;
                        default:
                            continue;
                    }
                }
                else
                {
                    if ( line == "end" )
                    {
                        switch ( myPhase )
                        {
                            case GamePhase.Setup:
                                GameState.PostSetup();
                                break;
                            case GamePhase.Turn:
                                bot.TakeTurn();
                                break;
                        }

                        myPhase = GamePhase.None;
                    }
                    else
                    {
                        String[] split = line.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

                        switch ( myPhase )
                        {
                            case GamePhase.Setup:
                                switch ( split[ 0 ] )
                                {
                                    case "width":
                                        GameState.MapWidth = int.Parse( split[ 1 ] );
                                        break;
                                    case "height":
                                        GameState.MapHeight = int.Parse( split[ 1 ] );
                                        break;
                                    case "seed":
                                        GameState.Random = new Random( int.Parse( split[ 1 ] ) );
                                        break;
                                    case "fow":
                                        GameState.FogOfWar = bool.Parse( split[ 1 ] );
                                        break;
                                    case "vrange":
                                        GameState.ViewRange = float.Parse( split[ 1 ] );
                                        break;
                                    case "teams":
                                        GameState.TeamCount = int.Parse( split[ 1 ] );
                                        break;
                                    case "turns":
                                        GameState.TurnLimit = int.Parse( split[ 1 ] );
                                        break;
                                    case "timeout":
                                        GameState.Timeout = int.Parse( split[ 1 ] );
                                        break;
                                }
                                break;
                            case GamePhase.Turn:
                                int team;
                                bool found;
                                Position pos;
                                Direction dir;
                                switch ( split[ 0 ] )
                                {
                                    case "t":
                                        GameState.Turn = int.Parse( split[ 1 ] );
                                        break;
                                    case "a":
                                        team = int.Parse( split[ 1 ] );
                                        pos = new Position( int.Parse( split[ 2 ] ), int.Parse( split[ 3 ] ) );
                                        dir = Direction.Parse( split[ 4 ] );
                                        if ( team == 0 )
                                        {
                                            found = false;
                                            foreach ( Agent agent in GameState.Agents[ 0 ] )
                                            {
                                                if ( agent.Position.Equals( pos ) )
                                                {
                                                    found = true;
                                                    if ( !agent.Confirmed )
                                                        agent.Confirmed = true;
                                                    else
                                                        throw new Exception( "Duplicate agent detected" );

                                                    break;
                                                }
                                            }

                                            if ( !found )
                                                GameState.Agents[ 0 ].Add( new Agent( team, pos, dir ) );
                                        }
                                        else
                                        {
                                            GameState.Agents[ team ].Add( new Agent( team, pos, dir ) );
                                        }
                                        break;
                                    case "d":
                                        team = int.Parse( split[ 1 ] );
                                        pos = new Position( int.Parse( split[ 2 ] ), int.Parse( split[ 3 ] ) );
                                        dir = Direction.Parse( split[ 4 ] );
                                        GameState.Dead[ team ].Add( new Agent( team, pos, dir ) );
                                        break;
                                    case "b":
                                        team = int.Parse( split[ 1 ] );
                                        pos = new Position( int.Parse( split[ 2 ] ), int.Parse( split[ 3 ] ) );
                                        GameState.Bases[ team ].Add( pos );
                                        break;
                                    case "p":
                                        pos = new Position( int.Parse( split[ 1 ] ), int.Parse( split[ 2 ] ) );
                                        GameState.Packages.Add( pos );
                                        break;
                                    case "w":
                                        pos = new Position( int.Parse( split[ 1 ] ), int.Parse( split[ 2 ] ) );
                                        GameState.Map[ pos.X, pos.Y ] = Tile.Wall;
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
        }
    }
}
