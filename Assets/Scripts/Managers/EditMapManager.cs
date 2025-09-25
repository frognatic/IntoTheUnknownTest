using IntoTheUnknownTest.Map;
using UnityEngine;

namespace IntoTheUnknownTest.Managers
{
    public class EditMapManager : Singleton<EditMapManager>
    {
        private BaseMapTile _selectedTile;

        protected override void Awake()
        {
            base.Awake();
            
            _selectedTile = null;
        }
        
        public void SelectTile(BaseMapTile tile)
        {
            _selectedTile = tile;
        }

        public void HandleTileEditClick(MapTile clickedTile)
        {
            ReplaceTile(clickedTile);
        }

        private void ReplaceTile(MapTile clickedTile)
        {
            Vector2Int gridPosition = clickedTile.GridPosition;
            MapTileManager.Instance.TryUpdateTileAt(gridPosition, _selectedTile);
        }
    }
}