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
        List<Strategy> strategies = new List<Strategy>();

        public Controller()
        {

        }


        private Strategy GetStrategyType(string strategy, int seed)
        {
            string[] type_param = strategy.Split(':');
            string name = type_param[0];

            switch (name)
            {
                case "basic":
                    return new BasicStrategy();
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
                }

                /*                game.Initialize(i, gParam.Agents);
                                InitializeAgents(i);

                                while (game.gameStatus == GameStatus.GameInProcess)
                                {
                                    int turn = game.GetTurn();

                                    totalMoves[turn]++;
                                    timers[turn].Start();

                                    var gw = new GameView(game, turn);
                                    Card? card = agents[turn].Move(gw);
                                    game.Move(card);
                                    timers[turn].Stop();
                                }
                                HandleEndGameResult(game, i);*/
            }
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