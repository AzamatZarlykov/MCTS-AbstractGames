using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    public class TicTacToe : Game, AbstractGame<TicTacToe, int>
    {
        // properties
        public int[][] board = new int[3][];
        int moves = 0;
        int turn = 1;
        int winner = -1;
        int win_x, win_y, win_dx, win_dy;
        // constructors
        public TicTacToe() 
        {
            for (int i = 0; i < 3; i++)
            {
                board[i] = new int[3];
            }
        }

        public TicTacToe InitialState(int seed)
        {
            return new TicTacToe();
        }

        public TicTacToe Clone()
        {
            //TicTacToe t = new TicTacToe(board);
            TicTacToe t = new TicTacToe();

            t.moves = moves;
            t.turn = turn;
            t.winner = winner;

            for (int i = 0; i < 3; ++i)
            {
                t.board[i] = (int[])board[i].Clone();
            }

            return t;
        }

        public int Player()
        {
            return turn;
        }

        public List<int> Actions()
        {
            List<int> r = new List<int>();

            if (winner == -1)
                for (int x = 0; x < 3; ++x)
                    for (int y = 0; y < 3; ++y)
                        if (board[x][y] == 0)
                            r.Add(x + 3 * y);

            return r;
        }

        public void Apply(int action)
        {
            if (!Move(action % 3, action / 3)) {
                throw new Exception("Illegal Move");
            }
        }

        public bool IsDone()
        {
            return winner != -1;
        }

        public double Outcome()
        {
            if (winner == 0) return 0;
            return winner == 1 ? 1000.0 : -1000.0;
        }

        public int RandomAction(Random random)
        {
            List<int> a = Actions();
            return a[random.Next(a.Count())];
        }

        public TicTacToe Result(int action)
        {
            TicTacToe s = Clone();
            s.Apply(action);
            return s;
        }

        bool Win(int x, int y, int dx, int dy)
        {
            if (board[x][y] > 0 &&
                board[x][y] == board[x + dx][y + dy] &&
                board[x][y] == board[x + 2 * dx][y + 2 * dy])
            {

                winner = board[x][y];
                win_x = x; win_y = y;
                win_dx = dx; win_dy = dy;
                return true;
            }
            else return false;
        }

        public void CheckWin()
        {
            for (int i = 0; i < 3; ++i)
                if (Win(i, 0, 0, 1) || Win(0, i, 1, 0))
                    return;

            if (Win(0, 0, 1, 1) || Win(0, 2, 1, -1))
                return;

            if (moves == 9)
                winner = 0;
        }

        bool Move(int x, int y)
        {
            if (winner >= 0 || x < 0 || x >= 3 || y < 0 || y >= 3 || board[x][y] != 0)
                return false;

            board[x][y] = turn;
            moves += 1;
            CheckWin();

            turn = 3 - turn;
            return true;
        }

        public int Winner() { return winner; }

        char AsChar(int i)
        {
            switch (i)
            {
                case 0: return '.';
                case 1: return 'X';
                case 2: return 'O';
                default: throw new Exception();
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int x = 0; x < 3; ++x)
            {
                for (int y = 0; y < 3; ++y)
                    sb.Append($"{AsChar(board[x][y])}");
                sb.Append("\n");
            }

            return sb.ToString();
        }

        public List<TicTacToe> GetAllPossibleStates(List<int> possibleActions)
        {
            List<TicTacToe> result = new List<TicTacToe>();

            foreach (int action in possibleActions)
            {
                result.Add(Result(action));
            }
            return result;
        }
    }
}
