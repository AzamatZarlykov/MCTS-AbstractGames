using System;
using Games;
using PolicyEvaluation;

namespace Strategies
{
    public interface Strategy<S, A>
    {
        A Action(S state);
    }

    public class Node<A>
    {
        int visitCount;
        double winScore;

        A? lastAction;    // last action (best move chosen to return)

        AbstractGame<Game, A>? game;

        Node<A>? parent;
        List<Node<A>> childArray = new List<Node<A>>();

        List<A>? allActions;
        public Node() { }
        public Node(AbstractGame<Game, A> game, Node<A> parent, A action)
        {
            this.game = game;
            this.parent = parent;
            this.lastAction = action;
        }

        public void SetAllActions(List<A> actions)
        {
            allActions = new List<A>(actions);
        }

        public List<A> GetAllActions()
        {
            return allActions!;
        }

        public A GetLastAction()
        {
            return this.lastAction!;
        }

        public void IncrementVisit()
        {
            visitCount++;
        }

        public void AddScore(double ws)
        {
            winScore += ws;
        }

        public void SetGameState(AbstractGame<Game, A> g)
        {
            game = g;
        }

        public Node<A> GetParent()
        {
            return parent!;
        }

        public AbstractGame<Game, A> GetGame()
        {
            return game!;
        }

        public int GetVisitCount()
        {
            return visitCount;
        }

        public double GetWinScore()
        {
            return winScore;
        }

        public List<Node<A>> GetChildArray()
        {
            return childArray;
        }

        public bool IsNotFullyExpanded()
        {
            return allActions!.Count > 0;
        }

        public List<A> GetExpandedActions()
        {
            List<A> actions = new List<A>();

            foreach (Node<A> childNode in GetChildArray())
            {
                actions.Add(childNode.GetLastAction());
            }

            return actions;
        }

        public Node<A> GetChildWithMinScore()
        {
            return childArray.MinBy(child => child.winScore)!;
        }

        public Node<A> Expand()
        {
            // Get the untried action
            A action = GetAllActions().First()!;
            GetAllActions().RemoveAt(0);

            AbstractGame<Game, A> state = (AbstractGame<Game, A>)game!.Result(action);

            Node<A> newNode = new Node<A>(state, this, action);
            childArray.Add(newNode);

            return newNode;
        }

        public Node<A> BestChild(double expParam)
        {
            return UCT<A>.FindBestNodeWithUCT(this, expParam);
        }

        public bool TerminalState()
        {
            // in the terminal state when game is done
            return game!.IsDone();
        }

    }

    class Tree<A>
    {
        Node<A> root;
        public Tree() 
        { 
            root = new Node<A>();
        }

        public Tree(Node<A> root)
        {
            this.root = root;
        }

        public Node<A> GetRoot()
        {
            return root!;
        }

        public void SetRoot(Node<A> node)
        {
            this.root = node;
        }
    }

    class MCTS<A> : Strategy<AbstractGame<Game, A>, A>
    {
        int limit;
        Random random;

        int WINSCORE = 1;

        public MCTS(Random r, int limit)
        {
            this.limit = limit;
            this.random = r;
        }

        private void AssignActions(Node<A> node, AbstractGame<Game, A> gameState)
        {
            if (node.GetAllActions() is null)
            {
                node.SetAllActions(gameState!.Actions());
            }
        }

        private Node<A> TreePolicy(Node<A> node)
        {
            AbstractGame<Game, A> gameState = node.GetGame();

            AssignActions(node, gameState);
            // while node is nonterminal 
            while (!node.TerminalState())
            {
                if (node.IsNotFullyExpanded())
                {
                    return node.Expand();
                }
                else
                {
                    node = node.BestChild(1.41);
                    AssignActions(node, gameState);
                }
            }
            return node;
        }

        private Strategy<AbstractGame<Game, A>,A> GetPlayoutStrategy(AbstractGame<Game, A> gameStateClone)
        {
            if (gameStateClone is TicTacToe)
            {
                return (Strategy<AbstractGame<Game, A>, A>)new BasicStrategy(random);
            }
            else
            {
                return (Strategy<AbstractGame<Game, A>, A>)new PerfectStrategy();
            }
        }

        private int DefaultPolicy(AbstractGame<Game, A> gameStateClone)
        {
            Strategy<AbstractGame<Game, A>, A> strat = GetPlayoutStrategy(gameStateClone);

            while (!gameStateClone.IsDone())
            {
                A action = strat.Action(gameStateClone);
                gameStateClone.Apply(action);
            }
            return gameStateClone.Winner();
        }

        // Backpropagation
        private void Backpropagation(Node<A> nodeToExplore, int playoutResult)
        {
            Node<A> tempNode = nodeToExplore;

            while (tempNode != null)
            {
                var game = tempNode.GetGame();
                tempNode.IncrementVisit();
                // draw
                if (playoutResult == 0)
                {
                    tempNode.AddScore(0.5);
                }
                else if (game.Player() == playoutResult)
                {
                    tempNode.AddScore(WINSCORE);
                }
                tempNode = tempNode.GetParent();
            }
        }

        public A Action(AbstractGame<Game, A> state)
        {
            Tree<A> tree = new Tree<A>();
            Node<A> rootNode = tree.GetRoot();

            rootNode.SetGameState(state);

            int curr = 1;
            while (curr <= limit)
            {
                Node<A> selectedNode = TreePolicy(rootNode);
                var game = selectedNode.GetGame();
                int playoutResult = DefaultPolicy((AbstractGame<Game, A>)game.Clone());
                Backpropagation(selectedNode, playoutResult);

                curr++;
            }

            Node<A> winnerNode = rootNode.BestChild(0);
            tree.SetRoot(winnerNode);
            return winnerNode.GetLastAction();
        }
    }
}
