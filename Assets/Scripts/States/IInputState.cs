using UnityEngine;

namespace IntoTheUnknownTest.States
{
    public interface IInputState
    {
        void HandleRaycastClick(RaycastHit2D hit);
        void OnEnter();
        void OnExit();
    }
}
