using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityEditor.SceneViewBookmarks
{
    /// <summary>
    /// Stores SceneView Bookmarks of a Scene
    /// </summary>
    //[CreateAssetMenu(fileName ="bookmarks.asset", menuName ="SceneView/Bookmarks", order =0)]
    public class SceneBookmarks : ScriptableObject
    {
        // TODO : keep a reference to the scene the bookmark belongs to
        // so that every user may have its own asset and avoid merge conflicts
        //[SerializeField] internal Scene scene;

        [SerializeField] internal List<Viewpoint> viewpoints;
    }
}