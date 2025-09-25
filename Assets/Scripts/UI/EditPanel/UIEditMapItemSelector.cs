using System;
using IntoTheUnknownTest.Managers;
using IntoTheUnknownTest.Map;
using UnityEngine;
using UnityEngine.UI;

namespace IntoTheUnknownTest.UI
{
    public class UIEditMapItemSelector : MonoBehaviour
    {
        [SerializeField] private Image _itemImage;
        [SerializeField] private Image _selectionImage;

        private IMapElement _mapElement;
        private Action _hideAllSelectionsAction;
        
        public void Init(IMapElement mapElement, Action hideAllSelectionsAction)
        {
            _mapElement = mapElement;
            _hideAllSelectionsAction = hideAllSelectionsAction;
            
            PrepareImage();
            HideSelection();
        }

        private void PrepareImage()
        {
            _itemImage.sprite = _mapElement.MapElementSprite;
        }

        public void OnButtonClick()
        {
            _hideAllSelectionsAction?.Invoke();
            ShowSelection();
            SelectAction();
        }

        private void SelectAction()
        {
            EditMapManager.Instance.SelectTile(_mapElement);
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
