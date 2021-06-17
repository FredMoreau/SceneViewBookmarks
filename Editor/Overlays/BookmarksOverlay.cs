#if UNITY_2021_2_OR_NEWER
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Toolbars;

#if CINEMACHINE
using Cinemachine;
#endif

namespace UnityEditor.SceneViewBookmarks
{
    [Overlay(typeof(SceneView), "SceneViewBookmarks", "Bookmarks", true)]
    class BookmarksOverlay : Overlay
    {
        Foldout editorFoldout, camerasFoldout, cmCamerasFoldout, sceneFoldout, filterFoldout, miscFoldout, optionsFoldout;

        public override void OnCreated()
        {
            DefaultBookmarks.onEditorBookmarksChange += () =>
            {
                editorFoldout.Clear();
                editorFoldout.Add(BookmarksOverlayHelper.BuiltinBookmarksVE());
            };
            SceneBookmarks.onSceneBookmarksChange += () =>
            {
                sceneFoldout.Clear();
                sceneFoldout.Add(BookmarksOverlayHelper.SceneBookmarksVE());
            };
        }

        public override VisualElement CreatePanelContent()
        {
            VisualElement root = new VisualElement();

            editorFoldout = new Foldout() { name = "Editor", text = "Editor" };
            root.Add(editorFoldout);
            //editorFoldout.Add(BookmarksOverlayHelper.BuiltinBookmarksVE());

            sceneFoldout = new Foldout() { name = "Scene", text = "Scene" };
            root.Add(sceneFoldout);
            sceneFoldout.Add(BookmarksOverlayHelper.SceneBookmarksVE());

            camerasFoldout = new Foldout() { name = "Cameras", text = "Cameras" };
            root.Add(camerasFoldout);
            camerasFoldout.Add(BookmarksOverlayHelper.SceneCamerasVE());

#if CINEMACHINE
            cmCamerasFoldout = new Foldout() { name = "Cinemachine Cameras", text = "CMV Cameras" };
            root.Add(cmCamerasFoldout);
            cmCamerasFoldout.Add(BookmarksOverlayHelper.SceneCMVCamerasVE());
#endif

            filterFoldout = new Foldout() { name = "Filter", text = "Filter" };
            root.Add(filterFoldout);
            filterFoldout.Add(BookmarksOverlayHelper.FilterVE());

            miscFoldout = new Foldout() { name = "Misc.", text = "Misc." };
            root.Add(miscFoldout);
            miscFoldout.Add(BookmarksOverlayHelper.MiscVE());

            optionsFoldout = new Foldout() { name = "Options", text = "Options" };
            root.Add(optionsFoldout);
            optionsFoldout.Add(BookmarksOverlayHelper.OptionsVE());

            return root;
        }
    }
}
#endif