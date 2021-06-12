//#define RESET_EDITOR_PREFS

using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.SceneViewBookmarks
{
    class DefaultBookmarks : ScriptableObject/*, ISerializationCallbackReceiver*/
    {
        [SerializeField] internal List<Viewpoint> viewpoints;

        private static DefaultBookmarks _instance;
        public static DefaultBookmarks Instance
        {
            get
            {
                if (_instance == null)
                    LoadOrCreate();
                return _instance;
            }
        }

        public static int Count { get => Instance.viewpoints.Count; }

        public Viewpoint this[int i]
        {
            get => viewpoints[i];
            set
            {
                viewpoints[i] = value;
                Save();
            }
        }

        public static void Add (Viewpoint viewpoint)
        {
            Instance.viewpoints.Add(viewpoint);
            Save();
        }

        public static void Remove (Viewpoint viewpoint)
        {
            Instance.viewpoints.Remove(viewpoint);
            Save();
        }

        private static void Save()
        {
            EditorPrefs.SetString("SceneView.Bookmarks.Default", JsonUtility.ToJson(_instance, false));
        }

        private static void LoadOrCreate()
        {
#if RESET_EDITOR_PREFS
            EditorPrefs.DeleteKey("SceneView.Bookmarks.Default");
#endif
            _instance = DefaultBookmarks.CreateInstance<DefaultBookmarks>();
            if (EditorPrefs.HasKey("SceneView.Bookmarks.Default"))
            {
                var epStr = EditorPrefs.GetString("SceneView.Bookmarks.Default");
                JsonUtility.FromJsonOverwrite(epStr, _instance);
                Debug.Log("<color=yellow>Default Loaded from Editor Prefs</color>");
            }
            else
            {
                _instance.viewpoints = GetDefaultBookmarks();
                Debug.Log("<color=yellow>Default Reset</color>");
                Save();
            }
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(Instance);
        }

        public static List<Viewpoint> GetDefaultBookmarks()
        {
            List<Viewpoint> viewpoints = new List<Viewpoint>();

            var allDisabled = new SceneView.SceneViewState();
            allDisabled.SetAllEnabled(false);

            var perspective_settings = new ViewSettings(Vector3.zero, Quaternion.Euler(30f, -50f, 0), 100f, false, 60f, false, SceneView.GetBuiltinCameraMode(DrawCameraMode.Textured), true, true, new SceneView.SceneViewState(), true);
            var perspective_view = new Viewpoint("Perspective (Shaded)", perspective_settings, -1 ^ (1 << 5), 0, (Viewpoint.Overrides)(-1) ^ Viewpoint.Overrides.Position);
            viewpoints.Add(perspective_view);

            var top_settings = new ViewSettings(Vector3.zero, Quaternion.LookRotation(Vector3.down, Vector3.forward), 100f, true, 60f, false, SceneView.GetBuiltinCameraMode(DrawCameraMode.Wireframe), true, false, allDisabled, true);
            var top_view = new Viewpoint("Top (Wireframe, UI Hidden)", top_settings, -1 ^ (1 << 5), 0, (Viewpoint.Overrides)(-1) ^ Viewpoint.Overrides.Position);
            viewpoints.Add(top_view);

            var front_settings = new ViewSettings(Vector3.zero, Quaternion.LookRotation(Vector3.forward, Vector3.up), 100f, true, 60f, false, SceneView.GetBuiltinCameraMode(DrawCameraMode.Wireframe), true, false, allDisabled, true);
            var front_view = new Viewpoint("Front (Wireframe, UI Hidden)", front_settings, -1 ^ (1 << 5), 0, (Viewpoint.Overrides)(-1) ^ Viewpoint.Overrides.Position);
            viewpoints.Add(front_view);

            var ui_settings = new ViewSettings(Vector3.zero, Quaternion.identity, 100f, true, 60f, true, SceneView.GetBuiltinCameraMode(DrawCameraMode.Textured), true, true, allDisabled, true);
            var ui_view = new Viewpoint("2D (UI Only)", ui_settings, 0 ^ (1 << 5), 0, () =>
            {
                SceneView.lastActiveSceneView.FrameLayers(Tools.visibleLayers);
            });
            viewpoints.Add(ui_view);

            return viewpoints;
        }

        //public void OnBeforeSerialize()
        //{
            
        //}

        //public void OnAfterDeserialize()
        //{
            
        //}
    }
}