namespace IntoTheUnknownTest.States
{
    public interface IInputState
    {
        void HandleRaycastClick(MapTile mapTile);
        void OnEnter();
        void OnExit();
    }
}
