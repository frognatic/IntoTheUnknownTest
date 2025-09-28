namespace IntoTheUnknownTest.Map
{
    public interface IActionState
    {
        void OnEnter(MapTile targetTile, MapTileUnit unit);
        void PerformAction();
        void OnExit();
    }
}
