using UnityEditor;

using UnityEditorInternal;

using UnityEngine;

using static PrajwalTools.DropStaticUtility;
using static PrajwalTools.PropertyGroupStaticUtility;
using static PrajwalTools.StaticPropertyUtility;
using static PrajwalTools.ThemeStaticUtility;


namespace PrajwalTools
{
    public class ScenePropertyGroup : EditorWindow
    {
        PropertyGroupEditorObject propertyGroupEditorObject;
        Vector2 scrollPos;
        bool dropable = true;
        ReorderableList lst;
        SerializedObject serializedObject;
        bool showClear = true;
        TextInputPopupWindow textInputPopupWindow;

        [MenuItem("Tools/Prajwal Tools/Scene Property Group")]
        public static void ShowWindow()
        {
            ScenePropertyGroup wnd = GetWindow<ScenePropertyGroup>();
            wnd.titleContent = new GUIContent("Scene Property Group");
        }

        private void Update()
        {
            Repaint();
        }

        private void OnGUI()
        {
            GUI.backgroundColor = backgroundColor;
            if (propertyGroupEditorObject == null)
            {
                propertyGroupEditorObject = FindObjectOfType<PropertyGroupEditorObject>();
            }
            if (propertyGroupEditorObject == null)
            {
                EditorGUILayout.LabelField("You don't have any PropertyGroupEditorObject component in this scene.");
                EditorGUILayout.LabelField("Click on Setup if you want to add EditorOnly GameObject for Property Group to work.");
                if (EditorGUILayout.LinkButton("Setup"))
                {
                    GameObject obj = new("PropertyGroupEditorObject");
                    obj.tag = "EditorOnly";
                    Undo.RegisterCreatedObjectUndo(obj, "Created PropertyGroupEditorObject");
                    propertyGroupEditorObject = ObjectFactory.AddComponent<PropertyGroupEditorObject>(obj);
                    Undo.RegisterCreatedObjectUndo(propertyGroupEditorObject, "Created PropertyGroupEditorObject");
                }
                return;
            }

            DrawPropertyModuleGUI(ref dropable,
            ref scrollPos,
            ref showClear,
            ref textInputPopupWindow,
            ref propertyGroupEditorObject.propertyModules,
            () =>
            {
                propertyGroupEditorObject = FindObjectOfType<PropertyGroupEditorObject>();
                return new(propertyGroupEditorObject);
            },
            ref serializedObject,
            ref lst,
            (_) =>
            {
                if (!string.IsNullOrEmpty(_))
                {
                    Undo.RecordObject(propertyGroupEditorObject, "add element");
                    propertyGroupEditorObject.propertyModules.Add(new() { Header = _ });
                    EditorUtility.SetDirty(propertyGroupEditorObject);
                }
            },
            DropCheck,
            (rect, index, isActive, isFocused) =>
            {
                if (propertyGroupEditorObject.propertyModules.Count > index)
                {
                    var item = propertyGroupEditorObject.propertyModules[index];
                    drawElementCallback(ref showClear, ref propertyGroupEditorObject.propertyModules, item, rect, propertyGroupEditorObject);
                }
            }, (index) =>
            {
                var item = propertyGroupEditorObject.propertyModules[index];
                return elementHeightCallback(item) - (showClear ? 0 : 20);
            },
            () =>
            {
                Undo.RecordObject(propertyGroupEditorObject, "Clear All");
                propertyGroupEditorObject.propertyModules.Clear();
                EditorUtility.SetDirty(propertyGroupEditorObject);
            });
        }
        private void OnEnable()
        {
            lst = null;
            serializedObject = null;
        }
        void DropCheck()
        {
            DragAndDropCheck(this, true, (menu) =>
            {
                foreachSelected((multipleSelect, currentObject) =>
                {
                    Object[] _toshow = null;
                    if (currentObject.GetType() == typeof(GameObject))
                    {
                        _toshow = ((GameObject)currentObject).GetComponents<Component>();
                    }
                    else
                    {
                        _toshow = new[] { currentObject };
                    }
                    foreach (var item in _toshow)
                    {
                        GetNextVisibleProperty(item,
                            (_) =>
                            {
                                return _.editable;
                            }, (_) =>
                            {
                                var _propertyPath = _.propertyPath;
                                menu.AddItem(new GUIContent((multipleSelect ? currentObject.name + "/" : "") + item.GetType() + "/" + _propertyPath.Replace('.', '/')), false, () =>
                                {
                                    Undo.RecordObject(propertyGroupEditorObject, "add element");
                                    propertyGroupEditorObject.propertyModules.Add(new() { propertyPath = _propertyPath, _object = item });
                                    EditorUtility.SetDirty(propertyGroupEditorObject);
                                });
                            });
                    }
                    if (multipleSelect)
                    {
                        menu.AddSeparator("");
                    }
                });

                foreachSelected((multipleSelect, currentObject) =>
                {
                    Object[] _toshow = null;
                    if (currentObject.GetType() == typeof(GameObject))
                    {
                        _toshow = ((GameObject)currentObject).GetComponents<Component>();
                    }
                    else
                    {
                        _toshow = new[] { currentObject };
                    }
                    foreach (var item in _toshow)
                    {
                        GetNextProperty(item,
                            (_) =>
                            {
                                return _.editable;
                            }, (_) =>
                            {
                                //var _propertyPath = _.propertyPath;
                                var _propertyPath = _.propertyPath;
                                menu.AddItem(new GUIContent("Advanced/" + (multipleSelect ? currentObject.name + "/" : "") + _propertyPath.Replace('.', '/')), false, () =>
                                {
                                    Undo.RecordObject(propertyGroupEditorObject, "add element");
                                    propertyGroupEditorObject.propertyModules.Add(new() { propertyPath = _propertyPath, _object = item });
                                    EditorUtility.SetDirty(propertyGroupEditorObject);
                                });
                            });
                    }
                });
            });
        }
    }
}
