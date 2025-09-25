using System.Collections.Generic;
using IntoTheUnknownTest.Grid;
using IntoTheUnknownTest.Managers;
using UnityEngine;

namespace IntoTheUnknownTest.Pathfinding
{
    public class PathfindingGrid
    {
        private GridSettings _gridSettings;
        private PathfindingNode[,] _grid;
        private Vector2Int _gridSize;

        private readonly Vector2Int[] _orthogonalMoveDirections = {
            new Vector2Int(1, 0), // E
            new Vector2Int(-1, 0), // W
            new Vector2Int(0, 1), // N
            new Vector2Int(0, -1), // S
        };
        
        private readonly Vector2Int[] _diagonalMoveDirections = {
            new Vector2Int(1, 1), // NE
            new Vector2Int(1, -1), // SE
            new Vector2Int(-1, 1), // NW
            new Vector2Int(-1, -1) // SW
        };

        public PathfindingGrid(GridSettings gridSettings)
        {
            _gridSettings = gridSettings;
        }
        
        public PathfindingNode[,] Grid => _grid;
        
        public int MaxSize => _gridSettings.GetGridX * _gridSettings.GetGridY;
        
        public void InitializeGrid()
        {
            int gridX = _gridSettings.GetGridX;
            int gridY = _gridSettings.GetGridY;

            _gridSize = new Vector2Int(gridX, gridY);
            CreateGrid();
            FillAllNeighbours();
        }
        
        private void CreateGrid()
        {
            _grid = new PathfindingNode[_gridSize.x, _gridSize.y];
            Vector3 worldBottomLeftVector = GetWorldBottomLeftVector;

            for (int x = 0; x < _gridSize.x; x++)
            {
                for (int y = 0; y < _gridSize.y; y++)
                {
                    Vector3 worldPoint = worldBottomLeftVector + Vector3.right * (x * _gridSettings.NodeDiameter + _gridSettings.NodeRadius) +
                        Vector3.up * (y * _gridSettings.NodeDiameter + _gridSettings.NodeRadius);

                    Vector2Int gridPosition = new Vector2Int(x, y);
                    _grid[x, y] = new PathfindingNode(worldPoint, gridPosition);
                }
            }
        }

        private void FillAllNeighbours()
        {
            foreach (var node in _grid)
                FillNeighboursList(node);
        }
        
        public List<PathfindingNode> GetNeighbours(PathfindingNode node, bool allowDiagonal)
        {
            List<PathfindingNode> neighbours = new List<PathfindingNode>();

            foreach (Vector2Int direction in _orthogonalMoveDirections)
            {
                TryAddNeighbourInDirection(node, direction, neighbours);
            }

            if (allowDiagonal)
            {
                foreach (Vector2Int direction in _diagonalMoveDirections)
                {
                    TryAddNeighbourInDirection(node, direction, neighbours);
                }
            }

            return neighbours;
        }
        
        private void TryAddNeighbourInDirection(PathfindingNode node, Vector2Int direction, List<PathfindingNode> neighbours)
        {
            Vector2Int checkPosition = node.GridPosition + direction;

            if (checkPosition.x >= 0 && checkPosition.x < _gridSize.x && checkPosition.y >= 0 && checkPosition.y < _gridSize.y)
            {
                neighbours.Add(_grid[checkPosition.x, checkPosition.y]);
            }
        }

        private void FillNeighboursList(PathfindingNode node)
        {
            foreach (var dir in _orthogonalMoveDirections)
            {
                int checkX = node.GridPosition.x + dir.x;
                int checkY = node.GridPosition.y + dir.y;

                if (checkX >= 0 && checkX < _gridSize.x && checkY >= 0 && checkY < _gridSize.y)
                {
                    node.AddNeighbour(_grid[checkX, checkY]);
                }
            }

            if (PathfindingManager.Instance.UseDiagonalMoveCalculations)
            {
                foreach (var dir in _diagonalMoveDirections)
                {
                    int checkX = node.GridPosition.x + dir.x;
                    int checkY = node.GridPosition.y + dir.y;

                    if (checkX >= 0 && checkX < _gridSize.x && checkY >= 0 && checkY < _gridSize.y)
                    {
                        node.AddNeighbour(_grid[checkX, checkY]);
                    }
                }
            }
        }

        private Vector3 GetWorldBottomLeftVector => Vector3.zero - Vector3.right * _gridSettings.GridWorldSize.x * 0.5f -
            Vector3.up * _gridSettings.GridWorldSize.y * 0.5f;

        public PathfindingNode GetNode(Vector3 worldPosition)
        {
            float percentX = (worldPosition.x - GetWorldBottomLeftVector.x) / _gridSettings.GridWorldSize.x;
            float percentY = (worldPosition.y - GetWorldBottomLeftVector.y) / _gridSettings.GridWorldSize.y;

            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.FloorToInt((_gridSize.x) * percentX);
            int y = Mathf.FloorToInt((_gridSize.y) * percentY);
            
            return _grid[x, y];
        }

        public PathfindingNode GetNode(Vector2Int gridPosition)
        {
            return _grid[gridPosition.x, gridPosition.y];
        }
    }
}