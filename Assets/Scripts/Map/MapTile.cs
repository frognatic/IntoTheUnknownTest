using IntoTheUnknownTest.Managers;
using IntoTheUnknownTest.Map;
using UnityEngine;

namespace IntoTheUnknownTest
{
    public class MapTile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private SpriteRenderer _selector;
        [SerializeField] private MapTileUnit _mapTileUnit;
        
        public Vector2Int GridPosition { get; private set; }
        
        private BaseMapTileData _mapTileData;

        public void InitTile(BaseMapTileData mapTileData, Vector2Int gridPosition)
        {
            GridPosition = gridPosition;
            
            _mapTileData = mapTileData;
            _spriteRenderer.sprite = _mapTileData.MapElementSprite;
            
            ResetSlot();
        }
        
        public void UpdateTile(BaseMapTileData newMapTileDataData)
        {
            _mapTileData = newMapTileDataData;
            _spriteRenderer.sprite = _mapTileData.MapElementSprite;
            ResetSlot();
        }

        public void SetElementOnSlot(BaseUnitData unitData)
        {
            UpdateTile(MapTileManager.Instance.DefaultMapTileData);
            _mapTileUnit.SetUnit(unitData);
        }

        public void ResetSlot()
        {
            _mapTileUnit.ResetSlot();
        }

        public void SetColor(Color colorToSet)
        {
            _selector.color = colorToSet;
        }
    }
}
