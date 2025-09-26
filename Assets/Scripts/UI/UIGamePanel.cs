using IntoTheUnknownTest.Managers;

namespace IntoTheUnknownTest.UI
{
    public class UIGamePanel : UIBasePanel
    {
        protected override void OnBackButtonClicked()
        {
            base.OnBackButtonClicked();
            MapTileManager.Instance.ClearPreviousPathHighlight();
        }
    }
}
