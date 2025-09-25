using UnityEngine;

namespace IntoTheUnknownTest.Map
{
    [CreateAssetMenu(fileName = "CoverMapTile", menuName = "Data/MapTiles/CoverMapTile")]
    public class CoverMapTileData : BaseMapTileData
    {
        public override bool IsWalkable => false;
        public override bool IsAttackableThrough => true;
    }
}
