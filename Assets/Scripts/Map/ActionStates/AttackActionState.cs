using System.Collections.Generic;
using System.Linq;
using IntoTheUnknownTest.Managers;
using IntoTheUnknownTest.Pathfinding;
using UnityEngine;

namespace IntoTheUnknownTest.Map
{
    public class AttackActionState : IActionState
    {
        private readonly MapTileManager _mapTileManager;
        private MapTileUnit _unit;
        private MapTile _targetTile;
        
        private List<PathfindingNode> _movePath = new List<PathfindingNode>();
        private List<PathfindingNode> _attackPath = new List<PathfindingNode>();
        
        private bool _isActionPerformable;

        public AttackActionState(MapTileManager mapTileManager)
        {
            _mapTileManager = mapTileManager;
        }
        
        private int MoveRange => ((PlayerUnitData)_unit.UnitData).MoveRange;
        private int AttackRange => ((PlayerUnitData)_unit.UnitData).AttackRange;
        
        public void OnEnter(MapTile targetTile, MapTileUnit unit)
        {
            _unit = unit;
            _targetTile = targetTile;

            if (TryPreparePerformableDirectAttack()) return;
            if (TryPreparePerformableMoveAndAttack()) return;
            ShowBestImpossibleAction();
        }

        private bool TryPreparePerformableDirectAttack()
        {
            var directAttackPath = FindPathForAttack(_unit.CurrentTile.transform.position, _targetTile.transform.position);
            if (directAttackPath != null && directAttackPath.Count <= AttackRange)
            {
                SetupAction(true, new List<PathfindingNode>(), directAttackPath);
                return true;
            }
            return false;
        }

        private bool TryPreparePerformableMoveAndAttack()
        {
            var targetNode = PathfindingManager.Instance.GetNode(_targetTile.GridPosition);
            var candidateNodes = GetValidAttackPositions(targetNode);
            var bestValidMovePath = FindBestValidPathToAnyCandidate(candidateNodes);

            if (bestValidMovePath != null)
            {
                var attackPositionNode = bestValidMovePath.Last();
                var attackPath = FindPathForAttack(attackPositionNode.WorldPosition, targetNode.WorldPosition);
                SetupAction(true, bestValidMovePath, attackPath);
                return true;
            }
            return false;
        }

        private void ShowBestImpossibleAction()
        {
            var targetNode = PathfindingManager.Instance.GetNode(_targetTile.GridPosition);
            var allCandidatePositions = GetValidAttackPositions(targetNode, true);

            List<PathfindingNode> bestMovePath = null;
            List<PathfindingNode> bestAttackPath = null;
            int shortestAttackPathLength = int.MaxValue;

            foreach (var candidateNode in allCandidatePositions)
            {
                var attackPath = FindPathForAttack(candidateNode.WorldPosition, targetNode.WorldPosition);
                if (attackPath == null) continue;

                var movePath = FindPathForMove(_unit.CurrentTile.transform.position, candidateNode.WorldPosition);
                if (movePath == null) continue;

                if (attackPath.Count < shortestAttackPathLength)
                {
                    shortestAttackPathLength = attackPath.Count;
                    bestAttackPath = attackPath;
                    bestMovePath = movePath;
                }
                else if (attackPath.Count == shortestAttackPathLength)
                {
                    if (bestMovePath == null || movePath.Count < bestMovePath.Count)
                    {
                        bestAttackPath = attackPath;
                        bestMovePath = movePath;
                    }
                }
            }

            if (bestMovePath != null)
            {
                SetupAction(false, bestMovePath, bestAttackPath);
            }
            else
            {
                var directAttackPath = FindPathForAttack(_unit.CurrentTile.transform.position, targetNode.WorldPosition) ?? new List<PathfindingNode>();
                SetupAction(false, new List<PathfindingNode>(), directAttackPath);
            }
        }

        private void SetupAction(bool performable, List<PathfindingNode> movePath, List<PathfindingNode> attackPath)
        {
            _isActionPerformable = performable;
            _movePath = movePath;
            _attackPath = attackPath;
            _mapTileManager.HighlightMoveAndAttackPath(_movePath, _attackPath, _unit.CurrentTile, MoveRange, AttackRange, _isActionPerformable);
        }

        public void PerformAction()
        {
            if (_isActionPerformable)
            {
                if (_movePath != null && _movePath.Any())
                {
                    UnitManager.Instance.MoveUnitAlongPath(_unit, _movePath, () =>
                    {
                        UnitManager.Instance.AttackUnit(_unit, _targetTile.OccupyingUnit, _mapTileManager.ClearActionState);
                    });
                }
                else
                {
                    UnitManager.Instance.AttackUnit(_unit, _targetTile.OccupyingUnit, _mapTileManager.ClearActionState);
                }
            }
            else
            {
                _mapTileManager.ClearActionState();
            }
        }

        public void OnExit()
        {
            _mapTileManager.ClearPreviousPathHighlight();
        }

        private List<PathfindingNode> FindPathForAttack(Vector3 start, Vector3 end)
        {
            return PathfindingManager.Instance.FindPath(start, end,
                node => node.IsAttackableThrough || node == PathfindingManager.Instance.GetNode(end));
        }
        
        private List<PathfindingNode> FindPathForMove(Vector3 start, Vector3 end)
        {
            return PathfindingManager.Instance.FindPath(start, end,
                node => (node.IsWalkable && (_mapTileManager.TryGetTile(node.GridPosition, out var t) && t.OccupyingUnit == null)) || node == PathfindingManager.Instance.GetNode(end));
        }

        private List<PathfindingNode> GetValidAttackPositions(PathfindingNode targetNode, bool ignoreAttackRange = false)
        {
            var searchRadius = AttackRange + MoveRange; 
            var nodesInRange = PathfindingManager.Instance.GetNodesWithinRange(targetNode, searchRadius);
            var validNodes = new List<PathfindingNode>();

            foreach (var node in nodesInRange)
            {
                if (node.IsWalkable && _mapTileManager.TryGetTile(node.GridPosition, out var tile) && tile.OccupyingUnit == null)
                {
                    var attackCheckPath = FindPathForAttack(node.WorldPosition, targetNode.WorldPosition);
                    if (attackCheckPath != null)
                    {
                        if (ignoreAttackRange || attackCheckPath.Count <= AttackRange)
                        {
                            validNodes.Add(node);
                        }
                    }
                }
            }
            return validNodes;
        }

        private List<PathfindingNode> FindBestValidPathToAnyCandidate(List<PathfindingNode> candidateNodes)
        {
            List<PathfindingNode> bestValidPath = null;
            foreach (var candidateNode in candidateNodes)
            {
                var movePath = FindPathForMove(_unit.CurrentTile.transform.position, candidateNode.WorldPosition);
                if (movePath != null && movePath.Count <= MoveRange)
                {
                    if (bestValidPath == null || movePath.Count < bestValidPath.Count)
                    {
                        bestValidPath = movePath;
                    }
                }
            }
            return bestValidPath;
        }
    }
}