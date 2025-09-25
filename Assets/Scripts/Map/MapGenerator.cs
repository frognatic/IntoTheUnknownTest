using IntoTheUnknownTest.Libraries;
using IntoTheUnknownTest.Pathfinding;

namespace IntoTheUnknownTest.Map
{
    public class MapGenerator
    {
        private MapTileLibrary _mapTileLibrary;

        public MapGenerator(MapTileLibrary mapTileLibrary)
        {
            _mapTileLibrary = mapTileLibrary;
        }

        public void GenerateGridData(PathfindingGrid pathfindingGrid)
        {
            pathfindingGrid.InitializeGrid();

            foreach (var node in pathfindingGrid.Grid)
            {
                BaseMapTile tileData = _mapTileLibrary.DefaultMapTile;
                node.SetWalkable(tileData.IsWalkable);
            }
        }
    }
}
