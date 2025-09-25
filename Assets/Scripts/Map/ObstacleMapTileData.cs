using UnityEngine;

namespace IntoTheUnknownTest.Map
{
    [CreateAssetMenu(fileName = "ObstacleMapTile", menuName = "Data/MapTiles/ObstacleMapTile")]
    public class ObstacleMapTileData : BaseMapTileData
    {
        public override bool IsWalkable => false;
        public override bool IsAttackableThrough => false;
    }
}
