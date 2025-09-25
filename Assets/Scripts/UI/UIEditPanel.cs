using System.Collections.Generic;
using IntoTheUnknownTest.Managers;
using IntoTheUnknownTest.Utilities;
using UnityEngine;

namespace IntoTheUnknownTest.UI
{
    public class UIEditPanel : UIBasePanel
    {
        [Header("Selector prefabs")]
        [SerializeField] private UIMapTileSelector _mapTileSelectorPrefab;
        [SerializeField] private Transform _mapTileSelectorContent;
        
        private List<UIMapTileSelector> _mapTileSelectors = new();

        public override void Show()
        {
            base.Show();
            PrepareTiles();
        }
        
        private void PrepareTiles()
        {
            _mapTileSelectors.Clear();
            _mapTileSelectorContent.DestroyAllChildren();
            
            foreach (var mapTile in MapTileManager.Instance.MapTiles)
            {
                var mapTileSelector = Instantiate(_mapTileSelectorPrefab, _mapTileSelectorContent);
                mapTileSelector.Init(mapTile, HideAllSelections);
                _mapTileSelectors.Add(mapTileSelector);
                
            }
        }

        public void HideAllSelections()
        {
            _mapTileSelectors.ForEach(x => x.HideSelection());
        }
    }
}
