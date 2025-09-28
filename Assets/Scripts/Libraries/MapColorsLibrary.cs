using System;
using System.Collections.Generic;
using UnityEngine;

namespace IntoTheUnknownTest.Libraries
{
    [CreateAssetMenu(fileName = "MapColorsLibrary", menuName = "Data/Library/MapColorsLibrary")]
    public class MapColorsLibrary : ScriptableObject
    {
        [SerializeField] private List<TileSelectorConfig> _tileSelectorConfigs;
        
        public List<TileSelectorConfig> TileSelectorConfigs => _tileSelectorConfigs;
        public Color GetColorForAction(TileSelectorActionType tileSelectorActionType) => _tileSelectorConfigs.Find(x => x.TileSelectorActionType == tileSelectorActionType).Color;
    }

    [Serializable]
    public class TileSelectorConfig
    {
        public TileSelectorActionType TileSelectorActionType;
        public Color Color;
        public string Name;
    }

    public enum TileSelectorActionType
    {
        Default,
        StartTile,
        InRange,
        OutOfRange,
        AttackPath
    }
}
