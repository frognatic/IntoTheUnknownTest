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
        [SerializeField] private int _movementRange = 4;
        
        [SerializeField] private Transform _mapTileContent;
        
        [Header("Colors")]
        [SerializeField] private Color _startTileColor = Color.yellow;
        [SerializeField] private Color _inRangeColor = Color.green;
        [SerializeField] private Color _outOfRangeColor = Color.red;
        [SerializeField] private Color _defaultTileColor = Color.white;
        
        private List<MapTile> _previousPathTiles = new List<MapTile>();
        private Dictionary<Vector2Int, MapTile> _mapTiles = new Dictionary<Vector2Int, MapTile>();
        
        private PlayerUnitData _playerUnitData;
        
        private const int _defaultMapTilePoolSize = 100;
        
        public List<BaseMapTileData> MapTiles => _mapTileLibrary.MapTiles;
        public BaseMapTileData DefaultMapTileData => _mapTileLibrary.DefaultMapTileData;
        
        private void Start()
        {
            PoolingManager.Instance.PrepareMapTiles(_mapTileContent, _defaultMapTilePoolSize);
            GenerateGridAndTiles();
        }
        
        public void GenerateGridAndTiles()
        {
            if (_mapTiles.Any())
            {
                foreach (var mapTile in _mapTiles.Values)
                {
                    PoolingManager.Instance.ReturnToPool(PoolObjectType.MapTiles, mapTile);
                }
            }
            _mapTiles.Clear();
    
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
        
        public void HandlePathfindingRequest(Vector3 targetPosition)
        {
            ClearPreviousPathHighlight();
            if (_mapTiles.Any())
            {
                MapTile startTileObject = _mapTiles.First().Value;
                startTileObject.SetColor(_defaultTileColor);
                Vector3 startPosition = startTileObject.transform.position;
            
                var path = PathfindingManager.Instance.FindPath(startPosition, targetPosition);

                if (path != null && path.Count > 0)
                {
                    HighlightPath(path, startTileObject);
                    Debug.LogWarning($"PATH COST: {path.Count}");
                }
            }
        }

        private void ClearPreviousPathHighlight()
        {
            foreach (var tile in _previousPathTiles)
            {
                if (tile != null)
                {
                    tile.SetColor(_defaultTileColor);
                }
            }
            _previousPathTiles.Clear();
        }

        private void HighlightPath(List<Vector3> path, MapTile startTileObject)
        {
            var tilesOnPath = GetTilesByPositions(path);

            for (int i = 0; i < tilesOnPath.Count; i++)
            {
                MapTile currentTile = tilesOnPath[i];

                int stepsToTile = i + 1;
                currentTile.SetColor(stepsToTile <= _movementRange ? _inRangeColor : _outOfRangeColor);
            }

            startTileObject.SetColor(_startTileColor);
                
            _previousPathTiles = tilesOnPath;
            _previousPathTiles.Add(startTileObject);
        }
        
        public void SetTileToDefault(Vector2Int gridPosition)
        {
            TryUpdateTile(gridPosition, DefaultMapTileData);
        }

        public void TryUpdateTile(Vector2Int gridPosition, IMapElement mapElement)
        {
            if (_mapTiles.TryGetValue(gridPosition, out MapTile tileToUpdate))
            {
                SelectTileUpdateAction(mapElement, tileToUpdate);

                var pathfindingGrid = PathfindingManager.Instance.GetPathfindingGrid();
                PathfindingNode nodeToUpdate = pathfindingGrid.GetNode(gridPosition);

                nodeToUpdate?.SetWalkable(mapElement.IsWalkable);
                nodeToUpdate?.SetAttackableThrough(mapElement.IsAttackableThrough);
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
                    tileToUpdate.SetElementOnSlot(selectedUnitData);
                    if (selectedUnitData is PlayerUnitData playerUnitData)
                    {
                        _playerUnitData = playerUnitData;
                    }
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
