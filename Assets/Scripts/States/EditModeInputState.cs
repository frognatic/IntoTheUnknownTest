using IntoTheUnknownTest.Managers;
using UnityEngine;

namespace IntoTheUnknownTest.States
{
    public class EditModeInputState : IInputState
    {
        public void HandleRaycastClick(RaycastHit2D hit)
        {
            MapTile clickedTile = hit.collider.GetComponent<MapTile>();
            if (clickedTile != null)
            {
                EditMapManager.Instance.HandleTileEditClick(clickedTile);
            }
        }

        public void OnEnter() {}
        public void OnExit() {}
    }
}
