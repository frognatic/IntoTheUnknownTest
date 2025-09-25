using UnityEngine;

namespace IntoTheUnknownTest.Map
{
    [CreateAssetMenu(fileName = "PlayerUnitData", menuName = "Data/Units/PlayerUnitData")]
    public class PlayerUnitData : BaseUnitData
    {
        public override bool IsWalkable => false;
        public override bool IsAttackableThrough => false;
    }
}
