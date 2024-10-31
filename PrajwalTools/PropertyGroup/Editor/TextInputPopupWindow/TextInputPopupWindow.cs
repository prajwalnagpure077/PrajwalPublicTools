using System;
using UnityEditor;
using UnityEngine;

namespace PrajwalTools
{
    public class TextInputPopupWindow : PopupWindowContent
    {
        string text;
        public Action<string> OnButtonClick;

        public override Vector2 GetWindowSize()
        {
            return new Vector2(300, 40);
        }

        public override void OnGUI(Rect rect)
        {
            if (Event.current != null && Event.current.isKey)
            {
                if (Event.current.keyCode == KeyCode.Return)
                    CloseWindow(true);
                else if (Event.current.keyCode == KeyCode.Escape)
                    CloseWindow(false);
            }

            GUI.backgroundColor = Color.Lerp(Color.grey, Color.blue, 0.4f);

            GUI.SetNextControlName("MyTextField");
            text = GUILayout.TextField(text);

            if (GUILayout.Button("Add"))
            {
                CloseWindow(true);
            }
            GUI.FocusControl("MyTextField");
        }

        void CloseWindow(bool invokeButton)
        {
            editorWindow.Close();
            if (invokeButton)
                OnButtonClick?.Invoke(text);
        }
    }
}