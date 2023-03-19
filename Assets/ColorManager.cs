using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{

    public List<DamageTypeColor> DamageTypeColors = new List<DamageTypeColor>();
    
    [System.Serializable]
    public class DamageTypeColor{
        public string damageTypeName;
        public Color color;
    }

    public Color GetDamageTypeMaterialByName(string damageTypeName)
    {
        foreach (DamageTypeColor obj in DamageTypeColors)
        {
            if (damageTypeName.ToLower() == obj.damageTypeName.ToLower())
                return obj.color;
        }

        return new Color();

    }

}
