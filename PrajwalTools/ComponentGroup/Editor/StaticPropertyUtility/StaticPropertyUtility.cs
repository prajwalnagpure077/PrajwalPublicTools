using System;

using UnityEditor;

namespace PrajwalTools
{
    public static class StaticPropertyUtility
    {
        public static void foreachSelected(Action<bool, UnityEngine.Object> foreachObject)
        {
            bool multipleSelected = DragAndDrop.objectReferences.Length > 1;
            foreach (var item in DragAndDrop.objectReferences)
            {
                foreachObject?.Invoke(multipleSelected, item);
            }
        }

        public static void GetNextProperty(UnityEngine.Object item, Func<SerializedProperty, bool> shouldShow, Action<SerializedProperty> foreachProperty)
        {
            SerializedObject obj = new(item);
            var serializeProperties = obj.GetIterator();
            bool first = true;
            while (serializeProperties.Next(first))
            {
                first = false;
                if (shouldShow(serializeProperties))
                {
                    foreachProperty?.Invoke(serializeProperties);
                }
                else
                {
                    continue;
                }
            }
        }

        public static void GetNextVisibleProperty(UnityEngine.Object item, Func<SerializedProperty, bool> shouldShow, Action<SerializedProperty> foreachProperty)
        {
            SerializedObject obj = new(item);
            var serializeProperties = obj.GetIterator();
            bool first = true;
            while (serializeProperties.NextVisible(first))
            {
                first = false;
                if (shouldShow(serializeProperties))
                {
                    foreachProperty?.Invoke(serializeProperties);
                }
                else
                {
                    continue;
                }
            }
        }
    }
}