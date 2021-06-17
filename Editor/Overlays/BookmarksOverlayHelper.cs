#if UNITY_2021_2_OR_NEWER
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Toolbars;
using System.Linq;

#if CINEMACHINE
using Cinemachine;
#endif

namespace UnityEditor.SceneViewBookmarks
{
    internal static class BookmarksOverlayHelper
    {
        internal static VisualElement BuiltinBookmarksVE()
        {
            VisualElement root = new VisualElement();
            foreach (Viewpoint v in DefaultBookmarks.Instance.viewpoints)
            {
                Button btn = new Button(() => v.Load(SceneView.lastActiveSceneView)) { name = v.name, text = v.name };
                root.Add(btn);
            }
            return root;
        }

        internal static VisualElement SceneCamerasVE()
        {
            VisualElement root = new VisualElement();
            foreach (Camera c in SceneViewHelpers.FindObjectsOfTypeAll<Camera>())
            {
                Button btn = new Button(() =>
                {
                    var sceneview = SceneView.lastActiveSceneView;
                    sceneview.in2DMode = false;
                    if (sceneview.orthographic = c.orthographic)
                        sceneview.size = c.orthographicSize; // FIXME not working as expected
                    else
                        sceneview.cameraSettings.fieldOfView = c.fieldOfView;
                    sceneview.AlignViewToObject(c.transform);
                }) { name = c.name, text = c.name };
                root.Add(btn);
            }
            return root;
        }

#if CINEMACHINE
        internal static VisualElement SceneCMVCamerasVE()
        {
            VisualElement root = new VisualElement();
            foreach (CinemachineVirtualCamera c in SceneViewHelpers.FindObjectsOfTypeAll<CinemachineVirtualCamera>())
            {
                Button btn = new Button(() =>
                {
                    var sceneview = SceneView.lastActiveSceneView;
                    sceneview.in2DMode = false;
                    if (sceneview.orthographic = c.m_Lens.Orthographic)
                        sceneview.size = c.m_Lens.OrthographicSize; // FIXME not working as expected
                    else
                        sceneview.cameraSettings.fieldOfView = c.m_Lens.FieldOfView;
                    sceneview.AlignViewToObject(c.transform);
                    if (Bookmarks.soloVirtualCamerasOnSelection || Bookmarks.linkMainCamera)
                    {
                        Selection.activeGameObject = c.gameObject;
                        CinemachineBrain.SoloCamera = c;
                    }
                })
                { name = c.name, text = c.name };
                root.Add(btn);
            }
            return root;
        }
#endif

        internal static VisualElement SceneBookmarksVE()
        {
            VisualElement root = new VisualElement();
            foreach (Viewpoint v in SceneBookmarks.Viewpoints)
            {
                Button btn = new Button(() => v.Load(SceneView.lastActiveSceneView)) { name = v.name, text = v.name };
                root.Add(btn);
            }
            return root;
        }

        internal static VisualElement FilterVE()
        {
            VisualElement root = new VisualElement();
            Button a_btn = new Button(() => { SceneView.lastActiveSceneView.SetSearchFilter("LODGroup", 2); }) { name = "LOD Groups", text = "LOD Groups" };
            Button b_btn = new Button(() => { SceneView.lastActiveSceneView.SetSearchFilter("Collider", 2); }) { name = "Collider", text = "Collider" };
            Button c_btn = new Button(() => { SceneView.lastActiveSceneView.SetSearchFilter("Renderer", 2); }) { name = "Renderer", text = "Renderer" };
            Button d_btn = new Button(() => { SceneView.lastActiveSceneView.SetSearchFilter("Metadata", 2); }) { name = "Metadata", text = "Metadata" };

            root.Add(a_btn);
            root.Add(b_btn);
            root.Add(c_btn);
            root.Add(d_btn);
            return root;
        }

        internal static VisualElement MiscVE()
        {
            VisualElement root = new VisualElement();
            Button a_btn = new Button(() => { SceneView.lastActiveSceneView.FrameAll(); }) { name = "Frame All", text = "Frame All" };
            Button b_btn = new Button(() => { SceneView.lastActiveSceneView.FrameLayers(Tools.visibleLayers); }) { name = "Frame Visible Layers", text = "Frame Visible Layers" };
            Button c_btn = new Button(() => { SceneView.lastActiveSceneView.AlignToSelection(); }) { name = "Align to Selection", text = "Align to Selection" };
            Button d_btn = new Button(() => { SceneView.lastActiveSceneView.SnapCamera(Camera.main, Bookmarks.linkFOV, true); }) { name = "Snap Main Camera", text = "Snap Main Camera" };
            root.Add(a_btn);
            root.Add(b_btn);
            root.Add(c_btn);
            root.Add(d_btn);
            return root;
        }

