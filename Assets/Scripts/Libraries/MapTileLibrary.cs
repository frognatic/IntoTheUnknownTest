using System.Collections.Generic;
using IntoTheUnknownTest.Map;
using UnityEngine;

namespace IntoTheUnknownTest.Libraries
{
    [CreateAssetMenu(fileName = "MapTileLibrary", menuName = "Data/Library/MapTileLibrary")]
    public class MapTileLibrary : ScriptableObject
    {
        [SerializeField] private List<BaseMapTile> _mapTiles;
        [SerializeField] private BaseMapTile _defaultMapTile;
        
        public List<BaseMapTile> MapTiles => _mapTiles;
        public BaseMapTile DefaultMapTile => _defaultMapTile;
    }
}
