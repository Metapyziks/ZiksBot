using System;
using System.Collections.Generic;

namespace DumbBot
{
    // Use this class, or a class that extends it, to
    // hold the bulk of your AI procedures
    class Bot
    {
        public IEnumerable<Agent> Agents
        {
            get { return GameState.Agents[ 0 ]; }
        }

        // This doesn't need to be modified
        public void TakeTurn()
        {
            OnTakeTurn();

            foreach ( Agent agent in GameState.Agents[ 0 ] )
                agent.CommitMove();
            Console.WriteLine( "go" );

            foreach ( Agent agent in GameState.Agents[ 0 ] )
                agent.FinishTurn();
        }

        // Add your logic here
        protected virtual void OnTakeTurn()
        {
            foreach ( Agent agent in Agents )
            {
                // If agent can move forward, do so
                if ( agent.CanMove() )
                    agent.Order = Order.MoveForward;
                // Otherwise, turn randomly
                else if( GameState.Random.NextDouble() < 0.5 )
                    agent.Order = Order.TurnRight;
                else
                    agent.Order = Order.TurnLeft;
            }
        }
    }
}
