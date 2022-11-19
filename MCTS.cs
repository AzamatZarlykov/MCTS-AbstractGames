using System;
using Game;
using PolicyEvaluation;

namespace Strategies
{
    interface Strategy
    {
        int Action(TicTacToe state);
    }

    public class Node
    {
        int visitCount;
        int winScore;
        TicTacToe? game;
        Node? parent;
        List<Node> childArray = new List<Node>();

        public Node() { }

        public Node(Node node)
        {
            visitCount = node.visitCount;
            winScore = node.winScore;
            game = node.game;
            parent = node.parent;
        }
        public Node(TicTacToe game, Node parent)
        {
            this.game = game;
            this.parent = parent;
        }

        public Node GetRandomChildNode(int seed)
        {
            return childArray[new Random(seed).Next(childArray.Count)];
        }

        public void IncrementVisit()
        {
            visitCount++;
        }

        public void AddScore(int ws)
        {
            winScore += ws;
        }

        public void SetGameState(TicTacToe g)
        {
            game = g;
        }

        public Node GetParent()
        {
            return parent!;
        }

        public TicTacToe GetGame()
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
    }

    class Tree
    {
        Node? root;
        public Tree() { }

        public Tree(Node root)
        {
            this.root = root;
        }

        public Node GetRoot()
        {
            return root!;
        }
    }

    class MCTS : Strategy
    {
        AbstractGame game;
        Strategy strategy;
        int limit;

        const int WINSCORE = 10;

        public MCTS(AbstractGame game, Strategy baseO, int limit)
        {
            this.limit = limit;
            this.game = game;
            this.strategy = baseO;
        }

        // Selection: guided by selection policy - UCT
        private Node SelectPromisingNode(Node root)
        {
            Node node = root;
            while (node.GetChildArray().Count != 0)
            {
                node = UCT.findBestNodeWithUCT(node);
            }
            return node;
        }

        // Expansion: grow the search tree by generating new children (possible states)
        private void ExpandNode(Node promisingNode)
        {
            List<TicTacToe> possibleStates = promisingNode.GetGame().GetAllPossibleStates();
            foreach (TicTacToe state in possibleStates)
            {
                Node newNode = new Node(state, promisingNode);
                promisingNode.GetChildArray().Add(newNode);
            }
        }

        // Simulation
        private int SimulateRandomPlayout(Node nodeToExplore)
        {

        }

        // backpropagation
        // Backpropogation
        private void Backpropogation(Node nodeToExplore, int playerNo)
        {
            Node tempNode = nodeToExplore;
            while (tempNode != null)
            {
                tempNode.IncrementVisit();
                if (tempNode.GetGame().turn == playerNo)
                {
                    tempNode.AddScore(WINSCORE);
                }
                tempNode = tempNode.GetParent();
            }
        }



        public int Action(TicTacToe state)
        {
            Tree tree = new Tree();
            Node rootNode = tree.GetRoot();

            rootNode.SetGameState(state);

            int curr = 1;
            while (curr <= limit)
            {
                Node promisingNode = SelectPromisingNode(rootNode);
                if (promisingNode.GetGame().Winner() == -1)
                {
                    ExpandNode(promisingNode);
                }
                Node nodeToExplore = promisingNode;
                if (promisingNode.GetChildArray().Count > 0)
                {
                    nodeToExplore = promisingNode.GetRandomChildNode(curr);
                }
                int playoutResult = SimulateRandomPlayout(nodeToExplore);
                Backpropogation(nodeToExplore, playoutResult);

                curr++;
            }
            return 0;
        }
    }
}
