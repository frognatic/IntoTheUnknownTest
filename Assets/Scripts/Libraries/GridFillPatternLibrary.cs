using System;
using System.Collections.Generic;
using IntoTheUnknownTest.Map;
using UnityEngine;

namespace IntoTheUnknownTest.Libraries
{
    [CreateAssetMenu(fileName = "GridFillPatternLibrary", menuName = "Data/Library/GridFillPatternLibrary")]
    public class GridFillPatternLibrary : ScriptableObject
    {
        [SerializeField] private List<GridTiles> _mapElements;
        [SerializeField] private List<GridUnits> _unitElements;
        
        public List<GridTiles> MapElements => _mapElements;
        public List<GridUnits> UnitElements => _unitElements;
    }

    [Serializable]
    public class GridTiles : IGridRandomPattern
    {
        public BaseMapTileData MapElement;
        public Vector2Int ElementsToGenerate;

        public IMapElement MapElementToPlace => MapElement;
        public Vector2Int ElementsRange => ElementsToGenerate;
    }
    
    [Serializable]
    public class GridUnits : IGridRandomPattern
    {
        public BaseUnitData UnitData;
        public Vector2Int ElementsToGenerate;
        
        public IMapElement MapElementToPlace => UnitData;
        public Vector2Int ElementsRange => ElementsToGenerate;
    }

    public interface IGridRandomPattern
    {
        IMapElement MapElementToPlace { get; }
        Vector2Int ElementsRange { get; }
    }
}
