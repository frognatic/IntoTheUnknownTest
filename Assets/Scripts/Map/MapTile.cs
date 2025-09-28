using IntoTheUnknownTest.Managers;
using IntoTheUnknownTest.Map;
using UnityEngine;

namespace IntoTheUnknownTest
{
    public class MapTile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private SpriteRenderer _selector;
        
        public Vector2Int GridPosition { get; private set; }
        public MapTileUnit OccupyingUnit { get; set; }
        
        private BaseMapTileData _mapTileData;
        
        public bool IsWalkable => _mapTileData.IsWalkable;
        public bool IsAttackableThrough => _mapTileData.IsAttackableThrough;

        public void InitTile(BaseMapTileData mapTileData, Vector2Int gridPosition)
        {
            GridPosition = gridPosition;
            
            _mapTileData = mapTileData;
            _spriteRenderer.sprite = _mapTileData.MapElementSprite;

            OccupyingUnit = null;
        }
        
        public void UpdateTile(BaseMapTileData newMapTileDataData)
        {
            _mapTileData = newMapTileDataData;
            _spriteRenderer.sprite = _mapTileData.MapElementSprite;
            
            if (OccupyingUnit != null)
            {
                UnitManager.Instance.DespawnUnit(OccupyingUnit);
            }
        }

        public void SetOccupant(MapTileUnit newUnit)
        {
            OccupyingUnit = newUnit;
        }

        public void ClearOccupant()
        {
            OccupyingUnit = null;
        }

        public void SetColor(Color colorToSet)
        {
            _selector.color = colorToSet;
        }
    }
}
