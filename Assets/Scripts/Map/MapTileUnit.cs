using IntoTheUnknownTest.Map;
using UnityEngine;

namespace IntoTheUnknownTest
{
    public class MapTileUnit : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _unitSpriteRenderer;

        private BaseUnitData _unitData;
        
        public void SetUnit(BaseUnitData unitData)
        {
            _unitData = unitData;
            _unitSpriteRenderer.sprite = unitData.MapElementSprite;
        }

        public void ResetSlot()
        {
            _unitData = null;
            _unitSpriteRenderer.sprite = null;
        }
    }
}
