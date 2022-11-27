using System;
using Games;
using Strategies;

namespace Search
{

    public class GameParameters
    {
        public string? Game { get; set; }
        public int TotalGames { get; set; }
        public int Seed { get; set; }
        public string[] Strategies { get; set; } = new string[2];
    }

    class Controller
    {
        private int draws;
        private int[] gamesWon;

        List<Strategy<AbstractGame<Game,int>, int>> strategies = new List<Strategy<AbstractGame<Game, int>, int>>();

        public Controller()
        {
            gamesWon = new int[2];
        }


        private Strategy<AbstractGame<Game, int>, int> GetStrategyType(string strategy, int seed)
        {
            string[] type_param = strategy.Split(':');
            string name = type_param[0];

            switch (name)
            {
                case "basic":
                    return new BasicStrategy(seed);
                case "perfect":
                    return new PerfectStrategy();
                case "mcts":
                    int limit = int.Parse(type_param[1]);
                    return new MCTS<int>(seed, limit);
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

        private void HandleEndGameResults(AbstractGame<Game, int> game, string[] strategies, int gameIndex)
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
                var game = parameters.Game == "tictactoe" ? (AbstractGame<Game, int>)new TicTacToe() : 
                    new TrivialGame();


                InitializeStrategies(parameters);

                while (!game.IsDone())
                {
                    int turn = game.Player();
                    int action = strategies[turn - 1].Action(game);
                    // Console.WriteLine($"Turn: {turn}; Action: {action}");
                    game.Apply(action);
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
            // $ dotnet run trivial mcts:100 basic -seed 22 -games 100
            // $ dotnet run tictactoe mcts:100 basic -seed 22 -games 100

            var parameters = new GameParameters()
            {
                Game = args[0],
                TotalGames = int.Parse(args[6]),
                Seed = int.Parse(args[4]),
                Strategies = new string[2] { args[1], args[2] }
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