using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace UnityEditor.SceneViewBookmarks
{
    public class AddViewpointPopupWindow : EditorWindow
    {
        static readonly string[] reservedTopMenuItemNames = new string[5] { "Options", "Filter", "Cameras", "Virtual Cameras", "Bookmark View" };

        public enum BookmarkSaveLocation { Scene, Preferences }

        public Viewpoint.Overrides overrides = Viewpoint.Overrides.Position | Viewpoint.Overrides.Direction | Viewpoint.Overrides.FieldOfView | Viewpoint.Overrides.IsOrtho | Viewpoint.Overrides.Is2D;
        public BookmarkSaveLocation bookmarkSaveLocation;
        public SceneView sceneview;
        [SerializeField] string bookmarkName = "New Bookmark";

        public static void Open(SceneView sceneview)
        {
            AddViewpointPopupWindow window = CreateInstance<AddViewpointPopupWindow>();
            window.position = new Rect(sceneview.position.x + sceneview.position.width * 0.5f - 125, sceneview.position.y + sceneview.position.height * 0.5f - 75, 300, 110);
            window.titleContent = new GUIContent("Bookmark Scene View as");
            window.sceneview = sceneview;
            //window.ShowPopup();
            //window.ShowModal();
            window.ShowModalUtility();
        }

        private void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            root.Add(new PropertyField() { name = "Name", bindingPath = nameof(bookmarkName)});
            root.Add(new PropertyField() { bindingPath = nameof(overrides), name = "Overrides" });
            root.Add(new PropertyField() { bindingPath = nameof(bookmarkSaveLocation), name = "Location" });

            VisualElement buttons = new VisualElement()
            {
                name = "",
                tooltip = "",
            };
            buttons.AddToClassList("unity-editor-toolbar");

            buttons.Add(new Button(() => CreateBookmark()) { name = "Create", text = "Create" });
            buttons.Add(new Button(() => Close()) { name = "Close", text = "Close" });

            root.Add(buttons);
            root.Bind(new SerializedObject(this));
        }

        //void OnGUI()
        //{
        //    EditorGUILayout.LabelField("Bookmark Settings", EditorStyles.wordWrappedLabel);
        //    bookmarkName = EditorGUILayout.TextField(bookmarkName);
        //    overrides = (Viewpoint.Overrides)EditorGUILayout.EnumFlagsField("Overrides", overrides);
        //    bookmarkSaveLocation = (BookmarkSaveLocation)EditorGUILayout.EnumPopup("Bookmark Location", bookmarkSaveLocation);
        //    GUILayout.BeginHorizontal();
        //    if (GUILayout.Button("Create"))
        //        CreateBookmark();
        //    else if (GUILayout.Button("Cancel"))
        //        this.Close();
        //    GUILayout.EndHorizontal();
        //}

        public void CreateBookmark()
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
                    SceneBookmarks.Add(new Viewpoint(bookmarkName, sceneview, overrides));
                    break;
                case BookmarkSaveLocation.Preferences:
                    DefaultBookmarks.Add(new Viewpoint(bookmarkName, sceneview, overrides));
                    break;
            }
            this.Close();
        }
    }
}