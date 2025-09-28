using System;
using System.Collections.Generic;
using System.Linq;
using IntoTheUnknownTest.Libraries;
using IntoTheUnknownTest.Map;
using IntoTheUnknownTest.Pathfinding;
using UnityEngine;

namespace IntoTheUnknownTest.Managers
{
    public class MapTileManager : Singleton<MapTileManager>
    {
        public event Action<Vector2Int, bool, bool> TileDataChanged;

        [SerializeField] private MapColorsLibrary _mapColorsLibrary;
        [SerializeField] private MapTileLibrary _mapTileLibrary;
        [SerializeField] private Transform _mapTileContent;
        [SerializeField] private Transform _unitsContent;

        private const int _defaultMapTilePoolSize = 100;
        private const int _defaultUnitPoolSize = 5;

        private List<MapTile> _previousPathTiles = new List<MapTile>();
        private readonly Dictionary<Vector2Int, MapTile> _mapTiles = new Dictionary<Vector2Int, MapTile>();

        private bool _isAnyActionActive;
        private MapTile _currentTargetTile;

        private IActionState _currentActionState;

        public List<BaseMapTileData> MapTiles => _mapTileLibrary.MapTiles;
        public List<TileSelectorConfig> TileSelectorConfig => _mapColorsLibrary.TileSelectorConfigs;

        private void Start()
        {
            InitializePools();
            GenerateGridAndTiles();

            UnitManager.Instance.UnitActionStarted += LockInput;
            UnitManager.Instance.UnitActionEnded += UnlockInput;
        }

        private void OnDisable()
        {
            UnitManager.Instance.UnitActionStarted -= LockInput;
            UnitManager.Instance.UnitActionEnded -= UnlockInput;
        }

        private void InitializePools()
        {
            PoolingManager.Instance.PrepareMapTiles(_mapTileContent, _defaultMapTilePoolSize);
            PoolingManager.Instance.PrepareUnits(_unitsContent, _defaultUnitPoolSize);
        }

        public void GenerateGridAndTiles()
        {
            ReturnAllPooledMapElements();

            var pathfindingGrid = PathfindingManager.Instance.GetPathfindingGrid();

            MapGenerator generator = new MapGenerator(_mapTileLibrary);
            generator.GenerateGridData(pathfindingGrid);

            foreach (var node in pathfindingGrid.Grid)
            {
                BaseMapTileData tileDataData = _mapTileLibrary.DefaultMapTileData;

                MapTile mapTile = PoolingManager.Instance.Get<MapTile>(PoolObjectType.MapTiles);
                mapTile.transform.position = node.WorldPosition;
                mapTile.InitTile(tileDataData, node.GridPosition);

                _mapTiles.Add(node.GridPosition, mapTile);
            }
        }

        private void ReturnAllPooledMapElements()
        {
            UnitManager.Instance.ClearAllUnits();
            if (_mapTiles.Count > 0)
            {
                foreach (var mapTile in _mapTiles.Values)
                {
                    PoolingManager.Instance.ReturnToPool(PoolObjectType.MapTiles, mapTile);
                }
            }
            _mapTiles.Clear();
        }

        private void LockInput() => _isAnyActionActive = true;
        private void UnlockInput() => _isAnyActionActive = false;

        public void HandleActionRequest(MapTile clickedTile)
        {
            if (_isAnyActionActive) return;

            var playerUnit = UnitManager.Instance.PlayerUnit;
            if (playerUnit == null) return;

            if (_currentActionState != null && clickedTile == _currentTargetTile)
            {
                _currentActionState.PerformAction();
                return;
            }

            PrepareNewAction(clickedTile, playerUnit);
        }

        private void PrepareNewAction(MapTile clickedTile, MapTileUnit playerUnit)
        {
            ClearActionState();
            _currentTargetTile = clickedTile;

            bool isEnemyClicked = clickedTile.OccupyingUnit?.UnitData is EnemyUnitData;

            if (isEnemyClicked)
            {
                _currentActionState = new AttackActionState(this);
            }
            else if (clickedTile.IsWalkable && clickedTile.OccupyingUnit == null)
            {
                _currentActionState = new MoveActionState(this);
            }
            _currentActionState?.OnEnter(_currentTargetTile, playerUnit);
        }

