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
        public event Action UnitActionStarted;
        public event Action UnitActionEnded;
        
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

            targetTile.SetOccupant(newUnit);
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
                unitToDespawn.CurrentTile.ClearOccupant();
            }
            
            _activeUnits.Remove(unitToDespawn);
            PoolingManager.Instance.ReturnToPool(PoolObjectType.Units, unitToDespawn);
            
            if (unitToDespawn == PlayerUnit)
            {
                PlayerUnit = null;
            }
        }
        
        public void MoveUnitAlongPath(MapTileUnit unitToMove, List<PathfindingNode> path, Action onComplete = null)
        {
            StartCoroutine(MoveUnitCoroutine(unitToMove, path, onComplete));
        }
        
        private IEnumerator MoveUnitCoroutine(MapTileUnit unitToMove, List<PathfindingNode> path, Action onComplete = null)
        {
            UnitActionStarted?.Invoke();
            
            MapTile currentTile = unitToMove.CurrentTile;

            foreach (var node in path)
            {
                if (unitToMove == null || unitToMove.gameObject == null)
                {
                    UnitActionEnded?.Invoke();
                    yield break;
                }
                
                if (!MapTileManager.Instance.TryGetTile(node.GridPosition, out MapTile nextTile)) break;
                
                Tween moveTween = unitToMove.transform.DOMove(node.WorldPosition, _moveDurationPerTile).SetEase(Ease.Linear).SetId(gameObject);
                yield return moveTween.WaitForCompletion();

                currentTile.ClearOccupant();
                nextTile.SetOccupant(unitToMove);
                unitToMove.SetCurrentTile(nextTile);
                currentTile = nextTile;
            }
            
            onComplete?.Invoke(); 
            UnitActionEnded?.Invoke();
        }
        
        public void AttackUnit(MapTileUnit attacker, MapTileUnit target, Action onComplete = null)
        {
            StartCoroutine(AttackCoroutine(attacker, target, onComplete));
        }
        
        private IEnumerator AttackCoroutine(MapTileUnit attacker, MapTileUnit target, Action onComplete = null)
        {
            UnitActionStarted?.Invoke();

            Vector3 originalPosition = attacker.transform.position;
            Vector3 targetPosition = target.transform.position;
            Vector3 direction = (targetPosition - originalPosition).normalized;

            Sequence attackSequence = DOTween.Sequence();
            attackSequence.Append(attacker.transform.DOMove(originalPosition + direction * _attackLungeAnimationDistance, _attackAnimationDuration));
            attackSequence.Append(attacker.transform.DOMove(originalPosition, _attackAnimationDuration)).SetId(gameObject);
        
            yield return attackSequence.WaitForCompletion();
        
            target.TakeDamage();

            MapTileManager.Instance.ClearPreviousPathHighlight();
            
            onComplete?.Invoke();
            UnitActionEnded?.Invoke();
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

        protected override void OnDestroy()
        {
            base.OnDestroy();
            DOTween.Kill(gameObject);
        }
    }
}
