using System;
using Strategies;

namespace PolicyEvaluation
{
    public class UCT
    {
        public static double uctValue(int parentVisit, double nodeWinScore, int nodeVisit)
        {
            if (nodeVisit == 0)
            {
                return Int32.MaxValue;
            }
            return ((double)nodeWinScore / (double)nodeVisit)
                + 1.41 * Math.Sqrt(Math.Log(parentVisit) / (double)nodeVisit);
        }

        public static Node findBestNodeWithUCT(Node node)
        {
            int parentVisit = node.GetVisitCount();
            return node.GetChildArray().MaxBy(cNode => uctValue(parentVisit, cNode.GetWinScore(), cNode.GetVisitCount()))!;
        }
    }
}
