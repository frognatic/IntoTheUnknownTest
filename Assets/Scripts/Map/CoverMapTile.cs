using UnityEngine;

namespace IntoTheUnknownTest.Map
{
    [CreateAssetMenu(fileName = "CoverMapTile", menuName = "Data/MapTiles/CoverMapTile")]
    public class CoverMapTile : BaseMapTile
    {
        public override bool IsWalkable => true;
    }
}
