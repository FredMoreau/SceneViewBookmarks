using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

#if CINEMACHINE
using Cinemachine;
#endif

#if DISCOVERY_MENU
using UnityEditor.DiscoveryMenu;
#endif

namespace UnityEditor.SceneViewBookmarks
{
    [InitializeOnLoad]
    public class BookmarksMenu
    {
        static BookmarksMenu()
        {
            SceneView.duringSceneGui += SceneView_duringSceneGui;
            EditorSceneManager.activeSceneChangedInEditMode += EditorSceneManager_activeSceneChangedInEditMode;

#if DISCOVERY_MENU
            HotBoxMenuItem hotBoxMenuItem = new HotBoxMenuItem("Views");
            hotBoxMenuItem.Refresh = () => { hotBoxMenuItem.menu = QuickAccessMenu(SceneView.lastActiveSceneView); };
            SceneViewHotBox.menuItems.Add("Views", hotBoxMenuItem);
#endif
        }

        private static void EditorSceneManager_activeSceneChangedInEditMode(Scene arg0, Scene arg1)
        {
            Bookmarks.linkMainCamera = false;
        }

        private static void SceneView_duringSceneGui(SceneView sceneview)
        {
            var e = Event.current;
            if (e.type == EventType.KeyUp)
            {
                Viewpoint shortcutView;
                if ((shortcutView = SceneBookmarks.Find((x) => x.shortcut == e.keyCode)) != null)
                {
                    shortcutView.Load(sceneview);
                    e.Use();
                }
            }
            if (e.control && e.isScrollWheel)
            {
                sceneview.cameraSettings.fieldOfView += e.delta.y;
                sceneview.orthographic = sceneview.cameraSettings.fieldOfView < 4;
                e.Use();
            }
            else if (e.control && e.type == EventType.MouseDown && e.button == 1)
            {
                QuickAccessMenu(sceneview).ShowAsContext();
            }

            if (Bookmarks.linkMainCamera)
            {
                sceneview.SnapCamera(Camera.main, Bookmarks.linkFOV);
            }

            if (Bookmarks.showAsSceneViewGUI)
            {
                var h = sceneview.position.height;
                Handles.BeginGUI();
                GUILayout.BeginVertical();
                GUILayout.Space(h - 64);

                GUILayout.BeginHorizontal();
                foreach (Viewpoint v in SceneBookmarks.Viewpoints)
                {
                    if (GUILayout.Button(v.name))
                        v.Load(sceneview);
                }
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
                Handles.EndGUI();
            }
        }

        public static GenericMenu QuickAccessMenu(SceneView sceneview)
        {
            GenericMenu menu = new GenericMenu();

            if (Bookmarks.linkMainCamera)
            {
                menu.AddItem(new GUIContent("Unlock MainCamera from SceneView"), false, () => Bookmarks.linkMainCamera = false);
                menu.AddSeparator("");
            }

            foreach (Viewpoint v in DefaultBookmarks.Instance.viewpoints)
            {
                menu.AddItem(new GUIContent(v.name), false, () => v.Load(sceneview));
            }

            menu.AddSeparator("");

            //foreach (Camera c in Object.FindObjectsOfType<Camera>()) // DONE : add the includeInactive parameter when possible
            foreach (Camera c in SceneViewHelpers.FindObjectsOfTypeAll<Camera>())
            {
                menu.AddItem(new GUIContent("Cameras/" + c.name), false, () =>
                {
                    sceneview.in2DMode = false;
                    if (sceneview.orthographic = c.orthographic)
                        sceneview.size = c.orthographicSize; // FIXME not working as expected
                else
                        sceneview.cameraSettings.fieldOfView = c.fieldOfView;
                    sceneview.AlignViewToObject(c.transform);
                });
            }

#if CINEMACHINE
            //foreach (CinemachineVirtualCamera c in Object.FindObjectsOfType<CinemachineVirtualCamera>()) // DONE : add the includeInactive parameter when possible
            foreach (CinemachineVirtualCamera c in SceneViewHelpers.FindObjectsOfTypeAll<CinemachineVirtualCamera>())
            {
                menu.AddItem(new GUIContent("Virtual Cameras/" + c.name), false, () =>
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
#endif

            menu.AddSeparator("");

            foreach (Viewpoint v in SceneBookmarks.Viewpoints)
            {
                menu.AddItem(new GUIContent(v.name), false, () => v.Load(sceneview));
            }

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Bookmark View", "Saves the current view as a Bookmark."), false, () => AddViewpointPopupWindow.Open(sceneview));
            menu.AddSeparator("");
            //menu.AddItem(new GUIContent("Filter/LOD", "Filters View."), false, () => SceneModeUtility.SearchForType(typeof(LODGroup)));
            menu.AddItem(new GUIContent("Filter/LOD Groups", "Filters View."), false, () => sceneview.SetSearchFilter("LODGroup", 2));
            menu.AddItem(new GUIContent("Filter/Colliders", "Filters View."), false, () => sceneview.SetSearchFilter("Collider", 2));
            menu.AddItem(new GUIContent("Filter/Renderer", "Filters View."), false, () => sceneview.SetSearchFilter("Renderer", 2));
            menu.AddItem(new GUIContent("Filter/Metadata", "Filters View."), false, () => sceneview.SetSearchFilter("Metadata", 2));
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Frame All"), false, () => { sceneview.FrameAll(); });
            menu.AddItem(new GUIContent("Frame Visible Layers"), false, () => { sceneview.FrameLayers(Tools.visibleLayers); });
            menu.AddItem(new GUIContent("Align to Selection"), false, () => sceneview.AlignToSelection());
            menu.AddItem(new GUIContent("Snap Main Camera"), false, () => sceneview.SnapCamera(Camera.main, Bookmarks.linkFOV, true));

            // TODO : experiment with sceneview.rootVisualElement
            //menu.AddItem(new GUIContent("Options/Show as UIElements"), false, () =>
            //{
            //    sceneview.rootVisualElement.Add(new Button(() => Debug.Log("TEST")) { text = "TEST" });
            //});

            menu.AddItem(new GUIContent("Options/Show as GUI"), Bookmarks.showAsSceneViewGUI, () => Bookmarks.showAsSceneViewGUI = !Bookmarks.showAsSceneViewGUI);
            menu.AddItem(new GUIContent("Options/Lock MainCamera to SceneView"), Bookmarks.linkMainCamera, () => Bookmarks.linkMainCamera = !Bookmarks.linkMainCamera);
            menu.AddItem(new GUIContent("Options/Link FOV"), Bookmarks.linkFOV, () => Bookmarks.linkFOV = !Bookmarks.linkFOV);
#if CINEMACHINE
            menu.AddItem(new GUIContent("Options/Solo Virtual Cameras"), Bookmarks.soloVirtualCamerasOnSelection, () => Bookmarks.soloVirtualCamerasOnSelection = !Bookmarks.soloVirtualCamerasOnSelection);
#endif
            menu.AddSeparator("Options/");
            menu.AddItem(new GUIContent("Options/Bookmarks Editor"), false, () => BookmarkEditorWindow.Open());
            menu.AddItem(new GUIContent("Options/Clear All Bookmarks"), false, () => SceneBookmarks.Clear());
            //menu.AddItem(new GUIContent("Options/Enable AA"), false, () => { sceneview.antiAlias = 8; });
            return menu;
        }
    }
}