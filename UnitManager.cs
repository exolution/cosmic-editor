using UnityEngine;
using System.Collections.Generic;
public static class UnitManager  {

    
    static Dictionary<int,Unit> unitMap = new Dictionary<int, Unit>();
    public static void add(GameObject gameObject,Unit unit)
    {
        unitMap.Add(gameObject.GetHashCode(), unit);
    }
    public static Unit getUnit(GameObject gameObject)
    {
        int hashCode = gameObject.GetHashCode();
        if (unitMap.ContainsKey(hashCode)){
            return unitMap[hashCode];
        }
        else
        {
            return null;
        }

    }
}
