using System;
using Games;

namespace Strategies
{
    public class BasicStrategy : Strategy<AbstractGame<Game, int>, int>
    {
        private Random random;
        public BasicStrategy(int seed)
        {
            this.random = new Random(seed);
        }

        public int Action(AbstractGame<Game, int> s)
        {
            TicTacToe t = (TicTacToe)s.Clone();

            for (int check = 0; check < 2; ++check)
            {
                int player = check == 0 ? t.Player() : 3 - t.Player();

                for (int x = 0; x < 3; ++x)
                {
                    for (int y = 0; y < 3; ++y)
                    {
                        if (t.board[x][y] == 0)
                        {
                            t.board[x][y] = player;
                            t.CheckWin();
                            if (t.Winner() == player)
                            {
                                return x + 3 * y;
                            }
                            t.board[x][y] = 0;
                        }
                    }
                }
            }
            return s.RandomAction(random);
        }
    }
}
