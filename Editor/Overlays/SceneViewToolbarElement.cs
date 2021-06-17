#if UNITY_2021_2_OR_NEWER
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Toolbars;
using Unity.EditorCoroutines.Editor;

namespace UnityEditor.SceneViewBookmarks
{
    /// <summary>
    /// A VisualElement with IAccessContainerWindow that waits for the containerWindow not to be null to call OnInitialized()
    /// </summary>
    abstract class SceneViewToolbarElement : VisualElement, IAccessContainerWindow
    {
        public EditorWindow containerWindow { get; set; }

        protected SceneViewToolbarElement()
        {
            EditorCoroutineUtility.StartCoroutine(DelayedInitialization(), this);
        }

        IEnumerator DelayedInitialization()
        {
            while (containerWindow == null)
                yield return null;
            OnInitialized();
        }

        abstract protected void OnInitialized();
    }
}
#endif