        public void ClearActionState()
        {
            _currentActionState?.OnExit();
            _currentActionState = null;
            _currentTargetTile = null;
        }

        public void ClearPreviousPathHighlight()
        {
            SetDefaultColorsForPreviousPath();
            _previousPathTiles.Clear();
        }

        private void SetDefaultColorsForPreviousPath()
        {
            foreach (var tile in _previousPathTiles)
            {
                if (tile != null)
                {
                    tile.SetColor(_mapColorsLibrary.GetColorForAction(TileSelectorActionType.Default));
                }
            }
        }

        public bool TryGetTile(Vector2Int gridPosition, out MapTile tile)
        {
            return _mapTiles.TryGetValue(gridPosition, out tile);
        }

        public void HighlightPath(List<PathfindingNode> path, MapTile startTile, int actionRange)
        {
            ClearPreviousPathHighlight();

            ColorTiles(path, (_, index) => index + 1 <= actionRange ? 
                _mapColorsLibrary.GetColorForAction(TileSelectorActionType.InRange) : _mapColorsLibrary.GetColorForAction(TileSelectorActionType.OutOfRange));

            startTile.SetColor(_mapColorsLibrary.GetColorForAction(TileSelectorActionType.StartTile));

            CachePathForClearing(path, startTile);
        }

        public void HighlightMoveAndAttackPath(List<PathfindingNode> movePath, List<PathfindingNode> attackPath, MapTile startTile, int moveRange, int attackRange, bool isSequenceValid)
        {
            ClearPreviousPathHighlight();

            // Move path
            ColorTiles(movePath, (_, index) => isSequenceValid ? 
                _mapColorsLibrary.GetColorForAction(TileSelectorActionType.InRange) : index + 1 <= moveRange ? _mapColorsLibrary.GetColorForAction(TileSelectorActionType.InRange) : _mapColorsLibrary.GetColorForAction(TileSelectorActionType.OutOfRange));

            // Attack path
            ColorTiles(attackPath, (_, index) => (index + 1) <= attackRange ? 
                _mapColorsLibrary.GetColorForAction(TileSelectorActionType.AttackPath) : _mapColorsLibrary.GetColorForAction(TileSelectorActionType.OutOfRange));

            startTile.SetColor(_mapColorsLibrary.GetColorForAction(TileSelectorActionType.StartTile));

            CachePathForClearing(movePath.Concat(attackPath), startTile);
        }

        private void ColorTiles(IEnumerable<PathfindingNode> pathNodes, Func<PathfindingNode, int, Color> colorSelector)
        {
            int index = 0;
            foreach (var node in pathNodes)
            {
                if (TryGetTile(node.GridPosition, out var tile))
                {
                    tile.SetColor(colorSelector(node, index));
                }
                index++;
            }
        }

        private void CachePathForClearing(IEnumerable<PathfindingNode> path, MapTile startTile)
        {
            _previousPathTiles = path.Select(node => _mapTiles.GetValueOrDefault(node.GridPosition))
                .Where(tile => tile != null)
                .ToList();
            _previousPathTiles.Add(startTile);
        }

        public void TryUpdateTile(Vector2Int gridPosition, IMapElement mapElement)
        {
            if (_mapTiles.TryGetValue(gridPosition, out MapTile tileToUpdate))
            {
                SelectTileUpdateAction(mapElement, tileToUpdate);
                TileDataChanged?.Invoke(gridPosition, tileToUpdate.IsWalkable, tileToUpdate.IsAttackableThrough);
            }
        }

        private void SelectTileUpdateAction(IMapElement mapElement, MapTile tileToUpdate)
        {
            switch (mapElement)
            {
                case BaseMapTileData selectedTileData:
                    tileToUpdate.UpdateTile(selectedTileData);
                    break;
                case BaseUnitData selectedUnitData:
                    tileToUpdate.UpdateTile(_mapTileLibrary.DefaultMapTileData);
                    UnitManager.Instance.SpawnUnit(selectedUnitData, tileToUpdate);
                    break;
            }
        }
    }
}
