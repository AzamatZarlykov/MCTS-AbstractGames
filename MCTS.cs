using System;
using Game;
using PolicyEvaluation;

namespace Strategies
{
    interface Strategy<S, A>
    {
        A Action(S state);
    }

    public class Node
    {
        int visitCount;
        int winScore;
        int lastAction = -1;    // last action (best move chosen to return)
        Trivial? game;
        Node? parent;
        List<Node> childArray = new List<Node>();
        List<int>? allActions;
        public Node() { }
        public Node(Trivial game, Node parent, int action)
        {
            this.game = game;
            this.parent = parent;
            this.lastAction = action;
        }

        public void SetAllActions(List<int> actions)
        {
            allActions = new List<int>(actions);
        }

        public List<int> GetAllActions()
        {
            return allActions!;
        }

        public int GetLastAction()
        {
            return this.lastAction;
        }

        public void IncrementVisit()
        {
            visitCount++;
        }

        public void AddScore(int ws)
        {
            winScore += ws;
        }

        public void SetGameState(Trivial g)
        {
            game = g;
        }

        public Node GetParent()
        {
            return parent!;
        }

        public Trivial GetGame()
        {
            return game!;
        }

        public int GetVisitCount()
        {
            return visitCount;
        }

        public int GetWinScore()
        {
            return winScore;
        }

        public List<Node> GetChildArray()
        {
            return childArray;
        }

        public bool IsNotFullyExpanded()
        {
            return allActions!.Count > 0 ? true : false;
        }

        public List<int> GetExpandedActions()
        {
            List<int> actions = new List<int>();

            foreach (Node childNode in GetChildArray())
            {
                actions.Add(childNode.GetLastAction());
            }

            return actions;
        }

        public Node GetChildWithMinScore()
        {
            return childArray.MinBy(child => child.winScore)!;
        }

        public Node Expand()
        {
            // Get the untried action
            int action = GetAllActions().First();
            GetAllActions().RemoveAt(0);

            Trivial state = game!.Result(action);

            Node newNode = new Node(state, this, action);
            childArray.Add(newNode);

            return newNode;
        }

        public Node BestChild(double expParam)
        {
            return UCT.FindBestNodeWithUCT(this, expParam);
        }

    }

    class Tree
    {
        Node root;
        public Tree() 
        { 
            root = new Node();
        }

        public Tree(Node root)
        {
            this.root = root;
        }

        public Node GetRoot()
        {
            return root!;
        }

        public void SetRoot(Node node)
        {
            this.root = node;
        }
    }

    class MCTS : Strategy
    {
        int limit;
        Random random;

        int WINSCORE = 1;

        public MCTS(int seed, int limit)
        {
            this.limit = limit;
            this.random = new Random(seed);
        }

        // Backpropogation
        private void Backpropogation(Node nodeToExplore, int playoutResult)
        {
            Node tempNode = nodeToExplore;
            while (tempNode != null)
            {
                tempNode.IncrementVisit();
                if (tempNode.GetGame().Player() == playoutResult)
                {
                    tempNode.AddScore(WINSCORE);
                }
                tempNode = tempNode.GetParent();
            }
        }

        private Node TreePolicy(Node node)
        {
            Trivial gameState = node.GetGame();

            if (node.GetAllActions() is null)
            {
                node.SetAllActions(gameState.GetAllActions());
            }

            if (node.IsNotFullyExpanded())
            {
                return node.Expand();
            }
            else
            {
                return node.BestChild(1.41);
            }
        }

        private int DefaultPolicy(Trivial gameStateClone)
        {
            while (!gameStateClone.IsDone())
            {

                int action = gameStateClone.RandomAction(random);
                gameStateClone.Move(action);
            }
            return gameStateClone.Winner();
        }

        public int Action(Trivial state)
        {
            Tree tree = new Tree();
            Node rootNode = tree.GetRoot();

            rootNode.SetGameState(state);

            int curr = 1;
            while (curr <= limit)
            {
                Node selectedNode = TreePolicy(rootNode);
                int playoutResult = DefaultPolicy(selectedNode.GetGame().Clone());
                Backpropogation(selectedNode, playoutResult);

                curr++;
            }
            Node winnerNode = rootNode.BestChild(0);
            tree.SetRoot(winnerNode);
            return winnerNode.GetLastAction();
        }
    }
}
