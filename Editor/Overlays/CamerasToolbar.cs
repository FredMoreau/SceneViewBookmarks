#if UNITY_2021_2_OR_NEWER
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Toolbars;

namespace UnityEditor.SceneViewBookmarks
{
    [EditorToolbarElement(id, typeof(SceneView))]
    class BookmarksEditorToolbarCameras : SceneViewToolbarElement
    {
        public const string id = "Cameras/Cameras";

        BookmarksEditorToolbarCameras()
        {
            tooltip = "";
        }

        protected override void OnInitialized()
        {
            UpdateContent();
            // TODO : find event to update the list of cameras
        }

        void UpdateContent()
        {
            Clear();
            Add(BookmarksOverlayHelper.BookmarksDropdown("Cameras",
                EditorGUIUtility.IconContent("SceneViewCamera").image,
                "",
                BookmarksOverlayHelper.SceneCamerasMenu(containerWindow as SceneView)));
        }
    }

#if CINEMACHINE
    [EditorToolbarElement(id, typeof(SceneView))]
    [Icon("Packages/com.unity.cinemachine/Gizmos/cm_logo.png")]
    class BookmarksEditorToolbarVirtualCameras : SceneViewToolbarElement
    {
        public const string id = "Cameras/VirtualCameras";

        BookmarksEditorToolbarVirtualCameras()
        {
            tooltip = "";
        }

        protected override void OnInitialized()
        {
            UpdateContent();
            // TODO : find event to update the list of cameras
        }

        void UpdateContent()
        {
            Clear();
            Add(BookmarksOverlayHelper.BookmarksDropdown("Cinemachine",
                AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.cinemachine/Gizmos/cm_logo.png"),
                "",
                BookmarksOverlayHelper.SceneVirtualCamerasMenu(containerWindow as SceneView)));
        }
    }
#endif

    [EditorToolbarElement(id, typeof(SceneView))]
    class BookmarksEditorToolbarCamerasDropdown : EditorToolbarDropdownToggle, IAccessContainerWindow
    {
        public const string id = "Cameras/Options";

        public EditorWindow containerWindow { get; set; }

        BookmarksEditorToolbarCamerasDropdown()
        {
            tooltip = "";
            icon = (Texture2D)EditorGUIUtility.IconContent("d_SettingsIcon@2x").image;
            dropdownClicked += ShowMenu;
        }

        void ShowMenu()
        {
            var menu = BookmarksOverlayHelper.CameraToolsMenu(SceneView.lastActiveSceneView);
            menu.DropDown(worldBound);
        }
    }

    [Overlay(typeof(SceneView), "Cameras")]
    [Icon("Packages/com.unity.cinemachine/Gizmos/cm_logo.png")]
    public class CamerasToolbar : ToolbarOverlay
    {
        Foldout camerasFoldout, cmCamerasFoldout, optionsFoldout;
        bool camerasFoldoutOn = true, cmCamerasFoldoutOn = true, optionsFoldoutOn = false;

        CamerasToolbar() : base(
            BookmarksEditorToolbarCameras.id,
#if CINEMACHINE
            BookmarksEditorToolbarVirtualCameras.id,
#endif
            BookmarksEditorToolbarCamerasDropdown.id)
        { }
        public override VisualElement CreatePanelContent()
        {
            if (!collapsed)
                return base.CreatePanelContent();
            else // if collapsed bring a full blown window
            {
                VisualElement root = new VisualElement();

                camerasFoldout = new Foldout() { value = camerasFoldoutOn, name = "Cameras", text = "Cameras" };
                camerasFoldout.Add(BookmarksOverlayHelper.SceneCamerasVE());
                camerasFoldout.RegisterValueChangedCallback((x) => camerasFoldoutOn = x.newValue);
                root.Add(camerasFoldout);

                cmCamerasFoldout = new Foldout() { value = cmCamerasFoldoutOn, name = "Cinemachine", text = "Cinemachine" };
                cmCamerasFoldout.Add(BookmarksOverlayHelper.SceneCMVCamerasVE());
                cmCamerasFoldout.RegisterValueChangedCallback((x) => cmCamerasFoldoutOn = x.newValue);
                root.Add(cmCamerasFoldout);

                optionsFoldout = new Foldout() { value = optionsFoldoutOn, name = "Options", text = "Options" };
                optionsFoldout.Add(BookmarksOverlayHelper.OptionsVE());
                optionsFoldout.RegisterValueChangedCallback((x) => optionsFoldoutOn = x.newValue);
                root.Add(optionsFoldout);

                return root;
            }
        }
    }
}
#endif