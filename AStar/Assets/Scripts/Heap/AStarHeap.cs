using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

namespace AStarHeap {
    public class AStarHeap : MonoBehaviour
    {
        public PathGridManagerHeap _pathGridManager;

        public Transform _startTransform,
            _targetTransform;

        public void AStarPathfind(NodeHeap _startNode, NodeHeap _targetNode)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Heap<NodeHeap> openHeap = new Heap<NodeHeap>((int) _pathGridManager._gridSize.x * (int) _pathGridManager._gridSize.y),
                closedHeap = new Heap<NodeHeap>((int)_pathGridManager._gridSize.x * (int)_pathGridManager._gridSize.y);

            openHeap.Add(_startNode);

            while (openHeap.Count > 0)
            {
                NodeHeap currentNode = openHeap.RemoveFirst();
                closedHeap.Add(currentNode);

                if (currentNode == _targetNode)
                {
                    RetracePath(_startNode, _targetNode);
                    stopWatch.Stop();
                    print("AStarPathFind() completed in " + stopWatch.ElapsedMilliseconds + " milliseconds.");
                    return;
                }

                foreach (NodeHeap neighbour in currentNode._nodeNeighbourList)
                {
                    if (neighbour._nodeType == NodeHeap.NodeType.blocked || closedHeap.Contains(neighbour))
                        continue;

                    int movementCostToNeighbour = currentNode.m_iGCost + GetDistance(currentNode, neighbour);

                    if (movementCostToNeighbour < neighbour.m_iGCost ||
                        !openHeap.Contains(neighbour))
                    {
                        neighbour.m_iGCost = movementCostToNeighbour;
                        neighbour.m_iHCost = GetDistance(neighbour, _targetNode);
                        neighbour.m_Parent = currentNode;
                    }

                    if (!openHeap.Contains(neighbour))
                    {
                        openHeap.Add(neighbour);
                        openHeap.UpdateItem(neighbour);
                    }
                }
            }
        }

        public int GetDistance (NodeHeap nodeA, NodeHeap nodeB)
        {
            int distanceX = Mathf.Abs(nodeA.m_iGridX - nodeB.m_iGridX);
            int distanceY = Mathf.Abs(nodeA.m_iGridY - nodeB.m_iGridY);

            if (distanceX > distanceY)
                return 14 * distanceY + 10 * (distanceX - distanceY);

            return 14 * distanceX + 10 * (distanceY - distanceX);
        }

        private void RetracePath (NodeHeap start, NodeHeap end)
        {
            List<NodeHeap> path = new List<NodeHeap>();
            NodeHeap current = end;

            while (current != start)
            {
                path.Add(current);
                current = current.m_Parent;
            }

            path.Reverse();

            _pathGridManager._path = path;
        }

        private void Update()
        {
            AStarPathfind(_pathGridManager.GridLocator(_startTransform.position),
                _pathGridManager.GridLocator(_targetTransform.position));
        }
    }
}