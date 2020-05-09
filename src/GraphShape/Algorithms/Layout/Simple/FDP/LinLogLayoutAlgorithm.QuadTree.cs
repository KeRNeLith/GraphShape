using System;
using JetBrains.Annotations;
using QuikGraph;

namespace GraphShape.Algorithms.Layout
{
    public partial class LinLogLayoutAlgorithm<TVertex, TEdge, TGraph>
        where TEdge : IEdge<TVertex>
        where TGraph : IBidirectionalGraph<TVertex, TEdge>
    {
        private class QuadTree
        {
            public int Index { get; private set; }

            public Point Position { get; private set; }

            public double Weight { get; private set; }

            private Point _minPos;
            private Point _maxPos;

            public double Width => Math.Max(_maxPos.X - _minPos.X, _maxPos.Y - _minPos.Y);

            [NotNull, ItemCanBeNull]
            public QuadTree[] Children { get; } = new QuadTree[4];

            private const int MaxDepth = 20;

            public QuadTree(int index, Point position, double weight, Point minPos, Point maxPos)
            {
                Index = index;
                Position = position;
                Weight = weight;
                _minPos = minPos;
                _maxPos = maxPos;
            }

            public void AddNode(int nodeIndex, Point nodePos, double nodeWeight, int depth)
            {
                if (depth > MaxDepth)
                    return;

                if (Index >= 0)
                {
                    AddNode2(Index, Position, Weight, depth);
                    Index = -1;
                }

                Position = new Point(
                    (Position.X * Weight + nodePos.X * nodeWeight) / (Weight + nodeWeight),
                    (Position.Y * Weight + nodePos.Y * nodeWeight) / (Weight + nodeWeight));
                Weight += nodeWeight;

                AddNode2(nodeIndex, nodePos, nodeWeight, depth);
            }

            private void AddNode2(int nodeIndex, Point nodePos, double nodeWeight, int depth)
            {
                int childIndex = 0;
                double middleX = (_minPos.X + _maxPos.X) / 2;
                double middleY = (_minPos.Y + _maxPos.Y) / 2;

                if (nodePos.X > middleX)
                    childIndex += 1;

                if (nodePos.Y > middleY)
                    childIndex += 2;

                if (Children[childIndex] is null)
                {
                    var newMin = new Point();
                    var newMax = new Point();
                    if (nodePos.X <= middleX)
                    {
                        newMin.X = _minPos.X;
                        newMax.X = middleX;
                    }
                    else
                    {
                        newMin.X = middleX;
                        newMax.X = _maxPos.X;
                    }

                    if (nodePos.Y <= middleY)
                    {
                        newMin.Y = _minPos.Y;
                        newMax.Y = middleY;
                    }
                    else
                    {
                        newMin.Y = middleY;
                        newMax.Y = _maxPos.Y;
                    }

                    Children[childIndex] = new QuadTree(nodeIndex, nodePos, nodeWeight, newMin, newMax);
                }
                else
                {
                    Children[childIndex].AddNode(nodeIndex, nodePos, nodeWeight, depth + 1);
                }
            }

            /// <summary>
            /// Moves a node (re-compute section position by subtracting moved node position).
            /// </summary>
            public void MoveNode(Point oldPos, Point newPos, double nodeWeight)
            {
                Position += (newPos - oldPos) * (nodeWeight / Weight);

                int childIndex = 0;
                double middleX = (_minPos.X + _maxPos.X) / 2;
                double middleY = (_minPos.Y + _maxPos.Y) / 2;

                if (oldPos.X > middleX)
                    childIndex += 1;
                if (oldPos.Y > middleY)
                    childIndex += 2;

                if (Children[childIndex] != null)
                    Children[childIndex].MoveNode(oldPos, newPos, nodeWeight);
            }
        }
    }
}