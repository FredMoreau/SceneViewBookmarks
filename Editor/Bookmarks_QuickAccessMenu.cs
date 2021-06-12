using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

#if CINEMACHINE
using Cinemachine;
#endif

namespace UnityEditor.SceneViewBookmarks
{
    public partial class Bookmarks
    {
        public static GenericMenu QuickAccessMenu(SceneView sceneview)
        {
            GenericMenu menu = new GenericMenu();

            if (linkMainCamera)
            {
                menu.AddItem(new GUIContent("Unlock MainCamera from SceneView"), false, () => linkMainCamera = false);
                menu.AddSeparator("");
            }

            foreach (Viewpoint v in Instance.builtinViewpoints)
            {
                menu.AddItem(new GUIContent(v.name), false, () => v.Load(sceneview));
            }

            menu.AddSeparator("");

            //foreach (Camera c in Object.FindObjectsOfType<Camera>()) // DONE : add the includeInactive parameter when possible
            foreach (Camera c in FindObjectsOfTypeAll<Camera>())
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
            foreach (CinemachineVirtualCamera c in FindObjectsOfTypeAll<CinemachineVirtualCamera>())
            {
                menu.AddItem(new GUIContent("Virtual Cameras/" + c.name), false, () =>
                {
                    sceneview.in2DMode = false;
                    if (sceneview.orthographic = c.m_Lens.Orthographic)
                        sceneview.size = c.m_Lens.OrthographicSize; // FIXME not working as expected
                else
                        sceneview.cameraSettings.fieldOfView = c.m_Lens.FieldOfView;
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

            foreach (Viewpoint v in Instance.sceneViewpoints)
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
            menu.AddItem(new GUIContent("Align to Selection"), false, () => AlignToSelection(sceneview));
            menu.AddItem(new GUIContent("Snap Main Camera"), false, () => SnapCamera(Camera.main, sceneview, linkFOV, true));

            // TODO : experiment with sceneview.rootVisualElement
            //menu.AddItem(new GUIContent("Options/Show as UIElements"), false, () =>
            //{
            //    sceneview.rootVisualElement.Add(new Button(() => Debug.Log("TEST")) { text = "TEST" });
            //});

            menu.AddItem(new GUIContent("Options/Show as GUI"), showAsSceneViewGUI, () => showAsSceneViewGUI = !showAsSceneViewGUI);
            menu.AddItem(new GUIContent("Options/Lock MainCamera to SceneView"), linkMainCamera, () => linkMainCamera = !linkMainCamera);
            menu.AddItem(new GUIContent("Options/Link FOV"), linkFOV, () => linkFOV = !linkFOV);
#if CINEMACHINE
            menu.AddItem(new GUIContent("Options/Solo Virtual Cameras"), soloVirtualCamerasOnSelection, () => soloVirtualCamerasOnSelection = !soloVirtualCamerasOnSelection);
#endif
            menu.AddSeparator("Options/");
            menu.AddItem(new GUIContent("Options/Bookmarks Editor"), false, () => BookmarkEditorWindow.Open());
            menu.AddItem(new GUIContent("Options/Clear All Bookmarks"), false, () => Instance.sceneViewpoints.Clear());
            //menu.AddItem(new GUIContent("Options/Save to Disk"), false, () => Instance?.SaveToJson(path));
            //menu.AddSeparator("Options/");
            //menu.AddItem(new GUIContent("Options/Enable AA"), false, () => { sceneview.antiAlias = 8; });
            return menu;
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