        internal static VisualElement OptionsVE()
        {
            VisualElement root = new VisualElement();

            Button snapMainCameraButton = new Button(() => SceneView.lastActiveSceneView.SnapCamera(Camera.main, Bookmarks.linkFOV, true))
            {
                text = "Snap Camera", tooltip = "Snap MainCamera to SceneView."
            };
            root.Add(snapMainCameraButton);

            Toggle cameraLockToggle = new Toggle("Lock Camera") { tooltip = "Lock MainCamera to SceneView." };
            cameraLockToggle.value = Bookmarks.linkMainCamera;
            cameraLockToggle.AddToClassList("svbm-cell-checkbox");
            cameraLockToggle.RegisterValueChangedCallback((x) => Bookmarks.linkMainCamera = x.newValue);
            root.Add(cameraLockToggle);

            Toggle linkFovToggle = new Toggle("Link FOV") { tooltip = "Link FOV." };
            linkFovToggle.value = Bookmarks.linkFOV;
            linkFovToggle.AddToClassList("svbm-cell-checkbox");
            linkFovToggle.RegisterValueChangedCallback((x) => Bookmarks.linkFOV = x.newValue);
            root.Add(linkFovToggle);

            Toggle soloVirtualCamerasToggle = new Toggle("Solo CMVC") { tooltip = "Solo Virtual Cameras" };
            soloVirtualCamerasToggle.value = Bookmarks.soloVirtualCamerasOnSelection;
            soloVirtualCamerasToggle.AddToClassList("svbm-cell-checkbox");
            soloVirtualCamerasToggle.RegisterValueChangedCallback((x) => Bookmarks.soloVirtualCamerasOnSelection = x.newValue);
            root.Add(soloVirtualCamerasToggle);

            return root;
        }

        internal static VisualElement BookmarksSettingsDropdown(Texture image, string tooltip, EventCallback<MouseDownEvent> mouseDownEvent)
        {
            VisualElement root = new VisualElement();

            VisualElement blk = new VisualElement() { tooltip = tooltip };
            blk.AddToClassList("unity-text-element");
            blk.AddToClassList("unity-toolbar-button");
            blk.AddToClassList("unity-editor-toolbar-element");

            Image icon = new Image();
            icon.image = image;
            icon.AddToClassList("unity-image");
            icon.AddToClassList("unity-editor-toolbar-element__icon");
            blk.Add(icon);

            VisualElement arrow = new VisualElement();
            arrow.AddToClassList("unity-icon-arrow");
            blk.Add(arrow);

            blk.RegisterCallback(mouseDownEvent);

            root.Add(blk);

            return root;
        }

        internal static VisualElement BookmarksDropdown(string title, Texture image, string tooltip, EventCallback<MouseDownEvent> mouseDownEvent = null)
        {
            VisualElement root = new VisualElement();

            VisualElement blk = new VisualElement() { tooltip = tooltip };
            blk.AddToClassList("unity-text-element");
            blk.AddToClassList("unity-toolbar-button");
            blk.AddToClassList("unity-editor-toolbar-element");

            Image icon = new Image();
            icon.image = image;
            icon.AddToClassList("unity-image");
            icon.AddToClassList("unity-editor-toolbar-element__icon");
            blk.Add(icon);

            TextElement textElement = new TextElement() { text = title };
            textElement.AddToClassList("unity-text-element");
            textElement.AddToClassList("unity-editor-toolbar-element__label");
            blk.Add(textElement);

            VisualElement arrow = new VisualElement();
            arrow.AddToClassList("unity-icon-arrow");
            blk.Add(arrow);

            blk.RegisterCallback(mouseDownEvent);

            root.Add(blk);

            return root;
        }

        internal static VisualElement AddCreateBookmarkButton(string text = "+")
        {
            Button button = new Button(() => AddViewpointPopupWindow.Open(SceneView.lastActiveSceneView)) { text = text };
            return button;
        }

        internal static GenericMenu UserBookmarksMenu(SceneView sceneview)
        {
            GenericMenu menu = new GenericMenu();
            foreach (Viewpoint v in DefaultBookmarks.Instance.viewpoints)
            {
                menu.AddItem(new GUIContent(v.name), false, () => v.Load(sceneview));
            }
            return menu;
        }

