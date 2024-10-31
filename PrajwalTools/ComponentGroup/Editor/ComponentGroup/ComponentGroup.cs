using System;
using System.Collections.Generic;
using System.Linq;

using Unity.VisualScripting;

using UnityEditor;

using UnityEngine;

using static PrajwalTools.DropStaticUtility;
using static PrajwalTools.StaticPropertyUtility;
using static PrajwalTools.ThemeStaticUtility;

using Object = UnityEngine.Object;
namespace PrajwalTools
{
    public class ComponentGroup : EditorWindow
    {
        [SerializeField] ComponentGroupSceneObjectSO componentGroupSceneObjectSO;
        bool autoSelect = true;
        bool unFold = true;
        bool showClear = true;
        Vector2 scrollPos;

        ComponentGroupType currentComponentGroupType;
        ComponentGroupSceneObject componentGroupSceneObject
        {
            get
            {
                if (_componentGroupSceneObject == null)
                {
                    _componentGroupSceneObject = FindObjectOfType<ComponentGroupSceneObject>();
                    if (_componentGroupSceneObject == null)
                    {
                        GameObject obj = new();
                        _componentGroupSceneObject = obj.AddComponent<ComponentGroupSceneObject>();
                        Undo.RegisterCreatedObjectUndo(obj, "created object for editor");
                    }
                }
                return _componentGroupSceneObject;
            }
        }
        static string GetPrefKey(string _)
        {
            return nameof(ComponentGroup) + "_" + _;
        }

        public bool AutoSelect
        {
            get => autoSelect; set
            {
                if (value != autoSelect)
                    EditorPrefs.SetString(GetPrefKey(nameof(autoSelect)), value.ToString());
                autoSelect = value;
            }
        }

        public bool UnFold
        {
            get => unFold; set
            {
                if (value != unFold)
                    EditorPrefs.SetString(GetPrefKey(nameof(unFold)), value.ToString());
                unFold = value;
            }
        }
        public bool ShowClear
        {
            get => showClear; set
            {
                if (value != showClear)
                    EditorPrefs.SetString(GetPrefKey(nameof(showClear)), value.ToString());
                showClear = value;
            }
        }
        private void OnFocus()
        {

            bool.TryParse(EditorPrefs.GetString(GetPrefKey(nameof(autoSelect)), "true"), out autoSelect);
            bool.TryParse(EditorPrefs.GetString(GetPrefKey(nameof(unFold)), "true"), out unFold);
            bool.TryParse(EditorPrefs.GetString(GetPrefKey(nameof(showClear)), "true"), out showClear);
        }

        ComponentGroupSceneObject _componentGroupSceneObject;

        [MenuItem("Tools/Prajwal Tools/Component Group")]
        public static void ShowWindow()
        {
            ComponentGroup wnd = GetWindow<ComponentGroup>();
            wnd.titleContent = new GUIContent("Component Group");
        }
        private void Update()
        {
            Repaint();
        }

