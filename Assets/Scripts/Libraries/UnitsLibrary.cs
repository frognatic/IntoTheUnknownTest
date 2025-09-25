using System.Collections.Generic;
using IntoTheUnknownTest.Map;
using UnityEngine;

namespace IntoTheUnknownTest.Libraries
{
    [CreateAssetMenu(fileName = "UnitsLibrary", menuName = "Data/Library/UnitsLibrary")]
    public class UnitsLibrary : ScriptableObject
    {
        [SerializeField] private List<BaseUnitData> _baseUnits;
        
        public List<BaseUnitData> BaseUnits => _baseUnits;
    }
}
