using System;
using System.Collections.Generic;
using IntoTheUnknownTest.Libraries;
using IntoTheUnknownTest.Map;
using IntoTheUnknownTest.Pathfinding;
using UnityEngine;

namespace IntoTheUnknownTest.Managers
{
    public class MapTileManager : Singleton<MapTileManager>
    {
        public event Action<Vector2Int, bool, bool> TileDataChanged;
        
        [SerializeField] private MapTileLibrary _mapTileLibrary;
        [SerializeField] private Transform _mapTileContent;
        [SerializeField] private Transform _unitsContent;

        [Header("Colors")]
        [SerializeField] private Color _startTileColor = Color.yellow;
        [SerializeField] private Color _inRangeColor = Color.green;
        [SerializeField] private Color _outOfRangeColor = Color.red;
        [SerializeField] private Color _defaultTileColor = Color.white;

        private const int _defaultMapTilePoolSize = 100;
        private const int _defaultUnitPoolSize = 5;

        private List<MapTile> _previousPathTiles = new List<MapTile>();
        private readonly Dictionary<Vector2Int, MapTile> _mapTiles = new Dictionary<Vector2Int, MapTile>();

        private bool _isAnyActionActive;
        private MapTile _currentTargetTile;
        
        private IActionState _currentActionState;

        public List<BaseMapTileData> MapTiles => _mapTileLibrary.MapTiles;

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
                    tile.SetColor(_defaultTileColor);
                }
            }
        }

        public bool TryGetTile(Vector2Int gridPosition, out MapTile tile)
        {
            return _mapTiles.TryGetValue(gridPosition, out tile);
        }

        public void HighlightPath(List<Vector3> path, MapTile startTileObject, int actionRange)
        {
            var tilesOnPath = GetTilesByPositions(path);

            for (int i = 0; i < tilesOnPath.Count; i++)
            {
                MapTile currentTile = tilesOnPath[i];

                int stepsToTile = i + 1;
                currentTile.SetColor(stepsToTile <= actionRange ? _inRangeColor : _outOfRangeColor);
            }

            startTileObject.SetColor(_startTileColor);

            _previousPathTiles = tilesOnPath;
            _previousPathTiles.Add(startTileObject);
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

        private List<MapTile> GetTilesByPositions(List<Vector3> positions)
        {
            List<MapTile> result = new List<MapTile>();
            foreach (var pos in positions)
            {
                PathfindingNode node = PathfindingManager.Instance.GetNode(pos);
                if (node != null && _mapTiles.TryGetValue(node.GridPosition, out var mapTile))
                {
                    result.Add(mapTile);
                }
            }
            return result;
        }
    }
}
