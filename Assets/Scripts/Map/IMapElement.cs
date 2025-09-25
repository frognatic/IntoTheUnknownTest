using UnityEngine;

namespace IntoTheUnknownTest.Map
{
    public interface IMapElement
    {
        int PlacingLimit { get; }
        string Id { get; }
        Sprite MapElementSprite { get; }
        bool IsWalkable { get; }
        bool IsAttackableThrough { get; }
    }
}
