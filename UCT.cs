using System;
using Strategies;

namespace PolicyEvaluation
{
    public class UCT
    {
        public static double UctValue(int parentVisit, double nodeWinScore, int nodeVisit, double expParam)
        {
            if (nodeVisit == 0)
            {
                return int.MaxValue;
            }
            return ((1 - (double)nodeWinScore) / (double)nodeVisit)
                + expParam * Math.Sqrt(Math.Log(parentVisit) / (double)nodeVisit);
        }

        public static Node FindBestNodeWithUCT(Node node, double expParam)
        {
            int parentVisit = node.GetVisitCount();
            return node.GetChildArray().MaxBy(cNode => UctValue(parentVisit, cNode.GetWinScore(), 
                cNode.GetVisitCount(), expParam))!;
        }
    }
}
