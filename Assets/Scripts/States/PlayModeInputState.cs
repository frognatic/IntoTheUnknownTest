using IntoTheUnknownTest.Managers;

namespace IntoTheUnknownTest.States
{
    public class PlayModeInputState : IInputState
    {
        public void HandleRaycastClick(MapTile mapTile)
        {
            MapTileManager.Instance.HandlePathfindingRequest(mapTile);
        }

        public void OnEnter() {}
        public void OnExit() {}
    }
}
