using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

#if CINEMACHINE
using Cinemachine;
#endif

#if DISCOVERY_MENU
using UnityEditor.DiscoveryMenu;
#endif

// TODO : save in Project Settings
// TODO : make it work on GameView Cameras
// FIXME : loading a new scene throws errors when Camera is linked.
// FIXME : camera's overweritten on scene load if Link was left on.
// FIXME : if SOLO isn't active and Link is, the same camera is applied changes. 

namespace UnityEditor.SceneViewBookmarks
{
    [InitializeOnLoad]
    [System.Serializable]
    public class Bookmarks
    {
        private List<Viewpoint> builtin_viewpoints;
        public List<Viewpoint> viewpoints;

        static bool showAsSceneViewGUI = default;
        static bool linkMainCamera = default;
        static bool linkFOV = default;
        static readonly string path = Path.Combine(Application.dataPath, "SceneViewBookmarks.json");
        static readonly string[] reservedTopMenuItemNames = new string[1] { "Options" };

#if CINEMACHINE
        static bool soloVirtualCamerasOnSelection = default;
#endif

        static Bookmarks()
        {
            instance = new Bookmarks();
            instance.builtin_viewpoints = new List<Viewpoint>();

            var allDisabled = new SceneView.SceneViewState();
            allDisabled.SetAllEnabled(false);

            var perspective_settings = new ViewSettings(Vector3.zero, Quaternion.Euler(30f, -50f, 0), 100f, false, 60f, false, SceneView.GetBuiltinCameraMode(DrawCameraMode.Textured), true, true, new SceneView.SceneViewState(), true);
            var perspective_view = new Viewpoint("Perspective (Shaded)", perspective_settings, -1 ^ (1 << 5), 0, true, () =>
            {
                //Debug.Log(Tools.visibleLayers.ToBinaryString());
                //SceneView.lastActiveSceneView.FrameLayers(Tools.visibleLayers);
            });
            instance.builtin_viewpoints.Add(perspective_view);

            var top_settings = new ViewSettings(Vector3.zero, Quaternion.LookRotation(Vector3.down, Vector3.forward), 100f, true, 60f, false, SceneView.GetBuiltinCameraMode(DrawCameraMode.Wireframe), true, false, allDisabled, true);
            var top_view = new Viewpoint("Top (Wireframe, UI Hidden)", top_settings, -1 ^ (1 << 5), 0, true, () =>
            {
                //Debug.Log(Tools.visibleLayers.ToBinaryString());
                //SceneView.lastActiveSceneView.FrameLayers(Tools.visibleLayers);
            });
            instance.builtin_viewpoints.Add(top_view);

            var front_settings = new ViewSettings(Vector3.zero, Quaternion.LookRotation(Vector3.forward, Vector3.up), 100f, true, 60f, false, SceneView.GetBuiltinCameraMode(DrawCameraMode.Wireframe), true, false, allDisabled, true);
            var front_view = new Viewpoint("Front (Wireframe, UI Hidden)", front_settings, -1 ^ (1 << 5), 0, true, () =>
            {
                //Debug.Log(Tools.visibleLayers.ToBinaryString());
                //SceneView.lastActiveSceneView.FrameLayers(Tools.visibleLayers);
            });
            instance.builtin_viewpoints.Add(front_view);

            var ui_settings = new ViewSettings(Vector3.zero, Quaternion.identity, 100f, true, 60f, true, SceneView.GetBuiltinCameraMode(DrawCameraMode.Textured), true, true, allDisabled, true);
            var ui_view = new Viewpoint("2D (UI Only)", ui_settings, 0 ^ (1 << 5), 0, true, () =>
            {
                //Debug.Log(Tools.visibleLayers.ToBinaryString());
                SceneView.lastActiveSceneView.FrameLayers(Tools.visibleLayers);
            });
            instance.builtin_viewpoints.Add(ui_view);

            instance.viewpoints = new List<Viewpoint>();
            instance.LoadFromJson(path);
            SceneView.duringSceneGui += SceneView_duringSceneGui;
            EditorApplication.quitting += EditorApplication_quitting;

            HotBoxMenuItem hotBoxMenuItem = new HotBoxMenuItem("Views");
            hotBoxMenuItem.Refresh = () => { hotBoxMenuItem.menu = QuickAccessMenu(SceneView.lastActiveSceneView); };
            SceneViewHotBox.menuItems.Add("Views", hotBoxMenuItem);
        }

