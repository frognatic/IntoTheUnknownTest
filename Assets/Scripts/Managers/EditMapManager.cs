using System.Collections.Generic;
using System.Linq;
using IntoTheUnknownTest.Map;
using UnityEngine;

namespace IntoTheUnknownTest.Managers
{
    public class EditMapManager : Singleton<EditMapManager>
    {
        private IMapElement _selectedMapElement;
        private readonly Dictionary<IMapElement, MapTile> _uniqueElementPositions = new Dictionary<IMapElement, MapTile>();

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
            if (_selectedMapElement == null) return;

            ClearUniqueElementIfOverwritten(clickedTile);
            ClearPreviousPositionOfUniqueElement();
            TryUpdateTile(clickedTile, _selectedMapElement);
            TryCacheUniqueElement(clickedTile);
        }

        private void ClearUniqueElementIfOverwritten(MapTile clickedTile)
        {
            var existingEntry = _uniqueElementPositions.FirstOrDefault(kvp => kvp.Value == clickedTile);
            
            if (existingEntry.Key != null)
            {
                _uniqueElementPositions.Remove(existingEntry.Key);
            }
        }

        private void ClearPreviousPositionOfUniqueElement()
        {
            if (_selectedMapElement == null || !_selectedMapElement.IsUniqueOnMap) return;
            
            if (_uniqueElementPositions.TryGetValue(_selectedMapElement, out MapTile previousTile))
            {
                MapTileManager.Instance.SetTileToDefault(previousTile.GridPosition);
                _uniqueElementPositions.Remove(_selectedMapElement);
            }
        }

        private void TryCacheUniqueElement(MapTile clickedTile)
        {
            if (_selectedMapElement.IsUniqueOnMap)
            {
                _uniqueElementPositions[_selectedMapElement] = clickedTile;
            }
        }

        private void TryUpdateTile(MapTile clickedTile, IMapElement mapElement)
        {
            Vector2Int gridPosition = clickedTile.GridPosition;
            MapTileManager.Instance.TryUpdateTile(gridPosition, mapElement);
        }
    }
}