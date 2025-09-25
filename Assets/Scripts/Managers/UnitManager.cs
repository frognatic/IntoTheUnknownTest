using System.Collections.Generic;
using IntoTheUnknownTest.Libraries;
using IntoTheUnknownTest.Map;
using UnityEngine;

namespace IntoTheUnknownTest.Managers
{
    public class UnitManager : Singleton<UnitManager>
    {
        [SerializeField] private UnitsLibrary _unitsLibrary;
        
        public List<BaseUnitData> Units => _unitsLibrary.BaseUnits;
    }
}
