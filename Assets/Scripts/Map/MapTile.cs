using IntoTheUnknownTest.Map;
using UnityEngine;

namespace IntoTheUnknownTest
{
    public class MapTile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        public Vector2Int GridPosition { get; private set; } 
        private BaseMapTile _mapTile;

        public void InitTile(BaseMapTile mapTile, Vector2Int gridPosition)
        {
            GridPosition = gridPosition;
            
            _mapTile = mapTile;
            _spriteRenderer.sprite = _mapTile.TileSprite;
        }
        
        public void UpdateTile(BaseMapTile newMapTileData)
        {
            _mapTile = newMapTileData;
            _spriteRenderer.sprite = _mapTile.TileSprite;
        }

        public void SetColor(Color colorToSet)
        {
            _spriteRenderer.color = colorToSet;
        }
    }
}
