using UnityEngine;

namespace IntoTheUnknownTest.Map
{
    public abstract class BaseUnitData : ScriptableObject, IMapElement
    {
        [SerializeField] private string _id;
        [SerializeField] private Sprite _mapElementSprite;
        [Tooltip("If the value is -1, then there are no limits")]
        [SerializeField] private int _placingLimit;
        
        public string Id => _id;
        public Sprite MapElementSprite => _mapElementSprite;
        public int PlacingLimit => _placingLimit;
        public abstract bool IsWalkable { get; }
        public abstract bool IsAttackableThrough { get; }
        public bool HasOnlyOneInstance => _placingLimit == 1;
    }
}
