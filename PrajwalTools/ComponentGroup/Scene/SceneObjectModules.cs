using UnityEditor;

using UnityEngine;

namespace PrajwalTools
{
#if UNITY_EDITOR
    [System.Serializable]
    public class SceneObjectModules
    {
        public Object item1
        {
            get
            {
                if (_item1 == null)
                {
                    _item1 = EditorUtility.InstanceIDToObject(instanceId);
                    editor = null;
                }
                return _item1;
            }
        }
        public Object _item1;
        public bool item2;
        public System.DateTime createdTime;
        int instanceId;
        Editor editor;
        public Editor _editor
        {
            get
            {
                if (editor == null || editor.target == null)
                    editor = Editor.CreateEditor(item1);
                return editor;
            }
        }

        public SceneObjectModules(Object item1, bool item2)
        {
            _item1 = item1;
            instanceId = item1.GetInstanceID();
            this.item2 = item2;
            editor = Editor.CreateEditor(item1);
            createdTime = System.DateTime.UtcNow;
        }
    }
#endif
}