using System.Collections.Generic;
using UnityEngine;

namespace AStarHeap
{
    public class NodeHeap : IHeapItem<NodeHeap>
    {
        public enum NodeType
        {
            open,
            blocked
        }

        public NodeType _nodeType;

        public Vector3 m_vPosition;

        public int m_iGridX;
        public int m_iGridY;

        public int m_iGCost;
        public int m_iHCost;

        public NodeHeap m_Parent;

        public List<NodeHeap> _nodeNeighbourList;

        int m_iHeapIndex;

        public int m_ifCost
        {
            get
            {
                return m_iGCost + m_iHCost;
            }
        }

        public int HeapIndex
        {
            get
            {
                return m_iHeapIndex;
            }

            set
            {
                m_iHeapIndex = value;
            }
        }

        public int CompareTo(NodeHeap node)
        {
            int iComp = m_ifCost.CompareTo(node.m_ifCost);
            if (iComp == 0)
            {
                iComp = m_iHCost.CompareTo(node.m_iHCost);
            }
            return ~iComp;
        }

        public NodeHeap(bool bIsBlocked, Vector3 vPos, int x, int y)
        {
            _nodeType = NodeType.blocked;
            m_vPosition = vPos;
            m_iGridX = x;
            m_iGridY = y;
        }
    }
}