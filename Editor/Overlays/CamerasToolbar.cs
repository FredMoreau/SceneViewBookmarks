#if UNITY_2021_2_OR_NEWER
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Toolbars;

#if CINEMACHINE
using Cinemachine;
#endif

namespace UnityEditor.SceneViewBookmarks
{
    [EditorToolbarElement(id, typeof(SceneView))]
    class BookmarksEditorToolbarCameras : VisualElement, IAccessContainerWindow
    {
        public const string id = "Cameras/Cameras";

        public EditorWindow containerWindow { get; set; }

        BookmarksEditorToolbarCameras()
        {
            tooltip = "";
            UpdateContent();
            // TODO : find event to update the list of cameras
        }

        void UpdateContent()
        {
            Clear();
            Add(BookmarksOverlayHelper.BookmarksDropdown("Cameras",
                EditorGUIUtility.IconContent("SceneViewCamera").image,
                "",
                OnClick));
        }

        void OnClick(MouseDownEvent evt)
        {
            BookmarksOverlayHelper.SceneCamerasMenu(containerWindow as SceneView).DropDown(worldBound);
        }
    }

#if CINEMACHINE
    [EditorToolbarElement(id, typeof(SceneView))]
    [Icon("Packages/com.unity.cinemachine/Gizmos/cm_logo.png")]
    class BookmarksEditorToolbarVirtualCameras : VisualElement, IAccessContainerWindow
    {
        public const string id = "Cameras/VirtualCameras";

        public EditorWindow containerWindow { get; set; }

        BookmarksEditorToolbarVirtualCameras()
        {
            tooltip = "";
            UpdateContent();
            // TODO : find event to update the list of cameras
        }

        void UpdateContent()
        {
            Clear();
            Add(BookmarksOverlayHelper.BookmarksDropdown("Cinemachine",
                AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.cinemachine/Gizmos/cm_logo.png"),
                "",
                OnClick));
        }

        void OnClick(MouseDownEvent evt)
        {
            BookmarksOverlayHelper.SceneVirtualCamerasMenu(containerWindow as SceneView).DropDown(worldBound);
        }
    }
#endif

    [EditorToolbarElement(id, typeof(SceneView))]
    class BookmarksEditorToolbarAddCameraButton : EditorToolbarButton, IAccessContainerWindow
    {
        public const string id = "Cameras/AddButton";

        public EditorWindow containerWindow { get; set; }

        public BookmarksEditorToolbarAddCameraButton()
        {
            icon = (Texture2D)EditorGUIUtility.IconContent("d_Toolbar Plus@2x").image;
            tooltip = "Create new Camera.";
            clicked += () =>
            {
#if CINEMACHINE
                var cm = new GameObject("Virtual Camera", new System.Type[1] { typeof(CinemachineVirtualCamera) }).GetComponent<CinemachineVirtualCamera>();
                (containerWindow as SceneView)?.SnapVirtualCamera(cm, true, false);
#else
                var cam = new GameObject("Camera", new System.Type[1] { typeof(Camera) }).GetComponent<Camera>();
                (containerWindow as SceneView)?.SnapCamera(cam, true, false);
#endif
            };
        }
    }

    [EditorToolbarElement(id, typeof(SceneView))]
    class BookmarksEditorToolbarCamerasDropdown : VisualElement, IAccessContainerWindow
    {
        public const string id = "Cameras/Options";

        public EditorWindow containerWindow { get; set; }

        BookmarksEditorToolbarCamerasDropdown()
        {
            tooltip = "";
            Add(BookmarksOverlayHelper.BookmarksSettingsDropdown(
                EditorGUIUtility.IconContent("d_SettingsIcon@2x").image,
                "",
                OnClick));
        }

        void OnClick(MouseDownEvent evt)
        {
            BookmarksOverlayHelper.CameraToolsMenu(containerWindow as SceneView).DropDown(worldBound);
        }
    }

    // TODO : add an item to force full panel
    //[EditorToolbarElement(id, typeof(SceneView))]
    //class BookmarksEditorToolbarCamerasForceDropdown : VisualElement, IAccessContainerWindow
    //{
    //    public const string id = "Cameras/Force";

    //    public EditorWindow containerWindow { get; set; }

    //    BookmarksEditorToolbarCamerasForceDropdown()
    //    {
    //        tooltip = "";
    //        var btn = new Button() { text = "#" };
    //        btn.RegisterCallback<MouseDownEvent>(OnClick);
    //        Add(btn);
    //    }

    //    void OnClick(MouseDownEvent evt)
    //    {
            
    //    }
    //}

    [Overlay(typeof(SceneView), "Cameras")]
    [Icon("Packages/com.unity.cinemachine/Gizmos/cm_logo.png")]
    public class CamerasToolbar : ToolbarOverlay
    {
        Foldout camerasFoldout, cmCamerasFoldout, optionsFoldout;
        bool camerasFoldoutOn = true, cmCamerasFoldoutOn = true, optionsFoldoutOn = false, forceFullPanel = false;

        CamerasToolbar() : base(
            BookmarksEditorToolbarCameras.id,
#if CINEMACHINE
            BookmarksEditorToolbarVirtualCameras.id,
#endif
            BookmarksEditorToolbarAddCameraButton.id,
            BookmarksEditorToolbarCamerasDropdown.id)
        { }
        public override VisualElement CreatePanelContent()
        {
            if (!collapsed && !forceFullPanel)
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

                var addButton = new Button(() =>
                {
#if CINEMACHINE
                    var cm = new GameObject("Virtual Camera", new System.Type[1] { typeof(CinemachineVirtualCamera) }).GetComponent<CinemachineVirtualCamera>();
                    (containerWindow as SceneView)?.SnapVirtualCamera(cm, true, false);
#else
                    var cam = new GameObject("Camera", new System.Type[1] { typeof(Camera) }).GetComponent<Camera>();
                    (containerWindow as SceneView)?.SnapCamera(cam, true, false);
#endif
                })
                { text = "New Camera" };
                root.Add(addButton);

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