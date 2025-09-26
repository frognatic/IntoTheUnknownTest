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

        private bool _isAnyActionActive = false;
        private MapTile _currentTargetTile = null;
        
        private List<Vector3> _currentHighlightedPath = new List<Vector3>();
        
        public List<BaseMapTileData> MapTiles => _mapTileLibrary.MapTiles;
        public BaseMapTileData DefaultMapTileData => _mapTileLibrary.DefaultMapTileData;
        
        private void Start()
        {
            PoolingManager.Instance.PrepareMapTiles(_mapTileContent, _defaultMapTilePoolSize);
            PoolingManager.Instance.PrepareUnits(_unitsContent, _defaultUnitPoolSize);
            GenerateGridAndTiles();
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
            if (_mapTiles.Any())
            {
                foreach (var mapTile in _mapTiles.Values)
                {
                    PoolingManager.Instance.ReturnToPool(PoolObjectType.MapTiles, mapTile);
                }
            }
            _mapTiles.Clear();
        }
        
        public void LockInput() => _isAnyActionActive = true;
        public void UnlockInput() => _isAnyActionActive = false;
        
        public void HandlePathfindingRequest(MapTile targetMapTile)
        {
            if (_isAnyActionActive) return;

            var playerUnit = UnitManager.Instance.PlayerUnit;
            if (playerUnit == null) return;

            var playerUnitData = playerUnit.UnitData as PlayerUnitData;
            if (playerUnitData == null) return;
            
            if (_currentTargetTile != null && targetMapTile == _currentTargetTile)
            {
                if (_currentHighlightedPath.Count > 0 && _currentHighlightedPath.Count <= playerUnitData.MoveRange)
                {
                    UnitManager.Instance.MoveUnitAlongPath(playerUnit, _currentHighlightedPath);
                    SetDefaultColorsForPreviousPath();
                }
            }
            else
            {
                ClearPreviousPathHighlight();
            
                MapTile startTileObject = playerUnit.CurrentTile;
                Vector3 startPosition = startTileObject.transform.position;
                Vector3 targetPosition = targetMapTile.transform.position;
    
                var path = PathfindingManager.Instance.FindPath(startPosition, targetPosition);

                if (path != null && path.Count > 0)
                {
                    HighlightPath(path, startTileObject, playerUnitData.MoveRange);
                    _currentHighlightedPath = path;
                    _currentTargetTile = targetMapTile;
                }
            }
        }

        public void ClearPreviousPathHighlight()
        {
            SetDefaultColorsForPreviousPath();
            _previousPathTiles.Clear();
            _currentHighlightedPath.Clear();
            _currentTargetTile = null;
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

        private void HighlightPath(List<Vector3> path, MapTile startTileObject, int actionRange)
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

                var pathfindingGrid = PathfindingManager.Instance.GetPathfindingGrid();
                PathfindingNode nodeToUpdate = pathfindingGrid.GetNode(gridPosition);

                nodeToUpdate?.SetWalkable(tileToUpdate.IsWalkable);
                nodeToUpdate?.SetAttackableThrough(tileToUpdate.IsAttackableThrough);
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
                    tileToUpdate.UpdateTile(DefaultMapTileData);
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
