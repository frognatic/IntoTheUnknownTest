using System;
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
        
        private readonly List<MapTileUnit> _activeUnits = new List<MapTileUnit>();
        private const float _moveDurationPerTile = 0.3f;
        private const float _attackLungeAnimationDistance = 0.5f;
        private const float _attackAnimationDuration = 0.15f;
        
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
        
        public void MoveUnitAlongPath(MapTileUnit unitToMove, List<Vector3> path, Action onComplete = null)
        {
            StartCoroutine(MoveUnitCoroutine(unitToMove, path, onComplete));
        }
        
        private IEnumerator MoveUnitCoroutine(MapTileUnit unitToMove, List<Vector3> path, Action onComplete = null)
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
            onComplete?.Invoke(); 
        }
        
        public void AttackUnit(MapTileUnit attacker, MapTileUnit target, Action onComplete = null)
        {
            StartCoroutine(AttackCoroutine(attacker, target, onComplete));
        }
        
        private IEnumerator AttackCoroutine(MapTileUnit attacker, MapTileUnit target, Action onComplete = null)
        {
            MapTileManager.Instance.LockInput();

            Vector3 originalPosition = attacker.transform.position;
            Vector3 targetPosition = target.transform.position;
            Vector3 direction = (targetPosition - originalPosition).normalized;

            Sequence attackSequence = DOTween.Sequence();
            attackSequence.Append(attacker.transform.DOMove(originalPosition + direction * _attackLungeAnimationDistance, _attackAnimationDuration));
            attackSequence.Append(attacker.transform.DOMove(originalPosition, _attackAnimationDuration));
        
            yield return attackSequence.WaitForCompletion();
        
            target.TakeDamage();

            MapTileManager.Instance.ClearPreviousPathHighlight();
            MapTileManager.Instance.UnlockInput();
            yield return null;
            
            onComplete?.Invoke(); 
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
