using System;
using Game;

namespace Strategies
{
    public class BasicStrategy : Strategy<TicTacToe, int>
    {
        private Random random;
        public BasicStrategy(int seed)
        {
            this.random = new Random(seed);
        }

        public int Action(TicTacToe s)
        {
            TicTacToe t = s.Clone();

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
