using System.Collections.Generic;
using UnityEngine;

namespace AStarList
{
    public class NodeList
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

        public NodeList m_Parent;

        public List<NodeList> _nodeNeighbourList;

        public int m_ifCost
        {
            get
            {
                return m_iGCost + m_iHCost;
            }
        }

        public NodeList(bool bIsBlocked, Vector3 vPos, int x, int y)
        {
            _nodeType = NodeType.blocked;
            m_vPosition = vPos;
            m_iGridX = x;
            m_iGridY = y;
        }
    }
}