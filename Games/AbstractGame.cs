using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    public interface AbstractGame<out S, A> where S : Game, new()
    {
        S InitialState(int seed);
        S Clone();
        int Player(); // which player moves next: 1 (maximizing) or 2 (minimizing)
        List<A> Actions(); // available moves in this state
        void Apply(A action); // apply action to state  
        bool IsDone(); // true if game has finished
        A RandomAction(Random random); // returns random action
        S Result(A action); // create the copy of the state and makes the move
        int Winner();    // 1 = player 1 wins; 2 = player 2 wins; 0 = draw
    }

    public class Game { }

}
