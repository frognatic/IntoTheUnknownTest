using IntoTheUnknownTest.Map;
using UnityEngine;

namespace IntoTheUnknownTest
{
    public class MapTileUnit : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _unitSpriteRenderer;
        
        public MapTile CurrentTile { get; private set; }
        public BaseUnitData UnitData { get; private set; }
        
        public void Setup(BaseUnitData unitData, MapTile mapTile)
        {
            CurrentTile = mapTile;
            UnitData  = unitData;
            _unitSpriteRenderer.sprite = unitData.MapElementSprite;
            transform.position = mapTile.transform.position;
        }
        
        public void SetCurrentTile(MapTile newTile)
        {
            CurrentTile = newTile;
        }
    }
}
