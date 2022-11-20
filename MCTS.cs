﻿using System;
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
        int lastAction = -1;    // last action (best move chosen to return)
        TicTacToe? game;
        Node? parent;
        List<Node> childArray = new List<Node>();

        public Node() { }
        public Node(TicTacToe game, Node parent, int action)
        {
            this.game = game;
            this.parent = parent;
            this.lastAction = action;
        }

        public Node Clone()
        {
            Node clone = (Node)this.MemberwiseClone();

            clone.game = game!.Clone();
            clone.parent = parent is not null ? parent.Clone() : null;
            clone.childArray = new List<Node>(childArray);

            return clone;
        }
        
        public Node GetChildWithMaxScore()
        {
            return childArray.MaxBy(child => child.winScore)!;
        }

        public Node GetRandomChildNode(Random random)
        {
            return childArray[random.Next(childArray.Count)];
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

        const int WINSCORE = 10;

        public MCTS(int seed, int limit)
        {
            this.limit = limit;
            this.random = new Random(seed);
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
            TicTacToe gameState = promisingNode.GetGame();

            List<int> possibleActions = gameState.GetAllActions();
            List<TicTacToe> possibleStates = gameState.GetAllPossibleStates(possibleActions);

            for(int i = 0; i < possibleActions.Count; i++)
            {
                int action = possibleActions[i];
                TicTacToe state = possibleStates[i];

                Node newNode = new Node(state, promisingNode, action);
                promisingNode.GetChildArray().Add(newNode);
            }
        }

        // Simulation
        private int SimulateRandomPlayout(Node nodeToExplore)
        {
            Node tempNode = nodeToExplore.Clone();
            TicTacToe gameState = tempNode.GetGame();
            
            while(!gameState.IsDone())
            {
                
                int action = gameState.RandomAction(random); 
                gameState.Move(action);
            }
            return gameState.Winner();
        }

        // Backpropogation
        private void Backpropogation(Node nodeToExplore, int playoutResult)
        {
            Node tempNode = nodeToExplore;
            while (tempNode != null)
            {
                tempNode.IncrementVisit();
                if (tempNode.GetGame().turn == playoutResult)
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
                // Selection
                Node promisingNode = SelectPromisingNode(rootNode);
                // Expansion
                ExpandNode(promisingNode);
                
                Node nodeToExplore = promisingNode;
                if (promisingNode.GetChildArray().Count > 0)
                {
                    nodeToExplore = promisingNode.GetRandomChildNode(random);
                }
                // Simulation
                int playoutResult = SimulateRandomPlayout(nodeToExplore);
                // Backpropogation
                Backpropogation(nodeToExplore, playoutResult);
                
                curr++;
            }
            Node winnerNode = rootNode.GetChildWithMaxScore();
            tree.SetRoot(winnerNode);
            return winnerNode.GetLastAction();
        }
    }
}
