using UnityEngine;

namespace IntoTheUnknownTest.Map
{
    public class BaseUnitData : ScriptableObject, IMapElement
    {
        [SerializeField] private string _id;
        [SerializeField] private Sprite _mapElementSprite;
        
        public string Id => _id;
        public Sprite MapElementSprite => _mapElementSprite;
    }
}
