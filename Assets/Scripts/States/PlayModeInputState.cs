using IntoTheUnknownTest.Managers;
using UnityEngine;

namespace IntoTheUnknownTest.States
{
    public class PlayModeInputState : IInputState
    {
        public void HandleRaycastClick(RaycastHit2D hit)
        {
            MapTileManager.Instance.HandlePathfindingRequest(hit.point);
        }

        public void OnEnter() {}
        public void OnExit() {}
    }
}
