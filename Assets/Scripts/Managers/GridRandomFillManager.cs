using System.Collections.Generic;
using System.Linq;
using IntoTheUnknownTest.Libraries;
using IntoTheUnknownTest.Pathfinding;
using IntoTheUnknownTest.Utilities;
using UnityEngine;

namespace IntoTheUnknownTest.Managers
{
    public class GridRandomFillManager : Singleton<GridRandomFillManager>
    {
        [SerializeField] private GridFillPatternLibrary _gridFillPatternLibrary;

        public void GenerateRandomElements()
        {
            if (_gridFillPatternLibrary == null) return;

            MapTileManager.Instance.GenerateGridAndTiles();

            PathfindingGrid pathfindingGrid = PathfindingManager.Instance.GetPathfindingGrid();
            if (pathfindingGrid == null) return;

            List<Vector2Int> availablePositions = pathfindingGrid.Grid
                .Cast<PathfindingNode>()
                .Where(node => node != null && node.IsWalkable)
                .Select(node => node.GridPosition)
                .ToList();
            
            if (!availablePositions.Any()) return;

            PlaceElements(availablePositions, _gridFillPatternLibrary.UnitElements.ToList<IGridRandomPattern>());
            PlaceElements(availablePositions, _gridFillPatternLibrary.MapElements.ToList<IGridRandomPattern>());
        }

        private void PlaceElements(List<Vector2Int> availablePositions, List<IGridRandomPattern> elements)
        {
            foreach (var element in elements)
            {
                int amountToSpawn = RandUtilities.GetRandomValueFromRange(element.ElementsRange);
                for (int i = 0; i < amountToSpawn; i++)
                {
                    if (availablePositions.Count == 0) return;

                    int randomIndex = RandUtilities.GetRandomValueFromRange(new Vector2Int(0, availablePositions.Count - 1));
                    Vector2Int randomPosition = availablePositions[randomIndex];

                    MapTileManager.Instance.TryUpdateTile(randomPosition, element.MapElementToPlace);
                    availablePositions.RemoveAt(randomIndex);
                }
            }
        }
    }
}
