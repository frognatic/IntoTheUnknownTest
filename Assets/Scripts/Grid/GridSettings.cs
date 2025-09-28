using System;
using UnityEngine;

namespace IntoTheUnknownTest.Grid
{
    [Serializable]
    [CreateAssetMenu(fileName = "Grid Settings", menuName = "Data/Grid/Grid Settings")]
    public class GridSettings : ScriptableObject
    {
        [Tooltip("Min size is 3x3")]
        [SerializeField] private Vector2Int _gridWorldSize;
        [SerializeField] private bool _useDiagonalMoveCalculations = false;
        [SerializeField] private float _nodeRadius;
        
        public Vector2Int GridWorldSize => _gridWorldSize;
        public float NodeRadius => _nodeRadius;
        public float NodeDiameter => _nodeRadius * 2;
        public bool UseDiagonalMoveCalculations => _useDiagonalMoveCalculations;
        
        public int GetGridX => Mathf.RoundToInt(_gridWorldSize.x / NodeDiameter);
        public int GetGridY => Mathf.RoundToInt(_gridWorldSize.y / NodeDiameter);
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            _gridWorldSize.x = Mathf.Max(3, _gridWorldSize.x);
            _gridWorldSize.y = Mathf.Max(3, _gridWorldSize.y);
        }
#endif
    }
}
