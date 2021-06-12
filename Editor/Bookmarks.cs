using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

#if CINEMACHINE
using Cinemachine;
#endif

#if DISCOVERY_MENU
using UnityEditor.DiscoveryMenu;
#endif

// DONE : make it work on GameView Cameras
// FIXED : loading a new scene throws errors when Camera is linked.
// FIXED : camera's overweritten on scene load if Link was left on.

// TODO : move built-in views to Editor Settings
// FIXME : if SOLO isn't active and Link is, the same camera is applied changes.
// FIXME : if link is active and a bookmark is selected, the camera moves to that location. Add a warning ?
// FIXME : switching from a 2D view to a Camera gives weird results

namespace UnityEditor.SceneViewBookmarks
{
    [InitializeOnLoad]
    [System.Serializable]
    public partial class Bookmarks
    {
        internal List<Viewpoint> builtinViewpoints;
        [NonSerialized] public List<Viewpoint> sceneViewpoints;

        static bool showAsSceneViewGUI = default;
        static bool linkMainCamera = default;
        static bool linkFOV = default;

#if CINEMACHINE
        static bool soloVirtualCamerasOnSelection = default;
#endif

        static Bookmarks()
        {
            instance = new Bookmarks();

            instance.builtinViewpoints = DefaultBookmarks.Instance.viewpoints;

            instance.sceneViewpoints = new List<Viewpoint>();
            SceneView.duringSceneGui += SceneView_duringSceneGui;
            EditorApplication.quitting += EditorApplication_quitting;
            EditorSceneManager.activeSceneChangedInEditMode += EditorSceneManager_activeSceneChangedInEditMode;

#if DISCOVERY_MENU
            HotBoxMenuItem hotBoxMenuItem = new HotBoxMenuItem("Views");
            hotBoxMenuItem.Refresh = () => { hotBoxMenuItem.menu = QuickAccessMenu(SceneView.lastActiveSceneView); };
            SceneViewHotBox.menuItems.Add("Views", hotBoxMenuItem);
#endif

            LoadSceneBookmarks(SceneManager.GetActiveScene());
        }

        private static void EditorSceneManager_activeSceneChangedInEditMode(Scene arg0, Scene arg1)
        {
            linkMainCamera = false;
            SaveSceneBookmarks(arg0);
            LoadSceneBookmarks(arg1);
        }

        private static void LoadSceneBookmarks(Scene scene)
        {
            if (scene == null)
            {
                Debug.LogWarning("Couldn't find Scene.");
                return;
            }
            //Debug.LogFormat("<color=cyan>Loading</color> {0} <color=cyan>SceneView Bookmarks</color>", scene.name);
            var path = scene.path;
            //Debug.LogFormat("<color=red>{0}</color>", scene.path);
            path = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
            path = Path.Combine(path, "bookmarks.asset");
            var sceneBookmarks = AssetDatabase.LoadAssetAtPath<SceneBookmarks>(path);
            if (sceneBookmarks != null)
                instance.sceneViewpoints = sceneBookmarks.viewpoints;
            else
                instance.sceneViewpoints.Clear();
        }

        private static void SaveSceneBookmarks(Scene scene)
        {
            //Debug.LogFormat("<color=cyan>Saving</color> {0} <color=cyan>SceneView Bookmarks</color>", scene.name);
            if (instance.sceneViewpoints.Count == 0)
                return;
            var path = scene.path;
            path = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, "bookmarks.asset");

            var sceneBookmarks = AssetDatabase.LoadAssetAtPath<SceneBookmarks>(path);
            if (sceneBookmarks == null)
            {
                sceneBookmarks = SceneBookmarks.CreateInstance<SceneBookmarks>();
                AssetDatabase.CreateAsset(sceneBookmarks, path);
            }
            sceneBookmarks.viewpoints = new List<Viewpoint>(instance.sceneViewpoints);
            AssetDatabase.SaveAssets();
        }

        private static void EditorApplication_quitting()
        {
            SaveSceneBookmarks(SceneManager.GetActiveScene());
        }

        private static void SceneView_duringSceneGui(SceneView sceneview)
        {
            var e = Event.current;
            if (e.type == EventType.KeyUp)
            {
                Viewpoint shortcutView;
                if ((shortcutView = Instance?.sceneViewpoints.Find((x) => x.shortcut == e.keyCode)) != null)
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
                foreach (Viewpoint v in Instance.sceneViewpoints)
                {
                    if (GUILayout.Button(v.name))
                        v.Load(sceneview);
                }
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
                Handles.EndGUI();
            }
        }

        static void AlignToSelection(SceneView sceneview)
        {
            var selectedObject = Selection.activeGameObject;
            if (selectedObject == null)
                return;

            sceneview.in2DMode = false;
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

        static void SnapCamera(Camera camera, SceneView sceneview, bool linkFOV = false, bool undo = false)
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
                    if (undo)
                        Undo.RegisterCompleteObjectUndo(vcam.gameObject, "Snap Cinemachine Virtual Camera");

                    vcam.transform.rotation = sceneview.rotation;
                    vcam.transform.position = sceneview.pivot - sceneview.camera.transform.forward * sceneview.cameraDistance;
                    if (linkFOV)
                        vcam.m_Lens.FieldOfView = sceneview.camera.fieldOfView;

                    if (!undo)
                    {
                        EditorUtility.SetDirty(vcam.transform);
                        EditorUtility.SetDirty(vcam);
                    }
                }
                return;
            }
#endif
            if (undo)
                Undo.RegisterCompleteObjectUndo(camera.gameObject, "Snap Camera");

            camera.transform.rotation = sceneview.rotation;
            camera.transform.position = sceneview.pivot - sceneview.camera.transform.forward * sceneview.cameraDistance;
            if (linkFOV)
                camera.fieldOfView = sceneview.camera.fieldOfView;

            if (!undo)
            {
                EditorUtility.SetDirty(camera.transform);
                EditorUtility.SetDirty(camera);
            }
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

        public Viewpoint this[int index]
        {
            get => sceneViewpoints[index];
        }

        public static int Count
        {
            get => Instance.sceneViewpoints.Count;
        }
    }
}