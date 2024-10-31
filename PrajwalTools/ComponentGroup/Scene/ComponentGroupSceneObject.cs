using System.Collections.Generic;

using PrajwalTools;

using UnityEngine;

#if UNITY_EDITOR
public class ComponentGroupSceneObject : MonoBehaviour
{
    public List<SceneObjectModules> currentShowenObjects = new();
    private void Reset()
    {
        this.tag = "EditorOnly";
        this.name = "Component Group Scene Object";
    }
}

#endif