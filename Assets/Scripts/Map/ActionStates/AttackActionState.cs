using System.Collections.Generic;
using System.Linq;
using IntoTheUnknownTest.Managers;
using IntoTheUnknownTest.Pathfinding;

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

            if (TryPreparePerformableDirectAttack())
            {
                return;
            }

            if (TryPreparePerformableMoveAndAttack())
            {
                return;
            }

            ShowBestImpossibleAction();
        }
        
        private bool TryPreparePerformableDirectAttack()
        {
            var directAttackPath = FindDirectAttackPath();
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
                var attackPath = PathfindingManager.Instance.FindPath(attackPositionNode.WorldPosition, targetNode.WorldPosition, node => node.IsAttackableThrough || node == targetNode);
                SetupAction(true, bestValidMovePath, attackPath);
                return true;
            }
            return false;
        }
        
        private void ShowBestImpossibleAction()
        {
            var targetNode = PathfindingManager.Instance.GetNode(_targetTile.GridPosition);
            var candidateNodes = GetValidAttackPositions(targetNode);
            var bestButOutOfRangePath = FindShortestGeometricPathToAnyCandidate(candidateNodes);

            if (bestButOutOfRangePath != null)
            {
                var attackPositionNode = bestButOutOfRangePath.Last();
                var attackPath = PathfindingManager.Instance.FindPath(attackPositionNode.WorldPosition, targetNode.WorldPosition, node => node.IsAttackableThrough || node == targetNode);
                SetupAction(false, bestButOutOfRangePath, attackPath);
            }
            else
            {
                var directAttackPath = FindDirectAttackPath() ?? new List<PathfindingNode>();
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

        #region Helper Methods
        private List<PathfindingNode> FindDirectAttackPath()
        {
            return PathfindingManager.Instance.FindPath(
                _unit.CurrentTile.transform.position, 
                _targetTile.transform.position,
                node => node.IsAttackableThrough || node == PathfindingManager.Instance.GetNode(_targetTile.GridPosition)
            );
        }

        private List<PathfindingNode> GetValidAttackPositions(PathfindingNode targetNode)
        {
            var nodesInRange = PathfindingManager.Instance.GetNodesWithinRange(targetNode, AttackRange);
            return nodesInRange.Where(node => 
                node.IsWalkable && 
                _mapTileManager.TryGetTile(node.GridPosition, out var tile) && tile.OccupyingUnit == null &&
                PathfindingManager.Instance.FindPath(node.WorldPosition, targetNode.WorldPosition, n => n.IsAttackableThrough || n == targetNode) != null
            ).ToList();
        }

        private List<PathfindingNode> FindBestValidPathToAnyCandidate(List<PathfindingNode> candidateNodes)
        {
            List<PathfindingNode> bestValidPath = null;
            foreach (var candidateNode in candidateNodes)
            {
                var movePath = PathfindingManager.Instance.FindPath(
                    _unit.CurrentTile.transform.position, 
                    candidateNode.WorldPosition, 
                    node => (node.IsWalkable && (_mapTileManager.TryGetTile(node.GridPosition, out var t) && t.OccupyingUnit == null)) || node == candidateNode
                );

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
        
        private List<PathfindingNode> FindShortestGeometricPathToAnyCandidate(List<PathfindingNode> candidateNodes)
        {
            List<PathfindingNode> bestGeometricPath = null;
            foreach (var candidateNode in candidateNodes)
            {
                var movePath = PathfindingManager.Instance.FindPath(
                    _unit.CurrentTile.transform.position, 
                    candidateNode.WorldPosition, 
                    node => (node.IsWalkable && (_mapTileManager.TryGetTile(node.GridPosition, out var t) && t.OccupyingUnit == null)) || node == candidateNode
                );
                
                if (movePath != null)
                {
                    if (bestGeometricPath == null || movePath.Count < bestGeometricPath.Count)
                    {
                        bestGeometricPath = movePath;
                    }
                }
            }
            return bestGeometricPath;
        }
        #endregion
    }
}