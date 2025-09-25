using IntoTheUnknownTest.Map;
using UnityEngine;

namespace IntoTheUnknownTest
{
    public class MapTile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private SpriteRenderer _slotElement;
        [SerializeField] private SpriteRenderer _selector;
        
        public Vector2Int GridPosition { get; private set; }
        
        private BaseMapTileData _mapTileData;
        private BaseUnitData _slotUnitData;

        public void InitTile(BaseMapTileData mapTileData, Vector2Int gridPosition)
        {
            GridPosition = gridPosition;
            
            _mapTileData = mapTileData;
            _spriteRenderer.sprite = _mapTileData.MapElementSprite;
        }
        
        public void UpdateTile(BaseMapTileData newMapTileDataData)
        {
            _mapTileData = newMapTileDataData;
            _spriteRenderer.sprite = _mapTileData.MapElementSprite;
            ResetSlot();
        }

        public void SetElementOnSlot(BaseUnitData unitData)
        {
            _slotUnitData = unitData;
            _slotElement.sprite = _slotUnitData.MapElementSprite;
        }

        public void ResetSlot()
        {
            _slotUnitData = null;
            _slotElement.sprite = null;
        }

        public void SetColor(Color colorToSet)
        {
            _selector.color = colorToSet;
        }
    }
}
