using UnityEngine;

namespace IntoTheUnknownTest.Map
{
    public abstract class BaseMapTile : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private Sprite _tileSprite;
        
        public string Id => _id;
        public Sprite TileSprite => _tileSprite;
        
        public Vector2Int GridPosition { get; private set; }
        public abstract bool IsWalkable { get; }
        
        public void Initialize(Vector2Int gridPosition)
        {
            this.GridPosition = gridPosition;
        }
    }
}
