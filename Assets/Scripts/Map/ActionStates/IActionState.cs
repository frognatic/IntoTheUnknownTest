namespace IntoTheUnknownTest.Map
{
    public interface IActionState
    {
        int ActionRange { get; }
        void OnEnter(MapTile targetTile, MapTileUnit unit);
        void PerformAction();
        void OnExit();
    }
}
