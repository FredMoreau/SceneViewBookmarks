#if UNITY_2021_2_OR_NEWER
using UnityEngine;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using UnityEditor.Toolbars;

namespace UnityEditor.SceneViewBookmarks
{
    [EditorToolbarElement(id, typeof(SceneView))]
    class BookmarksEditorToolbarEditorBookmarks : VisualElement, IAccessContainerWindow //ToolbarOverlayElement
    {
        public const string id = "Bookmarks/Editor";

        public EditorWindow containerWindow { get; set; }

        BookmarksEditorToolbarEditorBookmarks()
        {
            tooltip = "";
            UpdateContent();
            DefaultBookmarks.onEditorBookmarksChange += () =>
            {
                UpdateContent();
            };
        }

        void UpdateContent()
        {
            Clear();
            Add(BookmarksOverlayHelper.BookmarksDropdown("User",
                EditorGUIUtility.IconContent("d_FrameCapture@2x").image,
                "",
                OnClick));
        }

        void OnClick(MouseDownEvent evt)
        {
            BookmarksOverlayHelper.UserBookmarksMenu(containerWindow as SceneView).DropDown(worldBound);
        }
    }

    [EditorToolbarElement(id, typeof(SceneView))]
    class BookmarksEditorToolbarSceneBookmarks : VisualElement, IAccessContainerWindow
    {
        public const string id = "Bookmarks/Scene";

        public EditorWindow containerWindow { get; set; }

        BookmarksEditorToolbarSceneBookmarks()
        {
            tooltip = "";
            UpdateContent();
            SceneBookmarks.onSceneBookmarksChange += () =>
            {
                UpdateContent();
            };
        }

        void UpdateContent()
        {
            Clear();
            Add(BookmarksOverlayHelper.BookmarksDropdown("Scene",
                EditorGUIUtility.IconContent("d_FrameCapture@2x").image,
                "",
                OnClick));
        }

        void OnClick(MouseDownEvent evt)
        {
            BookmarksOverlayHelper.SceneBookmarksMenu(containerWindow as SceneView).DropDown(worldBound);
        }
    }

    [EditorToolbarElement(id, typeof(SceneView))]
    class BookmarksEditorToolbarAddBookmarkButton : EditorToolbarButton
    {
        public const string id = "Bookmarks/AddButton";

        public BookmarksEditorToolbarAddBookmarkButton()
        {
            icon = (Texture2D)EditorGUIUtility.IconContent("d_Toolbar Plus@2x").image;
            tooltip = "Create new Bookmark.";
            clicked += () => AddViewpointPopupWindow.Open(SceneView.lastActiveSceneView);
        }
    }

    [EditorToolbarElement(id, typeof(SceneView))]
    class BookmarksEditorToolbarOptionsDropdown : VisualElement, IAccessContainerWindow
    {
        public const string id = "Bookmarks/Options";

        public EditorWindow containerWindow { get; set; }

        BookmarksEditorToolbarOptionsDropdown()
        {
            tooltip = "";
            Add(BookmarksOverlayHelper.BookmarksSettingsDropdown(
                EditorGUIUtility.IconContent("d_SettingsIcon@2x").image,
                "",
                OnClick));
        }

        void OnClick(MouseDownEvent evt)
        {
            BookmarksOverlayHelper.ExtraToolsMenu(containerWindow as SceneView).DropDown(worldBound);
        }
    }

    [Overlay(typeof(SceneView), "SceneView Bookmarks")]
    [Icon("Assets/PlacementToolsIcon.png")]
    public class BookmarksEditorToolbar : ToolbarOverlay
    {
        Foldout editorFoldout, sceneFoldout, miscFoldout;
        bool editorFoldoutOn = true, sceneFoldoutOn = true, miscFoldoutOn = false;
        BookmarksEditorToolbar() : base(
            BookmarksEditorToolbarEditorBookmarks.id,
            BookmarksEditorToolbarSceneBookmarks.id,
            BookmarksEditorToolbarAddBookmarkButton.id,
            BookmarksEditorToolbarOptionsDropdown.id)
        {
            //CreatePanelContent();
            DefaultBookmarks.onEditorBookmarksChange += () =>
            {
                editorFoldout?.Clear();
                editorFoldout?.Add(BookmarksOverlayHelper.BuiltinBookmarksVE());
            };
            SceneBookmarks.onSceneBookmarksChange += () =>
            {
                sceneFoldout?.Clear();
                sceneFoldout?.Add(BookmarksOverlayHelper.SceneBookmarksVE());
            };

            collapsedChanged += (x) =>
            {

            };

            displayedChanged += (x) =>
            {

            };

            floatingChanged += (x) =>
            {

            };

            layoutChanged += (x) =>
            {

            };
        }

        public override VisualElement CreatePanelContent()
        {
            if (!collapsed)
                return base.CreatePanelContent();
            else // if collapsed bring a full blown window
            {
                VisualElement root = new VisualElement();

                editorFoldout = new Foldout() { value = editorFoldoutOn, name = "User", text = "User" };
                editorFoldout.Add(BookmarksOverlayHelper.BuiltinBookmarksVE());
                editorFoldout.RegisterValueChangedCallback((x) => editorFoldoutOn = x.newValue);
                root.Add(editorFoldout);

                sceneFoldout = new Foldout() { value = sceneFoldoutOn, name = "Scene", text = "Scene" };
                sceneFoldout.Add(BookmarksOverlayHelper.SceneBookmarksVE());
                sceneFoldout.RegisterValueChangedCallback((x) => sceneFoldoutOn = x.newValue);
                root.Add(sceneFoldout);

                root.Add(BookmarksOverlayHelper.AddCreateBookmarkButton("Add"));

                miscFoldout = new Foldout() { value = miscFoldoutOn, name = "Misc.", text = "Misc." };
                miscFoldout.Add(BookmarksOverlayHelper.MiscVE());
                sceneFoldout.RegisterValueChangedCallback((x) => miscFoldoutOn = x.newValue);
                root.Add(miscFoldout);

                return root;
            }
        }
    }
}
#endif