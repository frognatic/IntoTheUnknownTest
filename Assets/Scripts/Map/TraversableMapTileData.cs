
using UnityEngine;

namespace IntoTheUnknownTest.Map
{
    [CreateAssetMenu(fileName = "TraversableMapTile", menuName = "Data/MapTiles/TraversableMapTile")]
    public class TraversableMapTileData : BaseMapTileData
    {
        public override bool IsWalkable => true;
    }
}
