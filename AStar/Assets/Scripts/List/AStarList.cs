using UnityEngine;
using System.Collections.Generic;

namespace AStarList {
    public class AStarList : MonoBehaviour
    {
        public PathGridManagerList _pathGridManager;

        public Transform _startTransform,
            _targetTransform;

        public void AStarPathfind(NodeList _startNode, NodeList _targetNode)
        {
            List<NodeList> openList = new List<NodeList>(),
                closedList = new List<NodeList>();

            openList.Add(_startNode);

            while (openList.Count > 0)
            {
                NodeList currentNode = openList[0];
                for (int i = 1; i < openList.Count; i++)
                {
                    if (openList[i].m_iGCost < currentNode.m_iGCost ||
                        openList[i].m_iGCost == currentNode.m_iGCost && openList[i].m_iHCost < currentNode.m_iHCost)
                    {
                        currentNode = openList[i];
                    }
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                if (currentNode == _targetNode)
                {
                    RetracePath(_startNode, _targetNode);
                    return;
                }

                foreach (NodeList neighbour in currentNode._nodeNeighbourList)
                {
                    if (neighbour._nodeType == NodeList.NodeType.blocked || closedList.Contains(neighbour))
                        continue;

                    int movementCostToNeighbour = currentNode.m_iGCost + GetDistance(currentNode, neighbour);

                    if (movementCostToNeighbour < neighbour.m_iGCost ||
                        !openList.Contains(neighbour))
                    {
                        neighbour.m_iGCost = movementCostToNeighbour;
                        neighbour.m_iHCost = GetDistance(neighbour, _targetNode);
                        neighbour.m_Parent = currentNode;
                    }

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
        }

        public int GetDistance (NodeList nodeA, NodeList nodeB)
        {
            int distanceX = Mathf.Abs(nodeA.m_iGridX - nodeB.m_iGridX);
            int distanceY = Mathf.Abs(nodeA.m_iGridY - nodeB.m_iGridY);

            if (distanceX > distanceY)
                return 14 * distanceY + 10 * (distanceX - distanceY);

            return 14 * distanceX + 10 * (distanceY - distanceX);
        }

        private void RetracePath (NodeList start, NodeList end)
        {
            List<NodeList> path = new List<NodeList>();
            NodeList current = end;

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