        private void OnGUI()
        {
            GUI.backgroundColor = backgroundColor;
            EditorGUILayout.BeginHorizontal();

            currentComponentGroupType = (ComponentGroupType)EditorGUILayout.EnumPopup(currentComponentGroupType);
            GUI.backgroundColor = Color.red;
            GUI.contentColor = Color.white;
            if (GUILayout.Button("Clear All"))
            {
                componentGroupSceneObject.currentShowenObjects.Clear();
                SetSceneObjectDirty();
                componentGroupSceneObjectSO.currentShowenObjects.Clear();
            }
            GUI.contentColor = Color.white;
            GUI.backgroundColor = backgroundColor;

            AutoSelect = EditorGUILayout.ToggleLeft("auto Select", AutoSelect, GUILayout.MinWidth(10), GUILayout.MaxWidth(1000));
            UnFold = EditorGUILayout.ToggleLeft("unfold ", UnFold, GUILayout.MinWidth(10), GUILayout.MaxWidth(1000));
            ShowClear = EditorGUILayout.ToggleLeft("show Clear", ShowClear, GUILayout.MinWidth(10), GUILayout.MaxWidth(1000));

            EditorGUILayout.EndHorizontal();
            DragAndDropCheck(this, false, (menu) =>
            {
                if (AutoSelect)
                {
                    var currentObj = DragAndDrop.objectReferences[0];
                    if (currentObj.GetType() == typeof(GameObject))
                    {
                        if (DragAndDrop.objectReferences[0].IsSceneBound())
                        {
                            if (componentGroupSceneObject != null)
                            {
                                componentGroupSceneObject.currentShowenObjects = ((GameObject)currentObj).GetComponents<Component>().Select(x => new SceneObjectModules(x, UnFold)).ToList();
                                SetSceneObjectDirty();
                            }
                        }
                        else
                            componentGroupSceneObjectSO.currentShowenObjects = ((GameObject)currentObj).GetComponents<Component>().Select(x => new SceneObjectModules(x, UnFold)).ToList();
                    }
                    else
                    {
                        if (DragAndDrop.objectReferences[0].IsSceneBound())
                        {
                            if (componentGroupSceneObject != null)
                            {
                                componentGroupSceneObject.currentShowenObjects = new List<SceneObjectModules> { new SceneObjectModules(currentObj, UnFold) };
                                SetSceneObjectDirty();
                            }
                        }
                        else
                            componentGroupSceneObjectSO.currentShowenObjects = new List<SceneObjectModules> { new SceneObjectModules(currentObj, UnFold) };
                    }
                }
                else
                {
                    foreachSelected((multipleSelect, currentObject) =>
                    {
                        (string, Object)[] Objs = null;
                        if (currentObject.GetType() == typeof(GameObject))
                        {
                            int i = 0;
                            Objs = ((GameObject)currentObject).GetComponents<Component>().Select(x =>
                            {
                                i++;
                                return (i + x.GetType().ToString(), (Object)x);
                            }).ToArray();
                        }
                        else
                        {
                            Objs = new[] { (currentObject.name, currentObject) };
                        }
                        foreach (var item in Objs)
                        {
                            menu.AddItem(new(item.Item1), false, () =>
                            {
                                if (DragAndDrop.objectReferences[0].IsSceneBound())
                                {
                                    if (componentGroupSceneObject)
                                    {
                                        componentGroupSceneObject.currentShowenObjects.Add(new SceneObjectModules(item.Item2, UnFold));
                                        SetSceneObjectDirty();
                                    }
                                }
                                else
                                    componentGroupSceneObjectSO.currentShowenObjects.Add(new SceneObjectModules(item.Item2, UnFold));
                            });
                        }
                    });
                }
            }, false);

            DrawGUI();
        }
        void DrawGUI()
        {
            if ((currentComponentGroupType == ComponentGroupType.Scene ? componentGroupSceneObject.currentShowenObjects : componentGroupSceneObjectSO.currentShowenObjects) != null)
            {
                GUI.enabled = true;
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                foreach (var item in (currentComponentGroupType == ComponentGroupType.Scene ? componentGroupSceneObject.currentShowenObjects : componentGroupSceneObjectSO.currentShowenObjects))
                {
                    if (item.item1 == null)
                    {
                        continue;
                    }
                    if (ShowClear)
                    {
                        GUI.backgroundColor = Color.red;
                        GUI.contentColor = Color.white;
                        if (GUILayout.Button(new GUIContent("clear")))
                        {
                            (currentComponentGroupType == ComponentGroupType.Scene ? componentGroupSceneObject.currentShowenObjects : componentGroupSceneObjectSO.currentShowenObjects).Remove(item);
                            SetSceneObjectDirty();
                            EditorGUILayout.EndScrollView();
                            EditorGUILayout.EndVertical();
                            return;
                        }
                        GUI.contentColor = Color.white;
                        GUI.backgroundColor = backgroundColor;
                    }


                    EditorGUI.indentLevel = 0;
                    Rect _headerRect = EditorGUILayout.BeginVertical();
                    item._editor.DrawHeader();
                    EditorGUILayout.EndHorizontal();

                    if (item.item2)
                    {
                        EditorGUI.indentLevel = 1;
                        item._editor.OnInspectorGUI();
                    }
                    if (GUI.Button(_headerRect, "", GUIStyle.none))
                    {
                        item.item2 = !item.item2;
                    }

                    EditorGUILayout.Separator();
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
            }
        }

        void SetSceneObjectDirty()
        {
            if (componentGroupSceneObject != null)
                EditorUtility.SetDirty(componentGroupSceneObject);
        }

        enum ComponentGroupType
        {
            Scene,
            Project,
            //All
        }
    }
}