using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

#if CINEMACHINE
using Cinemachine;
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
    public static class Bookmarks
    {
        internal static bool showAsSceneViewGUI = default; // TODO : move to settings
        internal static bool linkMainCamera = default;
        internal static bool linkFOV = default;

#if CINEMACHINE
        internal static bool soloVirtualCamerasOnSelection = default;
#endif

        static Bookmarks()
        {
            EditorApplication.quitting += EditorApplication_quitting;
            EditorSceneManager.sceneOpened += EditorSceneManager_sceneOpened;
            EditorSceneManager.sceneLoaded += EditorSceneManager_sceneLoaded;
            EditorSceneManager.sceneClosing += EditorSceneManager_sceneClosing;
            EditorSceneManager.activeSceneChangedInEditMode += EditorSceneManager_activeSceneChangedInEditMode;

            SceneBookmarks.LoadSceneBookmarks(SceneManager.GetActiveScene());
        }

        private static void EditorSceneManager_sceneOpened(Scene scene, OpenSceneMode mode)
        {
            SceneBookmarks.LoadSceneBookmarks(scene);
        }

        private static void EditorSceneManager_sceneClosing(Scene scene, bool removingScene)
        {
            SceneBookmarks.SaveSceneBookmarks(scene);
        }

        private static void EditorSceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            SceneBookmarks.LoadSceneBookmarks(scene);
        }

        private static void EditorSceneManager_activeSceneChangedInEditMode(Scene arg0, Scene arg1)
        {
            if (arg0 != null && arg0.path != null)
                SceneBookmarks.SaveSceneBookmarks(arg0);

            SceneBookmarks.LoadSceneBookmarks(arg1);
        }

        private static void EditorApplication_quitting()
        {
            SceneBookmarks.SaveSceneBookmarks(SceneManager.GetActiveScene());
        }
    }
}