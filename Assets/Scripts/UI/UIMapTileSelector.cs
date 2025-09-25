using System;
using IntoTheUnknownTest.Managers;
using IntoTheUnknownTest.Map;
using UnityEngine;
using UnityEngine.UI;

namespace IntoTheUnknownTest.UI
{
    public class UIMapTileSelector : MonoBehaviour
    {
        [SerializeField] private Image _mapTileImage;
        [SerializeField] private Image _selectionImage;

        private BaseMapTile _mapTile;
        private Action _hideAllSelections;
        
        public void Init(BaseMapTile mapTile, Action hideAllSelections)
        {
            _mapTile = mapTile;
            _hideAllSelections = hideAllSelections;
            PrepareImage();
            HideSelection();
        }

        private void PrepareImage()
        {
            _mapTileImage.sprite = _mapTile.TileSprite;
        }

        public void OnButtonClick()
        {
            _hideAllSelections?.Invoke();
            ShowSelection();
            EditMapManager.Instance.SelectTile(_mapTile);
        }

        public void ShowSelection()
        {
            _selectionImage.gameObject.SetActive(true);
        }

        public void HideSelection()
        {
            _selectionImage.gameObject.SetActive(false);
        }
    }
}
