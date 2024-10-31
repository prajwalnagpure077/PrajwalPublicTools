using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace PrajwalTools
{
    public static class DropStaticUtility
    {
        public static void DragAndDropCheck(EditorWindow editorWindow, bool IsSceneBound, Action<GenericMenu> AddMenuItems, bool checkSceneBound = true)
        {
            if (EditorWindow.mouseOverWindow == editorWindow)
            {
                if (DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length > 0 && (!checkSceneBound || DragAndDrop.objectReferences[0].IsSceneBound() == IsSceneBound))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                    if (Event.current.type == EventType.DragPerform)
                    {
                        var menu = new GenericMenu();
                        AddMenuItems?.Invoke(menu);
                        menu.ShowAsContext();
                    }
                }
                else
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
            }
        }
    }
}