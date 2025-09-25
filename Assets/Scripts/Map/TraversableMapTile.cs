
using UnityEngine;

namespace IntoTheUnknownTest.Map
{
    [CreateAssetMenu(fileName = "TraversableMapTile", menuName = "Data/MapTiles/TraversableMapTile")]
    public class TraversableMapTile : BaseMapTile
    {
        public override bool IsWalkable => true;
    }
}
