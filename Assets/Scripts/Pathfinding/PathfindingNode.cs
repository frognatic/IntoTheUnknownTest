using System;
using System.Collections.Generic;
using UnityEngine;

namespace IntoTheUnknownTest.Pathfinding
{
    [Serializable]
    public class PathfindingNode : IHeapItem<PathfindingNode>
    {
        public int HeapIndex { get; set; }
        public bool IsWalkable { get; private set; }
        public bool IsAttackableThrough { get; private set; }
        public Vector3 WorldPosition { get; }
        public Vector2Int GridPosition { get; }
        public int GCost { get; private set; }
        public int HCost { get; private set; }
        public int FCost => GCost + HCost;
        public PathfindingNode PreviousNode { get; private set; }
        public List<PathfindingNode> NeighboursList { get; } = new();
        
        public PathfindingNode(Vector3 worldPosition, Vector2Int gridPosition)
        {
            WorldPosition = worldPosition;
            GridPosition = gridPosition;
        }

        internal void AddNeighbour(PathfindingNode neighbour) => NeighboursList.Add(neighbour);
        internal void SetGCost(int valueToSet) => GCost = valueToSet;
        internal void SetHCost(int valueToSet) => HCost = valueToSet;
        internal void SetPreviousNode(PathfindingNode node) => PreviousNode = node;
        public void SetWalkable(bool isWalkable) => IsWalkable = isWalkable;
        public void SetAttackableThrough(bool isAttackableThrough) => IsAttackableThrough = isAttackableThrough;
        public int CompareTo(PathfindingNode other)
        {
            int compare = FCost.CompareTo(other.FCost);
            if (compare == 0)
            {
                compare = HCost.CompareTo(other.HCost);
            }
            return compare;
        }
    }
}
