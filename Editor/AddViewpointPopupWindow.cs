using System;
using UnityEngine;

namespace UnityEditor.SceneViewBookmarks
{
    public class AddViewpointPopupWindow : EditorWindow
    {
        static readonly string[] reservedTopMenuItemNames = new string[5] { "Options", "Filter", "Cameras", "Virtual Cameras", "Bookmark View" };

        public enum BookmarkSaveLocation { Scene, Preferences }

        public BookmarkSaveLocation bookmarkSaveLocation;
        public SceneView sceneview;
        public string bookmarkName = "New Bookmark";

        public static void Open(SceneView sceneview)
        {
            AddViewpointPopupWindow window = CreateInstance<AddViewpointPopupWindow>();
            window.position = new Rect(sceneview.position.x + sceneview.position.width * 0.5f - 125, sceneview.position.y + sceneview.position.height * 0.5f - 75, 250, 150);
            window.titleContent = new GUIContent("Bookmark Scene View as");
            window.sceneview = sceneview;
            //window.ShowPopup();
            //window.ShowModal();
            window.ShowModalUtility();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Bookmark Settings", EditorStyles.wordWrappedLabel);
            bookmarkName = EditorGUILayout.TextField(bookmarkName);
            bookmarkSaveLocation = (BookmarkSaveLocation)EditorGUILayout.EnumPopup("Bookmark Location", bookmarkSaveLocation);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create"))
            {
                var delimiter = '/';
                var menuPath = bookmarkName.Split(delimiter);
                if (menuPath.Length > 1)
                {
                    if (Array.Exists<string>(reservedTopMenuItemNames, e => e == menuPath[0]))
                    {
                        bookmarkName = "**" + bookmarkName;
                    }
                }
                switch (bookmarkSaveLocation)
                {
                    case BookmarkSaveLocation.Scene:
                        Bookmarks.Instance.sceneViewpoints.Add(new Viewpoint(bookmarkName, sceneview));
                        break;
                    case BookmarkSaveLocation.Preferences:
                        Bookmarks.Instance.builtinViewpoints.Add(new Viewpoint(bookmarkName, sceneview));
                        break;
                }
                this.Close();
            }
            else if (GUILayout.Button("Cancel"))
            {
                this.Close();
            }
            GUILayout.EndHorizontal();
        }
    }
}