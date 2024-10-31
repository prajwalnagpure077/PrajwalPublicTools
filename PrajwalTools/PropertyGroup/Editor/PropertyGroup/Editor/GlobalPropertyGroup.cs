using UnityEditor;

using UnityEditorInternal;

using UnityEngine;

using static PrajwalTools.DropStaticUtility;
using static PrajwalTools.PropertyGroupStaticUtility;
using static PrajwalTools.StaticPropertyUtility;
using static PrajwalTools.ThemeStaticUtility;



namespace PrajwalTools
{
    public class GlobalPropertyGroup : EditorWindow
    {
        [SerializeField] PropertyModuleSO propertyModuleSO;
        Vector2 scrollPos;
        bool dropable = true;
        ReorderableList lst;
        SerializedObject serializedObject;
        bool showClear = true;
        TextInputPopupWindow textInputPopupWindow;

        [MenuItem("Tools/Prajwal Tools/Global Property Group")]
        public static void ShowWindow()
        {
            GlobalPropertyGroup wnd = GetWindow<GlobalPropertyGroup>();
            wnd.titleContent = new GUIContent("Global Property Group");
        }
        private void Update()
        {
            Repaint();
        }

        private void OnGUI()
        {
            GUI.backgroundColor = backgroundColor;
            DrawPropertyModuleGUI(ref dropable,
                ref scrollPos,
                ref showClear,
                ref textInputPopupWindow,
                ref propertyModuleSO.propertyModules,
                () =>
            {
                return new(propertyModuleSO);
            },
                ref serializedObject,
                ref lst,
                (_) =>
                {
                    if (!string.IsNullOrEmpty(_))
                    {
                        Undo.RecordObject(propertyModuleSO, "add element");
                        propertyModuleSO.propertyModules.Add(new() { Header = _ });
                        EditorUtility.SetDirty(propertyModuleSO);
                        AssetDatabase.SaveAssetIfDirty(propertyModuleSO);
                        AssetDatabase.Refresh();
                    }
                },
                DropCheck,
                (rect, index, isActive, isFocused) =>
                {
                    if (propertyModuleSO.propertyModules.Count > index)
                    {
                        var item = propertyModuleSO.propertyModules[index];
                        drawElementCallback(ref showClear, ref propertyModuleSO.propertyModules, item, rect);
                    }
                }, (index) =>
                {
                    var item = propertyModuleSO.propertyModules[index];
                    return elementHeightCallback(item) - (showClear ? 0 : 20);
                }, () =>
                {
                    Undo.RecordObject(propertyModuleSO, "clear all");
                    propertyModuleSO.propertyModules.Clear();
                    EditorUtility.SetDirty(propertyModuleSO);
                    AssetDatabase.SaveAssetIfDirty(propertyModuleSO);
                    AssetDatabase.Refresh();
                });
        }

        void DropCheck()
        {
            DragAndDropCheck(this, false, (menu) =>
            {
                foreachSelected((multipleSelect, currentObject) =>
                {
                    Object[] _toshow = null;
                    if (currentObject.GetType() == typeof(GameObject))
                    {
                        Debug.Log("GameObject");
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
                                    Undo.RecordObject(propertyModuleSO, "add element");
                                    propertyModuleSO.propertyModules.Add(new() { propertyPath = _propertyPath, _object = item });
                                    EditorUtility.SetDirty(propertyModuleSO);
                                    AssetDatabase.SaveAssetIfDirty(propertyModuleSO);
                                    AssetDatabase.Refresh();
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
                        Debug.Log("GameObject");
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
                                    propertyModuleSO.propertyModules.Add(new() { propertyPath = _propertyPath, _object = item });
                                    EditorUtility.SetDirty(propertyModuleSO);
                                    AssetDatabase.SaveAssetIfDirty(propertyModuleSO);
                                    AssetDatabase.Refresh();
                                });
                            });
                    }
                });
            });
        }
    }
}
