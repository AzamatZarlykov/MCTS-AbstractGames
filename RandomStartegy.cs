using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Strategies;

namespace Games
{
    public class RandomStartegy : Strategy<AbstractGame<Game, int>, int>
    {
        private Random random;
        public RandomStartegy(Random r)
        {
            this.random = r;
        }

        public int Action(AbstractGame<Game, int> s)
        {
            return s.RandomAction(random);
        }
    }
}
