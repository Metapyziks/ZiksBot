using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DumbBot
{
    class Bot
    {
        public IEnumerable<Agent> Agents
        {
            get { return GameState.Agents[ 0 ]; }
        }

        public void TakeTurn()
        {
            OnTakeTurn();

            foreach ( Agent agent in GameState.Agents[ 0 ] )
                agent.CommitMove();
            Console.WriteLine( "go" );

            foreach ( Agent agent in GameState.Agents[ 0 ] )
                agent.FinishTurn();
        }

        protected virtual void OnTakeTurn()
        {
            foreach ( Agent agent in Agents )
            {
                if ( agent.CanMove() )
                    agent.Order = Order.MoveForward;
                else
                    agent.Order = Order.TurnRight;
            }
        }
    }
}
