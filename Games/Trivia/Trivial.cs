using Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    public class PerfectStrategy : Strategy<AbstractGame<Game, int>, int>
    {
        public int Action(AbstractGame<Game, int> state)
        {
            return 3;
        }
    }

    public class TrivialGame : Game, AbstractGame<TrivialGame, int> 
    {
        int p1move, p2move;
        int turn = 1;
        public TrivialGame() { }

        public TrivialGame(int p1move, int p2move)
        {
            this.p1move = p1move;
            this.p2move = p2move;
        }

        public TrivialGame(int p1move, int p2move, int turn)
        {
            this.p1move = p1move;
            this.p2move = p2move;
            this.turn = turn;
        }

        public TrivialGame InitialState(int seed)
        {
            return new TrivialGame(0, 0);
        }

        public TrivialGame Clone()
        {
            return new TrivialGame(p1move, p2move, turn);
        }

        public int Player()
        {
            return turn;
        }

        public List<int> Actions()
        {
            if (p1move == 0 || p2move == 0)
            {
                return new List<int>() { 1,2,3 };
            }
            return new List<int>();
        }

        public void Apply(int action)
        {
            if (action < 1 || action > 3)
                throw new Exception("illegal move");

            turn = 3 - turn;

            if (p1move == 0)
                p1move = action;
            else if (p2move == 0)
                p2move = action;
            else throw new Exception("game is over");
        }

        public bool IsDone()
        {
            return p1move != 0 && p2move != 0;
        }

        public double Outcome()
        {
            if (p1move == p2move) return 0;
            return p1move > p2move ? 1000.0 : -1000.0;
        }

        public int RandomAction(Random random)
        {
            List<int> a = Actions();
            return a[random.Next(a.Count())];
        }

        public TrivialGame Result(int action)
        {
            TrivialGame state = Clone();
            state.Apply(action);
            return state;
        }

        public int Winner()
        {
            if (p1move == p2move) return 0;
            return p1move > p2move ? 1 : 2;
        }
    }
}
