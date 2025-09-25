using UnityEngine;

namespace IntoTheUnknownTest.Map
{
    public abstract class BaseUnitData : ScriptableObject, IMapElement
    {
        [SerializeField] private string _id;
        [SerializeField] private Sprite _mapElementSprite;
        
        public string Id => _id;
        public Sprite MapElementSprite => _mapElementSprite;
        public abstract bool IsWalkable { get; }
        public abstract bool IsAttackableThrough { get; }
    }
}
