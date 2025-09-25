using UnityEngine;

namespace IntoTheUnknownTest.Map
{
    public abstract class BaseMapTileData : ScriptableObject, IMapElement
    {
        [SerializeField] private string _id;
        [SerializeField] private Sprite _mapElementSprite;
        
        public Sprite MapElementSprite => _mapElementSprite;
        public string Id => _id;
        public abstract bool IsWalkable { get; }
    }
}
