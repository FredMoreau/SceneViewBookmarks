using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Collections.ObjectModel;

namespace UnityEditor.SceneViewBookmarks
{
    /// <summary>
    /// Stores SceneView Bookmarks of a Scene
    /// </summary>
    //[CreateAssetMenu(fileName ="bookmarks.asset", menuName ="SceneView/Bookmarks", order =0)]
    public class SceneBookmarks : ScriptableObject
    {
        #region static members
        public static Action onSceneBookmarksChange;

        static SceneBookmarks _current;
        public static SceneBookmarks current
        {
            get
            {
                if (_current == null)
                {
                    if (GetSavePath(SceneManager.GetActiveScene(), out string path))
                        _current = AssetDatabase.LoadAssetAtPath<SceneBookmarks>(path);
                    else
                        _current = SceneBookmarks.CreateInstance<SceneBookmarks>();
                }
                return _current;
            }
            private set
            {
                _current = value;
                if (_current == null)
                    _current = SceneBookmarks.CreateInstance<SceneBookmarks>();
                onSceneBookmarksChange?.Invoke();
            }
        }
        #endregion

        #region static members
        // TODO : keep a reference to the scene the bookmark belongs to
        // so that every user may have its own asset and avoid merge conflicts
        //[SerializeField] internal Scene scene;
        [SerializeField] private List<Viewpoint> _viewpoints = new List<Viewpoint>();
        #endregion

        #region instance indexer
        public Viewpoint this[int index]
        {
            get => _viewpoints[index];
        }
        #endregion

        #region static methods

        internal static bool GetSavePath(Scene scene, out string path)
        {
            path = "";
            if (scene == null || !scene.IsValid() || scene.path == string.Empty)
            {
                return false;
            }

            path = scene.path;
            path = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
            path = Path.Combine(path, "bookmarks.asset");
            return true;
        }

        internal static void LoadSceneBookmarks(Scene scene)
        {
            if (GetSavePath(scene, out string path))
                current = AssetDatabase.LoadAssetAtPath<SceneBookmarks>(path);
            else
                current = SceneBookmarks.CreateInstance<SceneBookmarks>();
        }

        internal static void LoadSceneBookmarks()
        {
            LoadSceneBookmarks(SceneManager.GetActiveScene());
        }

        internal static void SaveSceneBookmarks()
        {
            SaveSceneBookmarks(SceneManager.GetActiveScene());
        }

        internal static void SaveSceneBookmarks(Scene scene)
        {
            Debug.Log(scene.path);
            if (Count == 0)
                return;

            if (GetSavePath(scene, out string path))
            {
                if (AssetDatabase.GetAssetPath(current) == string.Empty)
                {
                    AssetDatabase.CreateFolder(Path.GetDirectoryName(scene.path), Path.GetFileNameWithoutExtension(scene.path));
                    AssetDatabase.CreateAsset(current, path);
                }
                EditorUtility.SetDirty(current);
                AssetDatabase.SaveAssets();
            }
            else
            {
                Debug.LogWarning("Cannot Save Scene Bookmarks\nPlease save your scene first.");
            }
        }

        public static ReadOnlyCollection<Viewpoint> Viewpoints
        {
            get => current._viewpoints.AsReadOnly();
        }

        public static int Count
        {
            get => current._viewpoints.Count;
        }

        public static void Add(Viewpoint viewpoint)
        {
            current._viewpoints.Add(viewpoint);
            onSceneBookmarksChange?.Invoke();
            SaveSceneBookmarks();
        }

        public static void Insert(int index, Viewpoint viewpoint)
        {
            current._viewpoints.Insert(index, viewpoint);
            onSceneBookmarksChange?.Invoke();
            SaveSceneBookmarks();
        }

        public static void Remove(Viewpoint viewpoint)
        {
            current._viewpoints.Remove(viewpoint);
            onSceneBookmarksChange?.Invoke();
            SaveSceneBookmarks();
        }

        public static void RemoveAt(int index)
        {
            current._viewpoints.RemoveAt(index);
            onSceneBookmarksChange?.Invoke();
            SaveSceneBookmarks();
        }

        public static void Clear()
        {
            current._viewpoints.Clear();
            onSceneBookmarksChange?.Invoke();
            SaveSceneBookmarks();
        }

        public static Viewpoint Find(Predicate<Viewpoint> match)
        {
            return current._viewpoints.Find(match);
        }
        #endregion
    }
}