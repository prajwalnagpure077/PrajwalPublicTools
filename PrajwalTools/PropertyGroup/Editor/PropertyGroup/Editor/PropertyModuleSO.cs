using System.Collections.Generic;

using UnityEngine;

namespace PrajwalTools
{
    [CreateAssetMenu(fileName = "PropertyModuleSO ", menuName = "PropertyModuleSO ")]
    public class PropertyModuleSO : ScriptableObject
    {
        public List<PropertyModule> propertyModules = new List<PropertyModule>();
    }
}