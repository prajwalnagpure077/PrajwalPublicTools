using System.Collections.Generic;

using UnityEngine;

namespace PrajwalTools
{
#if UNITY_EDITOR
    public class PropertyGroupEditorObject : MonoBehaviour
    {
        public List<PropertyModule> propertyModules = new List<PropertyModule>();
    }
#endif
}
