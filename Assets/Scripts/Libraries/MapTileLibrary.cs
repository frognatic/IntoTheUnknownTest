using System.Collections.Generic;
using IntoTheUnknownTest.Map;
using UnityEngine;

namespace IntoTheUnknownTest.Libraries
{
    [CreateAssetMenu(fileName = "MapTileLibrary", menuName = "Data/Library/MapTileLibrary")]
    public class MapTileLibrary : ScriptableObject
    {
        [SerializeField] private List<BaseMapTileData> _mapTiles;
        [SerializeField] private BaseMapTileData _defaultMapTileData;
        
        public List<BaseMapTileData> MapTiles => _mapTiles;
        public BaseMapTileData DefaultMapTileData => _defaultMapTileData;
    }
}
