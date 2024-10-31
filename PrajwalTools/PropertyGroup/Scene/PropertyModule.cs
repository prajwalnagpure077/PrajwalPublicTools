using UnityEngine;

namespace PrajwalTools
{
#if UNITY_EDITOR
    [System.Serializable]
    public class PropertyModule
    {
        public Object _object;
        public string propertyPath;
        public string Header;
    }  
#endif
}