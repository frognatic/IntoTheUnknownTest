using UnityEngine;

namespace IntoTheUnknownTest.Map
{
    [CreateAssetMenu(fileName = "ObstacleMapTile", menuName = "Data/MapTiles/ObstacleMapTile")]
    public class ObstacleMapTile : BaseMapTile
    {
        public override bool IsWalkable => false;
    }
}
