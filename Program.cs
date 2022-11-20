using System;
using Game;
using Strategies;

namespace Search
{

    public class GameParameters
    {
        public int TotalGames { get; set; }
        public int Seed { get; set; }
        public string[] Strategies { get; set; } = new string[2];
    }

    class Controller
    {
        private int draws;
        private int[] gamesWon;

        List<Strategy> strategies = new List<Strategy>();

        public Controller()
        {
            gamesWon = new int[2];
        }


        private Strategy GetStrategyType(string strategy, int seed)
        {
            string[] type_param = strategy.Split(':');
            string name = type_param[0];

            switch (name)
            {
                case "basic":
                    return new BasicStrategy(seed);
                case "mcts":
                    int limit = int.Parse(type_param[1]);
                    return new MCTS(seed, limit);
                default:
                    throw new Exception("unknown strategy");
            }
        }

        private void InitializeStrategies(GameParameters parameters)
        {
            foreach (string strategy in parameters.Strategies)
            {
                strategies.Add(GetStrategyType(strategy, parameters.Seed));
            }
        }

        private void HandleEndGameResults(TicTacToe game, string[] strategies, int gameIndex)
        {
            Console.Write("Game " + gameIndex + ": ");

            int result = game.Winner();

            if (result == 0)
            {
                Console.WriteLine("Draw");
                draws++;
                return;
            }
            result = result == 1 ? 0 : 1;
            Console.WriteLine($"Strategy {result + 1} ({strategies[result]}) won");
            gamesWon[result]++;
        }

        private void DisplayWinRate(GameParameters p)
        {
            Console.WriteLine($"Draw rate: {(100 * (double)draws / p.TotalGames):f1}%");
            for (int i = 0; i < 2; ++i)
                Console.WriteLine($"Strategy {i + 1} ({p.Strategies[i]}) won " +
                    $"{gamesWon[i]} / {p.TotalGames} games " +
                    $"({(100 * (double)gamesWon[i] / p.TotalGames):f1}%)");

            Console.WriteLine();
        }

        private void PrintStats(GameParameters parameters)
        {
            Console.WriteLine("\n==== STATISTICS ====\n");
            Console.WriteLine($"Total Games Played: {parameters.TotalGames}");

            DisplayWinRate(parameters);
        }

        public void Run(GameParameters parameters)
        {
            Console.WriteLine("==== RUNNING ====\n");
            for (int i = 1; i <= parameters.TotalGames; i++)
            {
                TicTacToe game = new TicTacToe();
                InitializeStrategies(parameters);

                while (!game.IsDone())
                {
                    int turn = game.turn;
                    // Console.WriteLine($"Turn: {turn}");
                    int action = strategies[turn - 1].Action(game);
                    // Console.WriteLine($"Action: X: {action % 3}; Y: {action / 3}");
                    game.Move(action);

                    // Console.WriteLine(game);
                }
                HandleEndGameResults(game, parameters.Strategies, i);
            }
            PrintStats(parameters);
            Console.WriteLine("==== FINISHED ====");
        }
    }

    public class Program
    {

        private static GameParameters ParseParameters(string[] args)
        {
        // $ ./tictactoe mcts:100 basic -seed 22 -games 100
            var parameters = new GameParameters()
            {
                TotalGames = int.Parse(args[5]),
                Seed = int.Parse(args[3]),
                Strategies = new string[2] { args[0], args[1] }
            };

            return parameters;
        }


        public static void Main(string[] args)
        {
            GameParameters parameters = ParseParameters(args);

            Controller controller = new Controller();
            controller.Run(parameters);
        }
    }
}