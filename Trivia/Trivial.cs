using Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class PerfectStrategy : Strategy<Trivial, int>
    {
        public int Action(Trivial state)
        {
            return 3;
        }
    }

    public class Trivial : AbstractGame<Trivial, int>
    {
        int p1Move, p2Move;

        public Trivial() { }

        public Trivial(int p1Move, int p2Move)
        {
            this.p1Move = p1Move;
            this.p2Move = p2Move;
        }

        public Trivial Clone()
        {
            return (Trivial)(this).MemberwiseClone();
        }

        public List<int> GetAllActions()
        {
            if (p1Move == 0 || p2Move == 0)
            {
                return new List<int>() { 1,2,3 };
            }
            return new List<int>();
        }

        public int RandomAction(Random random)
        {
            List<int> a = GetAllActions();
            return a[random.Next(a.Count())];
        }

        public Trivial Result(int action)
        {
            Trivial s = Clone();
            s.Move(action);
            return s;
        }

        public int Winner()
        {
            if (p1Move == p2Move) return 0;
            return p1Move > p2Move ? 1 : 2;
        }

        public bool IsDone()
        {
            return p1Move != 0 && p2Move != 0;
        }

        public void Move(int action)
        {
            if (action < 1 || action > 3)
                throw new Exception("illegal move");
            if (p1Move == 0)
                p1Move = action;
            else if (p2Move == 0)
                p2Move = action;
            else throw new Exception("game is over");
        }

        public int Outcome()
        {
            if (p1Move > p2Move)
                return 1000;
            if (p1Move < p2Move)
                return -1000;
            return 0;
        }

        public int Player()
        {
            return p1Move == 0 ? 1 : 2;
        }
    }
}
