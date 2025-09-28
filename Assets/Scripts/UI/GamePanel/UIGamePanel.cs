using IntoTheUnknownTest.Libraries;
using IntoTheUnknownTest.Managers;
using IntoTheUnknownTest.Utilities;
using UnityEngine;

namespace IntoTheUnknownTest.UI
{
    public class UIGamePanel : UIBasePanel
    {
        [SerializeField] private UIGameLegend _legendPrefab;
        [SerializeField] private Transform _legendContent;

        public override void Show()
        {
            base.Show();
            
            _legendContent.DestroyAllChildren();

            foreach (var tileSelectorConfig in MapTileManager.Instance.TileSelectorConfig)
            {
                if (tileSelectorConfig.TileSelectorActionType == TileSelectorActionType.Default) continue;
                
                var legendObj = Instantiate(_legendPrefab, _legendContent);
                legendObj.Init(tileSelectorConfig);
            }
        }
        
        protected override void OnBackButtonClicked()
        {
            base.OnBackButtonClicked();
            MapTileManager.Instance.ClearPreviousPathHighlight();
        }
    }
}
