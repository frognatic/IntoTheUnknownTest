using System.Collections.Generic;
using IntoTheUnknownTest.Managers;
using IntoTheUnknownTest.Map;
using IntoTheUnknownTest.Utilities;
using UnityEngine;

namespace IntoTheUnknownTest.UI
{
    public class UIEditPanel : UIBasePanel
    {
        [Header("Selector prefabs")]
        [SerializeField] private UIEditMapItemSelector _editMapItemSelectorPrefab;
        [SerializeField] private Transform _selectorsContent;
        
        private readonly List<UIEditMapItemSelector> _selectors = new();

        public override void Show()
        {
            base.Show();
            PrepareMapItems();
        }
        
        private void PrepareMapItems()
        {
            _selectors.Clear();
            _selectorsContent.DestroyAllChildren();
            
            foreach (var mapTile in MapTileManager.Instance.MapTiles)
            {
                CreateMapElement(mapTile);
            }

            foreach (var unit in UnitManager.Instance.Units)
            {
                CreateMapElement(unit);
            }
        }

        private void CreateMapElement(IMapElement mapElement)
        {
            var mapElementSelector = Instantiate(_editMapItemSelectorPrefab, _selectorsContent);
            mapElementSelector.Init(mapElement, HideAllSelections);
            _selectors.Add(mapElementSelector);
        }

        private void HideAllSelections()
        {
            _selectors.ForEach(x => x.HideSelection());
        }
    }
}
