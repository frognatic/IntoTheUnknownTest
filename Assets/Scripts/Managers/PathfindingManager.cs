using System;
using System.Collections.Generic;
using IntoTheUnknownTest.Grid;
using IntoTheUnknownTest.Pathfinding;
using UnityEngine;

namespace IntoTheUnknownTest.Managers
{
    public class PathfindingManager : Singleton<PathfindingManager>
    {
        [SerializeField] private GridSettings _gridSettings;

        public bool UseDiagonalMoveCalculations => _gridSettings.UseDiagonalMoveCalculations;

        private const int MoveStraightCost = 10;
        private const int MoveDiagonalCost = 14;

        private PathfindingGrid _pathfindingGrid;

        private PriorityQueue<PathfindingNode> _openSet;
        private HashSet<PathfindingNode> _closedSet;

        protected override void Awake()
        {
            base.Awake();

            _closedSet = new HashSet<PathfindingNode>();
            _pathfindingGrid = new PathfindingGrid(_gridSettings);
        }

        private void Start()
        {
            MapTileManager.Instance.TileDataChanged += OnTileDataChanged;
        }

        private void OnDisable()
        {
            MapTileManager.Instance.TileDataChanged -= OnTileDataChanged;
        }
        
        private void OnTileDataChanged(Vector2Int gridPosition, bool isWalkable, bool isAttackableThrough)
        {
            PathfindingNode nodeToUpdate = GetNode(gridPosition);
            nodeToUpdate?.SetWalkable(isWalkable);
            nodeToUpdate?.SetAttackableThrough(isAttackableThrough);
        }

        public PathfindingGrid GetPathfindingGrid() => _pathfindingGrid;

        public List<PathfindingNode> FindPath(Vector3 startPos, Vector3 targetPos, Predicate<PathfindingNode> traversableCondition)
        {
            PathfindingNode startNode = GetNode(startPos);
            PathfindingNode targetNode = GetNode(targetPos);

            _openSet = new PriorityQueue<PathfindingNode>(_pathfindingGrid.MaxSize);
            _closedSet.Clear();

            _openSet.Enqueue(startNode);

            while (_openSet.Count > 0)
            {
                PathfindingNode currentNode = _openSet.Dequeue();
                _closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    return CalculatedPath(startNode, targetNode);
                }

                CalculateMoveToNeighbourCosts(currentNode, targetNode, traversableCondition);
            }

            return null;
        }

        public PathfindingNode GetNode(Vector3 pos) => _pathfindingGrid.GetNode(pos);
        public PathfindingNode GetNode(Vector2Int pos) => _pathfindingGrid.GetNode(pos);
        
        public List<PathfindingNode> GetNodesWithinRange(PathfindingNode centerNode, int range)
        {
            var results = new List<PathfindingNode>();
            var grid = GetPathfindingGrid();

            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    if (x == 0 && y == 0) continue;

                    int distance = Mathf.Abs(x) + Mathf.Abs(y);
                    if (distance > range) continue;

                    Vector2Int checkPos = centerNode.GridPosition + new Vector2Int(x, y);
            
                    if (checkPos.x >= 0 && checkPos.x < _gridSettings.GetGridX && checkPos.y >= 0 && checkPos.y < _gridSettings.GetGridY)
                    {
                        results.Add(grid.GetNode(checkPos));
                    }
                }
            }
            return results;
        }

        private void CalculateMoveToNeighbourCosts(PathfindingNode currentNode, PathfindingNode targetNode, Predicate<PathfindingNode> traversableCondition)
        {
            foreach (PathfindingNode neighbour in _pathfindingGrid.GetNeighbours(currentNode, UseDiagonalMoveCalculations))
            {
                if (!traversableCondition(neighbour) && neighbour != targetNode || _closedSet.Contains(neighbour))
                    continue;

                int costMovingToNeighbour = currentNode.GCost + CalculateDistanceCost(currentNode, neighbour);
                bool isInOpenSet = _openSet.Contains(neighbour);
                
                if (costMovingToNeighbour < neighbour.GCost || !isInOpenSet)
                {
                    neighbour.SetGCost(costMovingToNeighbour);
                    neighbour.SetHCost(CalculateDistanceCost(neighbour, targetNode));
                    neighbour.SetPreviousNode(currentNode);

                    if (!isInOpenSet)
                    {
                        _openSet.Enqueue(neighbour);
                    }
                    else
                    {
                        _openSet.UpdateItem(neighbour);
                    }
                }
            }
        }

        private List<PathfindingNode> CalculatedPath(PathfindingNode startNode, PathfindingNode endNode)
        {
            List<PathfindingNode> path = new List<PathfindingNode>();
            PathfindingNode currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.PreviousNode;
            }

            path.Reverse();

            return path;
        }

        private int CalculateDistanceCost(PathfindingNode first, PathfindingNode second)
        {
            // Manhattan distance
            int xDistance = Mathf.Abs(first.GridPosition.x - second.GridPosition.x);
            int yDistance = Mathf.Abs(first.GridPosition.y - second.GridPosition.y);

            if (UseDiagonalMoveCalculations)
            {
                int diagonalMoves = Mathf.Min(xDistance, yDistance);
                int straightMoves = Mathf.Abs(xDistance - yDistance);

                return MoveDiagonalCost * diagonalMoves + MoveStraightCost * straightMoves;
            }
            return MoveStraightCost * (xDistance + yDistance);
        }
    }
}
