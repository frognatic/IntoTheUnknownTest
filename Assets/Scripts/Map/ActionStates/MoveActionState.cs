using System.Collections.Generic;
using IntoTheUnknownTest.Managers;
using IntoTheUnknownTest.Pathfinding;
using UnityEngine;

namespace IntoTheUnknownTest.Map
{
    public class MoveActionState : IActionState
    {
        private readonly MapTileManager _mapTileManager;
        private MapTileUnit _unit;
        private MapTile _targetTile;
        private List<PathfindingNode> _currentPath;

        public MoveActionState(MapTileManager mapTileManager)
        {
            _mapTileManager = mapTileManager;
        }
        
        public int ActionRange => ((PlayerUnitData)_unit.UnitData).MoveRange;

        public void OnEnter(MapTile targetTile, MapTileUnit unit)
        {
            _unit = unit;
            _targetTile = targetTile;

            Vector3 startPos = _unit.CurrentTile.transform.position;
            Vector3 targetPos = _targetTile.transform.position;

            _currentPath = PathfindingManager.Instance.FindPath(startPos, targetPos, node => node.IsWalkable);

            if (_currentPath != null)
            {
                _mapTileManager.HighlightPath(_currentPath, _unit.CurrentTile, ActionRange);
            }
        }

        public void PerformAction()
        {
            if (_currentPath != null && _currentPath.Count <= ActionRange)
            {
                UnitManager.Instance.MoveUnitAlongPath(_unit, _currentPath, () => _mapTileManager.ClearActionState());
                MapTileManager.Instance.ClearPreviousPathHighlight();
            }
        }

        public void OnExit()
        {
            _mapTileManager.ClearPreviousPathHighlight();
        }
    }
}
