using IntoTheUnknownTest.Units;
using UnityEngine;

namespace IntoTheUnknownTest.Map
{
    [CreateAssetMenu(fileName = "PlayerUnitData", menuName = "Data/Units/PlayerUnitData")]
    public class PlayerUnitData : BaseUnitData, IAttackable
    {
        [SerializeField] private int _moveRange;
        [SerializeField] private int _attackRange;
        
        public override bool IsWalkable => false;
        public override bool IsAttackableThrough => false;
        
        public int MoveRange => _moveRange;
        public int AttackRange => _attackRange;
    }
}