        private static void EditorApplication_quitting()
        {
            Instance?.SaveToJson(path);
        }

        private static void SceneView_duringSceneGui(SceneView sceneview)
        {
            if (linkMainCamera)
            {
                SnapCamera(Camera.main, sceneview, linkFOV);
            }

            if (showAsSceneViewGUI)
            {
                var h = sceneview.position.height;
                Handles.BeginGUI();
                GUILayout.BeginVertical();
                GUILayout.Space(h - 64);

                GUILayout.BeginHorizontal();
                foreach (Viewpoint v in Instance.viewpoints)
                {
                    if (GUILayout.Button(v.name))
                        v.Load(sceneview);
                }
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
                Handles.EndGUI();
            }

            var e = Event.current;
            if (e.control && e.type == EventType.MouseDown && e.button == 1)
            {
                QuickAccessMenu(sceneview).ShowAsContext();
            }
        }

        public static GenericMenu QuickAccessMenu(SceneView sceneview)
        {
            GenericMenu menu = new GenericMenu();

            if (linkMainCamera)
			{
                menu.AddItem(new GUIContent("Unlock MainCamera from SceneView"), false, () => linkMainCamera = false);
                menu.AddSeparator("");
            }

            foreach (Viewpoint v in Instance.builtin_viewpoints)
            {
                menu.AddItem(new GUIContent(v.name), false, () => v.Load(sceneview));
            }

            menu.AddSeparator("");
            
            //foreach (Camera c in Object.FindObjectsOfType<Camera>()) // TODO : add the includeInactive parameter when possible
            foreach (Camera c in FindObjectsOfTypeAll<Camera>())
            {
                menu.AddItem(new GUIContent("Cameras/" + c.name), false, () => sceneview.AlignViewToObject(c.transform));
            }

#if CINEMACHINE
            //foreach (CinemachineVirtualCamera c in Object.FindObjectsOfType<CinemachineVirtualCamera>()) // TODO : add the includeInactive parameter when possible
            foreach (CinemachineVirtualCamera c in FindObjectsOfTypeAll<CinemachineVirtualCamera>())
            {
                menu.AddItem(new GUIContent("Virtual Cameras/" + c.name), false, () =>
                {
                    sceneview.AlignViewToObject(c.transform);
                    if (soloVirtualCamerasOnSelection || linkMainCamera)
					{
                        Selection.activeGameObject = c.gameObject;
                        CinemachineBrain.SoloCamera = c;
					}
                });
            }
#endif

            menu.AddSeparator("");

            foreach (Viewpoint v in Instance.viewpoints)
            {
                menu.AddItem(new GUIContent(v.name), false, () => v.Load(sceneview));
            }

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Bookmark View", "Saves the current view as a Bookmark."), false, () => SavePopup.Open(sceneview));
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Frame All"), false, () => { sceneview.FrameAll(); });
            menu.AddItem(new GUIContent("Frame Visible Layers"), false, () => { sceneview.FrameLayers(Tools.visibleLayers); });
            menu.AddItem(new GUIContent("Align to Selection"), false, () => AlignToSelection(sceneview));
            menu.AddItem(new GUIContent("Snap Main Camera"), false, () => SnapCamera(Camera.main, sceneview));
            menu.AddItem(new GUIContent("Options/Show as GUI"), showAsSceneViewGUI, () => showAsSceneViewGUI = !showAsSceneViewGUI);
            menu.AddItem(new GUIContent("Options/Lock MainCamera to SceneView"), linkMainCamera, () => linkMainCamera = !linkMainCamera);
            menu.AddItem(new GUIContent("Options/Link FOV"), linkFOV, () => linkFOV = !linkFOV);
#if CINEMACHINE
            menu.AddItem(new GUIContent("Options/Solo Virtual Cameras"), soloVirtualCamerasOnSelection, () => soloVirtualCamerasOnSelection = !soloVirtualCamerasOnSelection);
#endif
            menu.AddSeparator("Options/");
            menu.AddItem(new GUIContent("Options/Bookmarks Editor"), false, () => BookmarkEditorWindow.Open());
            menu.AddItem(new GUIContent("Options/Clear All Bookmarks"), false, () => Instance.viewpoints.Clear());
            menu.AddItem(new GUIContent("Options/Save to Disk"), false, () => Instance?.SaveToJson(path));
            //menu.AddSeparator("Options/");
            //menu.AddItem(new GUIContent("Options/Enable AA"), false, () => { sceneview.antiAlias = 8; });
            return menu;
        }

