using IntoTheUnknownTest.Map;
using UnityEngine;

namespace IntoTheUnknownTest.Managers
{
    public class EditMapManager : Singleton<EditMapManager>
    {
        private IMapElement _selectedMapElement;

        protected override void Awake()
        {
            base.Awake();
            
            _selectedMapElement = null;
        }
        
        public void SelectTile(IMapElement tileData)
        {
            _selectedMapElement = tileData;
        }

        public void HandleTileEditClick(MapTile clickedTile)
        {
            if (_selectedMapElement is BaseMapTileData selectedTileData)
            {
                ReplaceTile(clickedTile, selectedTileData);
            }

            if (_selectedMapElement is BaseUnitData selectedUnitData)
            {
                PlaceUnit(clickedTile, selectedUnitData);
            }
        }

        private void ReplaceTile(MapTile clickedTile, BaseMapTileData selectedTileData)
        {
            Vector2Int gridPosition = clickedTile.GridPosition;
            MapTileManager.Instance.TryUpdateTileAt(gridPosition, selectedTileData);
        }

        private void PlaceUnit(MapTile clickedTile, BaseUnitData selectedUnitData)
        {
            Vector2Int gridPosition = clickedTile.GridPosition;
            MapTileManager.Instance.TryUpdateSlotAtTile(gridPosition, selectedUnitData);
        }
    }
}