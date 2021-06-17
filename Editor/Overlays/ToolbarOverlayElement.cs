// Kept as a Reference, no longer in use.
// Requires Editor Coroutines Package

//#if UNITY_2021_2_OR_NEWER
//using System.Collections;
//using UnityEngine.UIElements;
//using UnityEditor.Toolbars;
//using Unity.EditorCoroutines.Editor;

//namespace UnityEditor.SceneViewBookmarks
//{
//    /// <summary>
//    /// A VisualElement implementing IAccessContainerWindow that waits for the containerWindow not to be null to call OnInitialized()
//    /// </summary>
//    abstract class ToolbarOverlayElement : VisualElement, IAccessContainerWindow
//    {
//        public EditorWindow containerWindow { get; set; }

//        protected ToolbarOverlayElement()
//        {
//            EditorCoroutineUtility.StartCoroutine(DelayedInitialization(), this);
//        }

//        IEnumerator DelayedInitialization()
//        {
//            while (containerWindow == null)
//                yield return null;
//            OnInitialized();
//        }

//        abstract protected void OnInitialized();
//    }
//}
//#endif