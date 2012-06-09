using System;

namespace DumbBot
{
    // Class containing program entry point
    // Handles communicating with the game server
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

                // If we are not currently in an input group
                // then expect to be given a group name
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
                    // Check if current group has ended
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
                            case GamePhase.Setup: // Parse setup commands
                                switch ( split[ 0 ] )
                                {
                                    case "width": // Width of map
                                        GameState.MapWidth = int.Parse( split[ 1 ] );
                                        break;
                                    case "height": // Height of map
                                        GameState.MapHeight = int.Parse( split[ 1 ] );
                                        break;
                                    case "seed": // Seed for random number generation
                                        GameState.Random = new Random( int.Parse( split[ 1 ] ) );
                                        break;
                                    case "fow": // Fog of war boolean
                                        GameState.FogOfWar = bool.Parse( split[ 1 ] );
                                        break;
                                    case "vrange": // Agent view range
                                        GameState.ViewRange = float.Parse( split[ 1 ] );
                                        break;
                                    case "teams": // Number of teams
                                        GameState.TeamCount = int.Parse( split[ 1 ] );
                                        break;
                                    case "turns": // Turn limit
                                        GameState.TurnLimit = int.Parse( split[ 1 ] );
                                        break;
                                    case "timeout": // Timeout time in ms
                                        GameState.Timeout = int.Parse( split[ 1 ] );
                                        break;
                                }
                                break;
                            case GamePhase.Turn: // Parse turn commands
                                int team;
                                Position pos;
                                Direction dir;
                                switch ( split[ 0 ] )
                                {
                                    case "t": // Turn number
                                        GameState.Turn = int.Parse( split[ 1 ] );
                                        break;
                                    case "a": // Agent position and rotation
                                        team = int.Parse( split[ 1 ] );
                                        pos = new Position( int.Parse( split[ 2 ] ), int.Parse( split[ 3 ] ) );
                                        dir = Direction.Parse( split[ 4 ] );
                                        GameState.Agents[ team ].Add( new Agent( team, pos, dir ) );
                                        break;
                                    case "d": // Dead agent position and rotation
                                        team = int.Parse( split[ 1 ] );
                                        pos = new Position( int.Parse( split[ 2 ] ), int.Parse( split[ 3 ] ) );
                                        dir = Direction.Parse( split[ 4 ] );
                                        GameState.Dead[ team ].Add( new Agent( team, pos, dir ) );
                                        break;
                                    case "b": // Base position
                                        team = int.Parse( split[ 1 ] );
                                        pos = new Position( int.Parse( split[ 2 ] ), int.Parse( split[ 3 ] ) );
                                        GameState.Bases[ team ].Add( pos );
                                        break;
                                    case "p": // Energy package position
                                        pos = new Position( int.Parse( split[ 1 ] ), int.Parse( split[ 2 ] ) );
                                        GameState.Packages.Add( pos );
                                        break;
                                    case "w": // Wall position
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
