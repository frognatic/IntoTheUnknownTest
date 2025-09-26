using UnityEngine;

namespace IntoTheUnknownTest.Map
{
    [CreateAssetMenu(fileName = "EnemyUnitData", menuName = "Data/Units/EnemyUnitData")]
    public class EnemyUnitData : BaseUnitData
    {
        public override bool IsWalkable => false;
        public override bool IsAttackableThrough => false;
        public override bool IsUniqueOnMap => true;
    }
}