        internal static GenericMenu SceneBookmarksMenu(SceneView sceneview)
        {
            GenericMenu menu = new GenericMenu();
            foreach (Viewpoint v in SceneBookmarks.Viewpoints)
            {
                menu.AddItem(new GUIContent(v.name), false, () => v.Load(sceneview));
            }
            return menu;
        }

        internal static GenericMenu SceneCamerasMenu(SceneView sceneview)
        {
            GenericMenu menu = new GenericMenu();
            foreach (Camera c in SceneViewHelpers.FindObjectsOfTypeAll<Camera>())
            {
                menu.AddItem(new GUIContent(c.name), false, () =>
                {
                    sceneview.in2DMode = false;
                    if (sceneview.orthographic = c.orthographic)
                        sceneview.size = c.orthographicSize; // FIXME not working as expected
                    else
                        sceneview.cameraSettings.fieldOfView = c.fieldOfView;
                    sceneview.AlignViewToObject(c.transform);
                });
            }
            return menu;
        }

#if CINEMACHINE
        internal static GenericMenu SceneVirtualCamerasMenu(SceneView sceneview)
        {
            GenericMenu menu = new GenericMenu();
            foreach (CinemachineVirtualCamera c in SceneViewHelpers.FindObjectsOfTypeAll<CinemachineVirtualCamera>())
            {
                menu.AddItem(new GUIContent(c.name), false, () =>
                {
                    sceneview.in2DMode = false;
                    if (sceneview.orthographic = c.m_Lens.Orthographic)
                        sceneview.size = c.m_Lens.OrthographicSize; // FIXME not working as expected
                    else
                        sceneview.cameraSettings.fieldOfView = c.m_Lens.FieldOfView;
                    sceneview.AlignViewToObject(c.transform);
                    if (Bookmarks.soloVirtualCamerasOnSelection || Bookmarks.linkMainCamera)
                    {
                        Selection.activeGameObject = c.gameObject;
                        CinemachineBrain.SoloCamera = c;
                    }
                });
            }
            return menu;
        }
#endif

        internal static GenericMenu FilterMenu(SceneView sceneview)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Filter/LOD Groups", "Filters View."), false, () => sceneview.SetSearchFilter("LODGroup", 2));
            menu.AddItem(new GUIContent("Filter/Colliders", "Filters View."), false, () => sceneview.SetSearchFilter("Collider", 2));
            menu.AddItem(new GUIContent("Filter/Renderer", "Filters View."), false, () => sceneview.SetSearchFilter("Renderer", 2));
            menu.AddItem(new GUIContent("Filter/Metadata", "Filters View."), false, () => sceneview.SetSearchFilter("Metadata", 2));
            return menu;
        }

        internal static GenericMenu ExtraToolsMenu(SceneView sceneview)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Frame All"), false, () => { sceneview.FrameAll(); });
            menu.AddItem(new GUIContent("Frame Visible Layers"), false, () => { sceneview.FrameLayers(Tools.visibleLayers); });
            menu.AddItem(new GUIContent("Align to Selection"), false, () => sceneview.AlignToSelection());
            menu.AddItem(new GUIContent("Bookmarks Editor"), false, () => BookmarkEditorWindow.Open());
            menu.AddSeparator("Options/");
            menu.AddItem(new GUIContent("Options/Clear All Bookmarks"), false, () => SceneBookmarks.Clear());
            menu.AddItem(new GUIContent("Options/Show as GUI"), Bookmarks.showAsSceneViewGUI, () => Bookmarks.showAsSceneViewGUI = !Bookmarks.showAsSceneViewGUI);
            return menu;
        }

        internal static GenericMenu CameraToolsMenu(SceneView sceneview)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Snap Main Camera"), false, () => sceneview.SnapCamera(Camera.main, Bookmarks.linkFOV, true));
            menu.AddItem(new GUIContent("Lock MainCamera to SceneView"), Bookmarks.linkMainCamera, () => Bookmarks.linkMainCamera = !Bookmarks.linkMainCamera);
            menu.AddItem(new GUIContent("Link FOV"), Bookmarks.linkFOV, () => Bookmarks.linkFOV = !Bookmarks.linkFOV);
#if CINEMACHINE
            menu.AddItem(new GUIContent("Solo Virtual Cameras"), Bookmarks.soloVirtualCamerasOnSelection, () => Bookmarks.soloVirtualCamerasOnSelection = !Bookmarks.soloVirtualCamerasOnSelection);
#endif
            return menu;
        }
    }
}
#endif