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
            TryUpdateTile(clickedTile, _selectedMapElement);
        }

        private void TryUpdateTile(MapTile clickedTile, IMapElement mapElement)
        {
            Vector2Int gridPosition = clickedTile.GridPosition;
            MapTileManager.Instance.TryUpdateTile(gridPosition, mapElement);
        }
    }
}