        static void AlignToSelection(SceneView sceneview)
        {
            var selectedObject = Selection.activeGameObject;
            if (selectedObject == null)
                return;

            sceneview.AlignViewToObject(selectedObject.transform);

            //sceneview.rotation = selectedObject.transform.rotation;

            //if (Physics.Raycast(selectedObject.transform.position, selectedObject.transform.forward, out RaycastHit hit, 1000f, -5, QueryTriggerInteraction.Collide))
            //{
            //    sceneview.pivot = hit.point;

            //}
            //else
            //{
            //    sceneview.pivot = selectedObject.transform.position + selectedObject.transform.forward * 10f;
            //}

            var cam = selectedObject.GetComponent<Camera>();
            if (cam != null)
                sceneview.cameraSettings.fieldOfView = cam.fieldOfView;
#if CINEMACHINE
            var vcam = selectedObject.GetComponent<CinemachineVirtualCamera>();
            if (vcam != null)
                sceneview.cameraSettings.fieldOfView = vcam.m_Lens.FieldOfView;
#endif
        }

        static void SnapCamera (Camera camera, SceneView sceneview, bool linkFOV = false)
        {
#if CINEMACHINE
            CinemachineBrain brain = camera.GetComponent<CinemachineBrain>();
            if (brain != null)
            {
                if (brain.ActiveVirtualCamera == null)
                    return;

                var vcam = (CinemachineVirtualCamera)brain.ActiveVirtualCamera;
                if (vcam.GetCinemachineComponent(CinemachineCore.Stage.Body) == null && vcam.GetCinemachineComponent(CinemachineCore.Stage.Aim) == null)
                {
                    vcam.transform.rotation = sceneview.rotation;
                    vcam.transform.position = sceneview.pivot - sceneview.camera.transform.forward * sceneview.cameraDistance;
                    if (linkFOV)
                        vcam.m_Lens.FieldOfView = sceneview.camera.fieldOfView;
                }
                return;
            }
#endif

            camera.transform.rotation = sceneview.rotation;
            camera.transform.position = sceneview.pivot - sceneview.camera.transform.forward * sceneview.cameraDistance;
            if (linkFOV)
                camera.fieldOfView = sceneview.camera.fieldOfView;
        }

        static Bookmarks instance;

        public static Bookmarks Instance
        {
            get
            {
                if (instance == null)
                    instance = new Bookmarks();

                return instance;
            }
        }

        public Viewpoint this [int index]
        {
            get => viewpoints[index];
        }

        public static int Count
        {
            get => Instance.viewpoints.Count;
        }

        public void LoadFromJson (string path)
        {
            if (File.Exists(path))
                JsonUtility.FromJsonOverwrite(File.ReadAllText(path), this);
        }

        public void SaveToJson(string path)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));

            File.WriteAllText(path, JsonUtility.ToJson(this, true));
        }

        public class SavePopup : EditorWindow
        {
            public SceneView sceneview;
            public string bookmarkName = "New Bookmark";

            public static void Open(SceneView sceneview)
            {
                SavePopup window = CreateInstance<SavePopup>();
                window.position = new Rect(sceneview.position.x + sceneview.position.width * 0.5f - 125, sceneview.position.y + sceneview.position.height * 0.5f - 75, 250, 150);
                window.ShowPopup();
                window.titleContent = new GUIContent("Bookmark Scene View as");
                window.sceneview = sceneview;
            }

            void OnGUI()
            {
                EditorGUILayout.LabelField("Bookmark Settings", EditorStyles.wordWrappedLabel);
                bookmarkName = EditorGUILayout.TextField(bookmarkName);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Create"))
                {
                    // TODO : filter reserved names
                    Bookmarks.Instance.viewpoints.Add(new Viewpoint(bookmarkName, sceneview));
                    this.Close();
                }
                else if (GUILayout.Button("Cancel"))
                {
                    this.Close();
                }
                GUILayout.EndHorizontal();
            }
        }

        public static List<T> FindObjectsOfTypeAll<T>()
        {
            List<T> results = new List<T>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var s = SceneManager.GetSceneAt(i);
                if (s.isLoaded)
                {
                    var allGameObjects = s.GetRootGameObjects();
                    for (int j = 0; j < allGameObjects.Length; j++)
                    {
                        var go = allGameObjects[j];
                        results.AddRange(go.GetComponentsInChildren<T>(true));
                    }
                }
            }
            return results;
        }
    }
}