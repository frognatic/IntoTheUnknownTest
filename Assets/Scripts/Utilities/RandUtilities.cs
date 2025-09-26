using UnityEngine;

namespace IntoTheUnknownTest.Utilities
{
    public static class RandUtilities
    {
        public static int GetRandomValueFromRange(Vector2Int range)
        {
            // For Random.Range int maximum parameter is exclusive, so we need to add +1
            return Random.Range(range.x, range.y + 1);
        }
    }
}
