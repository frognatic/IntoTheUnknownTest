using System.Collections.Generic;
using IntoTheUnknownTest.Managers;
using UnityEngine;

namespace IntoTheUnknownTest.Map
{
    public class AttackActionState : IActionState
    {
        private MapTileManager _mapTileManager;
        private MapTileUnit _unit;
        private MapTile _targetTile;
        private List<Vector3> _currentPath;

        public AttackActionState(MapTileManager mapTileManager)
        {
            _mapTileManager = mapTileManager;
        }
        
        public int ActionRange => ((PlayerUnitData)_unit.UnitData).AttackRange;

        public void OnEnter(MapTile targetTile, MapTileUnit unit)
        {
            _unit = unit;
            _targetTile = targetTile;

            Vector3 startPos = _unit.CurrentTile.transform.position;
            Vector3 targetPos = _targetTile.transform.position;

            _currentPath = PathfindingManager.Instance.FindPath(startPos, targetPos, node => node.IsAttackableThrough);
            if (_currentPath != null)
            {
                _mapTileManager.HighlightPath(_currentPath, _unit.CurrentTile, ActionRange);
            }
        }
        
        public void PerformAction()
        {
            if (_currentPath != null && _currentPath.Count <= ActionRange)
            {
                UnitManager.Instance.AttackUnit(_unit, _targetTile.OccupyingUnit, () => _mapTileManager.ClearActionState());
            }
            else
            {
                _mapTileManager.ClearActionState();
                MapTileManager.Instance.ClearPreviousPathHighlight();
            }
        }

        public void OnExit()
        {
            _mapTileManager.ClearPreviousPathHighlight();
        }
    }
}
