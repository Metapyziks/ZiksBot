using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DumbBot
{
    class Program
    {
        enum GamePhase
        {
            None,   // Waiting for a phase type
            Setup,  // Receiving game properties
            Map,    // Receiving map dimentions and layout
            Turn,   // Receiving turn state information
            Done,   // Game ended or bot has been eliminated
        }

        static GamePhase myPhase = GamePhase.None;

        static void Main( string[] args )
        {
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
                        case "end":
                            if ( myPhase == GamePhase.Setup && GameState.Random == null )
                                GameState.Random = new Random();

                            myPhase = GamePhase.None; break;
                        case "setup":
                            myPhase = GamePhase.Setup; break;
                        case "map":
                            myPhase = GamePhase.Map; break;
                        case "turn":
                            myPhase = GamePhase.Turn; break;
                        case "done":
                            myPhase = GamePhase.Done; break;
                        default:
                            continue;
                    }
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
                            }
                            break;
                    }
                }
            }
        }
    }
}
