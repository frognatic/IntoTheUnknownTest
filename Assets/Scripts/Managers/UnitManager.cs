using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using IntoTheUnknownTest.Libraries;
using IntoTheUnknownTest.Map;
using IntoTheUnknownTest.Pathfinding;
using UnityEngine;

namespace IntoTheUnknownTest.Managers
{
    public class UnitManager : Singleton<UnitManager>
    {
        [SerializeField] private UnitsLibrary _unitsLibrary;
        
        private const float _moveDurationPerTile = 0.3f;
        private readonly List<MapTileUnit> _activeUnits = new List<MapTileUnit>();
        
        public MapTileUnit PlayerUnit { get; private set; }
        public List<BaseUnitData> Units => _unitsLibrary.BaseUnits;
        
        public void SpawnUnit(BaseUnitData unitData, MapTile targetTile)
        {
            if (targetTile.OccupyingUnit != null) return;

            if (unitData.IsUniqueOnMap)
            {
                var existingUnit = _activeUnits.FirstOrDefault(u => u.UnitData == unitData);
                if (existingUnit != null)
                {
                    DespawnUnit(existingUnit);
                }
            }

            MapTileUnit newUnit = PoolingManager.Instance.Get<MapTileUnit>(PoolObjectType.Units);
            newUnit.Setup(unitData, targetTile);

            targetTile.OccupyingUnit = newUnit;
            _activeUnits.Add(newUnit);

            if (unitData is PlayerUnitData)
            {
                PlayerUnit = newUnit;
            }
        }
        
        public void DespawnUnit(MapTileUnit unitToDespawn)
        {
            if (unitToDespawn == null) return;

            if (unitToDespawn.CurrentTile != null)
            {
                unitToDespawn.CurrentTile.OccupyingUnit = null;
            }
            
            _activeUnits.Remove(unitToDespawn);
            PoolingManager.Instance.ReturnToPool(PoolObjectType.Units, unitToDespawn);
            
            if (unitToDespawn == PlayerUnit)
            {
                PlayerUnit = null;
            }
        }
        
        public void MoveUnitAlongPath(MapTileUnit unitToMove, List<Vector3> path)
        {
            StartCoroutine(MoveUnitCoroutine(unitToMove, path));
        }
        
        private IEnumerator MoveUnitCoroutine(MapTileUnit unitToMove, List<Vector3> path)
        {
            MapTileManager.Instance.LockInput();
            
            MapTile currentTile = unitToMove.CurrentTile;

            foreach (var targetPosition in path)
            {
                PathfindingNode nextNode = PathfindingManager.Instance.GetNode(targetPosition);
                if (!MapTileManager.Instance.TryGetTile(nextNode.GridPosition, out MapTile nextTile)) break;
                
                Tween moveTween = unitToMove.transform.DOMove(targetPosition, _moveDurationPerTile).SetEase(Ease.Linear);
                yield return moveTween.WaitForCompletion();
                
                currentTile.OccupyingUnit = null; 
                nextTile.OccupyingUnit = unitToMove;
                unitToMove.SetCurrentTile(nextTile);
                currentTile = nextTile;
            }
            
            MapTileManager.Instance.UnlockInput();
        }
        
        public void ClearAllUnits()
        {
            var unitsToClear = new List<MapTileUnit>(_activeUnits);
            foreach (var unit in unitsToClear)
            {
                DespawnUnit(unit);
            }
            _activeUnits.Clear();
        }
    }
}
