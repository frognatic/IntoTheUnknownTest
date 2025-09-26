using IntoTheUnknownTest.Managers;

namespace IntoTheUnknownTest.States
{
    public class EditModeInputState : IInputState
    {
        public void HandleRaycastClick(MapTile mapTile)
        {
            if (mapTile != null)
            {
                EditMapManager.Instance.HandleTileEditClick(mapTile);
            }
        }

        public void OnEnter() {}
        public void OnExit() {}
    }
}
