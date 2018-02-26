using System.Collections.Generic;
using UnityEngine;

namespace AStarHeap
{
    public class PathGridManagerHeap : MonoBehaviour
    {
        public Transform _player,
            _target;
        public LayerMask _obstacleMask;
        public float _halfNodeWidth;
        public NodeHeap[,] _grid;
        public Vector2 _gridSize;
        private Vector3 _gridZeroZeroUpperLeftCornerPosition;

        public int GridWidth { get; private set; }
        public int GridHeight { get; private set; }

        public float NodeWidth { get; private set; }
        public float NodeHeight { get; private set; }

        public List<NodeHeap> _path;

        private void Start()
        {
            NodeWidth = _halfNodeWidth * 2f;
            NodeHeight = _halfNodeWidth * 2f;

            GridWidth = (int) (_gridSize.x / NodeWidth);
            GridHeight = (int) (_gridSize.y / NodeHeight);

            _grid = new NodeHeap[GridWidth, GridHeight];

            GenerateNodes();

            BakeNeighbours();

            // Calculate the lower left corner of the grid.
            _gridZeroZeroUpperLeftCornerPosition = _grid[0, 0].m_vPosition + new Vector3(-_halfNodeWidth, 0f, -_halfNodeWidth);
        }

        private void GenerateNodes()
        {
            for (int i = 0; i < GridHeight; i++)
            {
                for (int j = 0; j < GridWidth; j++)
                {
                    _grid[i, j] = new NodeHeap(false,
                        new Vector3((-_gridSize.x / 2) + _halfNodeWidth + (i * NodeHeight),
                        _halfNodeWidth,
                        (-_gridSize.y / 2) + _halfNodeWidth + (j * NodeWidth)),
                        i,
                        j);

                    if (Physics.CheckSphere(_grid[i, j].m_vPosition,
                        _halfNodeWidth,
                        _obstacleMask))
                    {
                        _grid[i, j]._nodeType = NodeHeap.NodeType.blocked;
                    }
                    else
                    {
                        _grid[i, j]._nodeType = NodeHeap.NodeType.open;
                    }
                }
            }
        }

        private List<NodeHeap> ReturnNeighbours(PathGridManagerHeap pathGridManager, NodeHeap _currentNode)
        {
            List<NodeHeap> _neighbourList = new List<NodeHeap>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    if (_currentNode.m_iGridX + x >= 0 && // X cannot be negative.
                        _currentNode.m_iGridY + y >= 0 && // Y cannot be negative.
                        _currentNode.m_iGridX + x <= pathGridManager._gridSize.x - 1 && // X cannot be outside the grid's bounds.
                        _currentNode.m_iGridY + y <= pathGridManager._gridSize.y - 1) // Y cannot be outside the grid's bounds.
                    {
                        _neighbourList.Add(pathGridManager._grid[_currentNode.m_iGridX + x, _currentNode.m_iGridY + y]);
                    }
                }
            }

            return _neighbourList;
        }

        private void BakeNeighbours()
        {
            foreach (NodeHeap node in _grid)
            {
                node._nodeNeighbourList = ReturnNeighbours(this, node);
            }
        }

        private void DrawNodes()
        {
            NodeHeap playerGridCoordinates = GridLocator(_player.position);
            NodeHeap targetGridCoordinates = GridLocator(_target.position);

            for (int i = 0; i < GridHeight; i++)
            {
                for (int j = 0; j < GridWidth; j++)
                {
                    if (_grid[i, j]._nodeType == NodeHeap.NodeType.open)
                    {
                        Gizmos.color = Color.clear;
                    }

                    if (_grid[i, j]._nodeType == NodeHeap.NodeType.blocked)
                    {
                        Gizmos.color = Color.red;
                    }

                    if (_grid[i, j].Equals(playerGridCoordinates))
                    {
                        Gizmos.color = Color.green;
                    }

                    if (_grid[i, j].Equals(targetGridCoordinates))
                    {
                        Gizmos.color = Color.magenta;
                    }

                    if (_path.Contains(_grid[i, j]))
                    {
                        Gizmos.color = Color.black;
                    }

                    Gizmos.DrawWireCube(_grid[i, j].m_vPosition,
                        new Vector3(NodeWidth, NodeHeight, NodeHeight));
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(_gridSize.x, _halfNodeWidth, _gridSize.y));
            if (_grid != null) {
                DrawNodes();
            }
        }

        public NodeHeap GridLocator(Vector3 location)
        {
            // Get the location's relative position to the grid.
            Vector3 relativeLocationPosition = new Vector3(location.x - _gridZeroZeroUpperLeftCornerPosition.x,
                _halfNodeWidth,
                location.z - _gridZeroZeroUpperLeftCornerPosition.z);

            // Dividing the results with the full node width and rounding the results to integers will return the location's grid position.
            // Clamp the values between 0 and the current grid size.
            int locationX = Mathf.Clamp((int)(relativeLocationPosition.x / (_halfNodeWidth * 2f)), 0, GridWidth - 1),
                locationZ = Mathf.Clamp((int)(relativeLocationPosition.z / (_halfNodeWidth * 2f)), 0, GridHeight - 1);

            return _grid[locationX, locationZ];
        }
    }
}