using System;
using System.Collections.Generic;

using UnityEditor;

using UnityEditorInternal;

using UnityEngine;

using static PrajwalTools.ThemeStaticUtility;

namespace PrajwalTools
{
    public static class PropertyGroupStaticUtility
    {
        internal static void DrawPropertyModuleGUI(ref bool dropable, ref Vector2 scrollPos, ref bool showClear, ref TextInputPopupWindow textInputPopupWindow, ref List<PropertyModule> propertyModules, Func<SerializedObject> propertyGroupEditorObject, ref SerializedObject serializedObject, ref ReorderableList lst, Action<string> OnAddNote, Action DropCheck, ReorderableList.ElementCallbackDelegate DrawElementCallback, ReorderableList.ElementHeightCallbackDelegate ElementHeightCallback, Action OnAllClear)
        {
            EditorGUILayout.BeginHorizontal();
            dropable = EditorGUILayout.ToggleLeft("dropable", dropable, GUILayout.MaxWidth(70));

            if (GUILayout.Button("Add Note"))
            {
                var _rect = GUILayoutUtility.GetLastRect();
                if (textInputPopupWindow == null)
                    textInputPopupWindow = new();
                textInputPopupWindow.OnButtonClick = OnAddNote;
                PopupWindow.Show(_rect, textInputPopupWindow);
            }

            GUI.backgroundColor = Color.red;
            GUI.contentColor = Color.white;
            if (GUILayout.Button("Clear All"))
            {
                OnAllClear?.Invoke();
            }
            GUI.contentColor = Color.white;
            GUI.backgroundColor = backgroundColor;

            showClear = EditorGUILayout.ToggleLeft("show Clear", showClear, GUILayout.MinWidth(10), GUILayout.MaxWidth(1000));

            EditorGUILayout.EndHorizontal();

            if (dropable)
                DropCheck?.Invoke();

            EditorGUILayout.BeginVertical();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            if (lst == null || serializedObject == null || serializedObject.targetObject == null)
            {
                serializedObject = propertyGroupEditorObject();
                lst = new(serializedObject, serializedObject.FindProperty("propertyModules"), true, false, false, true);

                lst.drawElementCallback += DrawElementCallback;
                lst.elementHeightCallback += ElementHeightCallback;
            }

            serializedObject.Update();
            lst.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
        }

        internal static float elementHeightCallback(PropertyModule item)
        {
            if (item == null)
                return 20;
            if (item._object == null || string.IsNullOrEmpty(item.propertyPath))
                return 40;
            var serializedObject = new SerializedObject(item._object);
            if (serializedObject != null)
            {
                var serializedProperty = serializedObject.FindProperty(item.propertyPath);
                if (serializedProperty != null)
                    return EditorGUI.GetPropertyHeight(serializedProperty, new GUIContent(""), true) + 20;
                else
                    return 10;
            }
            else if (!string.IsNullOrEmpty(item.Header))
                return 40f;
            else return 0;
        }

        internal static void drawElementCallback(ref bool showClear, ref List<PropertyModule> lst, PropertyModule item, Rect rect, UnityEngine.Object _object = null)
        {
            if (showClear)
            {
                float height = rect.height - 20;
                rect.height = 20;
                rect.position = new(rect.position.x, rect.position.y - rect.height + 20);

                GUI.backgroundColor = Color.red;
                GUI.contentColor = Color.white;
                if (GUI.Button(rect, new GUIContent("clear")))
                {
                    if (_object != null)
                    {
                        Undo.RecordObject(_object, "remove element");
                    }

                    lst.Remove(item);

                    if (_object != null)
                    {
                        EditorUtility.SetDirty(_object);
                    }
                }
                GUI.contentColor = Color.white;
                GUI.backgroundColor = backgroundColor;

                rect.position = new(rect.position.x, rect.position.y + 20);
                rect.height = height;
            }

            if (item._object != null)
            {
                var serializedObject = new SerializedObject(item._object);
                if (serializedObject != null)
                {
                    serializedObject.Update();
                    var serializedProperty = serializedObject.FindProperty(item.propertyPath);
                    if (serializedProperty != null)
                    {
                        EditorGUI.PropertyField(rect, serializedProperty, new GUIContent(""), true);
                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }
            else if (!string.IsNullOrEmpty(item.Header))
            {
                EditorGUI.LabelField(rect, new GUIContent(item.Header));
            }
            else
            {
                //GUI.contentColor = Color.red;
                EditorGUI.HelpBox(rect, "No Reference Found.", MessageType.Warning);
                //EditorGUI.LabelField(rect, "No Reference Found.");
                //GUI.contentColor = Color.white;
            }
        }
    }
}
