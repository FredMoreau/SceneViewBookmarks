using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Linq;
using UnityEditor.UI;

#if CINEMACHINE
using Cinemachine;
#endif

// TODO : move to Project Settings
// TODO : make list reorderable
// DONE : use UIElements
// DONE : add all other bookmarks' options (grid, lighting, gizmos, fx, 2d) + override toggles
// DONE : use builtin icons

namespace UnityEditor.SceneViewBookmarks
{
    public class BookmarkEditorWindow : EditorWindow
    {
        public static void Open()
        {
            BookmarkEditorWindow window = GetWindow<BookmarkEditorWindow>();
            window.titleContent = new GUIContent("Bookmark Editor");
            window.minSize = new Vector2(900, 200);

            window.rootVisualElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.unity.sceneview.bookmarks/Editor/SceneViewBookmarks.Styles.uss"));
        }

        GUIContent icon_sceneviewLighting, icon_sceneviewFx, icon_ortho, icon_is2d, icon_grid, icon_locked_layers, icon_visible_layers, icon_camera;
        GUIStyle centeredStyle;

        private void OnEnable()
        {
            icon_sceneviewLighting = EditorGUIUtility.IconContent("d_SceneViewLighting");
            icon_sceneviewFx = EditorGUIUtility.IconContent("d_SceneViewFx");
            icon_ortho = EditorGUIUtility.IconContent("d_SceneViewOrtho");
            icon_is2d = EditorGUIUtility.IconContent("SceneView2D");
            icon_grid = EditorGUIUtility.IconContent("d_GridAxisY");
            icon_visible_layers = EditorGUIUtility.IconContent("d_VisibilityOn");
            icon_locked_layers = EditorGUIUtility.IconContent("d_AssemblyLock"); // LockIcon-On, LockIcon, d_AssemblyLock, d_InspectorLock, InspectorLock
            icon_camera = EditorGUIUtility.IconContent("SceneViewCamera");
            RefreshUI();
        }

        readonly List<string> SceneViewStatesLabels = new List<string>() { "Skybox", "Fog", "Flares", "Animated Materials", "Post Processings", "Particle Systems" };

        private void OnCustomGUI()
        {
            centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;

            GUILayout.BeginHorizontal();
            GUILayout.Space(200); // three buttons
            GUILayout.Label("Menu Path / Name", centeredStyle, GUILayout.Width(220)); // text field
            GUILayout.Label("Overrides", centeredStyle, GUILayout.Width(100)); // mask field
            GUILayout.Label(icon_ortho, GUILayout.Width(30)); // ortho
            GUILayout.Label(icon_is2d, GUILayout.Width(30)); // is 2d
            GUILayout.Label(icon_camera, GUILayout.Width(30)); // is 2d
            GUILayout.Label("Render Mode", centeredStyle, GUILayout.Width(110)); // mask field
            GUILayout.Label(icon_sceneviewLighting, GUILayout.Width(24)); // scene lighting
            GUILayout.Label(icon_sceneviewFx, GUILayout.Width(100)); // mask field
            GUILayout.Label(icon_grid, GUILayout.Width(30)); // grid
            GUILayout.Label("G", centeredStyle, GUILayout.Width(24)); // gizmos
            GUILayout.Label(icon_visible_layers, GUILayout.Width(100)); // mask field
            GUILayout.Label(icon_locked_layers, GUILayout.Width(100)); // mask field
            GUILayout.Space(120); // two buttons
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void RefreshUI()
        {
            rootVisualElement.Clear();

            VisualElement topRowElement = new VisualElement();
            topRowElement.AddToClassList("svbm-row");
            rootVisualElement.Add(topRowElement);

            Button refreshButton = new Button(RefreshUI) { name = "Refresh", text = "Refresh" };
            refreshButton.AddToClassList("svbm-cell");
            topRowElement.Add(refreshButton);

            Button saveButton = new Button(() => Bookmarks.Instance?.SaveToJson(Bookmarks.path)) { name = "Save", text = "Save" };
            saveButton.AddToClassList("svbm-cell");
            topRowElement.Add(saveButton);

            IMGUIContainer header = new IMGUIContainer(OnCustomGUI);
            header.AddToClassList("sceneLightingButton");
            rootVisualElement.Add(header);

            if (Bookmarks.Count == 0)
                return;


            for (int i = 0; i < Bookmarks.Count; i++)
            {
                var index = i;

                VisualElement bookmarkElement = new VisualElement();
                bookmarkElement.AddToClassList("svbm-row");

                Button loadButton = new Button(() => Bookmarks.Instance[index].Load(SceneView.lastActiveSceneView)) { name = "LOAD", text = "LOAD" };
                loadButton.AddToClassList("svbm-cell-button");
                bookmarkElement.Add(loadButton);

                Button updateButton = new Button(() => Bookmarks.Instance[index].Save(SceneView.lastActiveSceneView)) { name = "UPDATE", text = "UPDATE" };
                updateButton.AddToClassList("svbm-cell-button");
                bookmarkElement.Add(updateButton);

                Button deleteButton = new Button(() =>
                {
                    Bookmarks.Instance.viewpoints.RemoveAt(index);
                    RefreshUI();
                }) { name = "DELETE", text = "DELETE" };
                deleteButton.AddToClassList("svbm-cell-button");
                bookmarkElement.Add(deleteButton);

                TextField nameField = new TextField("") { tooltip = "Set this bookmark's menu path / name.\nUse '/' to create a submenu." };
                nameField.value = Bookmarks.Instance[index].name;
                nameField.AddToClassList("svbm-cell-text");
                nameField.RegisterValueChangedCallback((x) => Bookmarks.Instance[index].name = x.newValue);
                bookmarkElement.Add(nameField);

                MaskField overridesMask = new MaskField(Enum.GetNames(typeof(Viewpoint.Overrides)).ToList(), (int)Bookmarks.Instance[index].overrides) { tooltip = "Set this bookmark's Overrides." };
                overridesMask.AddToClassList("svbm-cell-mask");
                overridesMask.RegisterValueChangedCallback((x) => Bookmarks.Instance[index].overrides = (Viewpoint.Overrides)x.newValue);
                bookmarkElement.Add(overridesMask);

                Toggle orthoToggle = new Toggle("") { tooltip = "Enable to set this bookmark's SceneView to Orthographic." };
                orthoToggle.value = Bookmarks.Instance[index].settings.ortho;
                orthoToggle.AddToClassList("svbm-cell-checkbox");
                orthoToggle.RegisterValueChangedCallback((x) => Bookmarks.Instance[index].settings.ortho = x.newValue);
                bookmarkElement.Add(orthoToggle);

                Toggle is2dToggle = new Toggle("") { tooltip = "Enable to set this bookmark's SceneView to 2D Mode." };
                is2dToggle.value = Bookmarks.Instance[index].settings.is2D;
                is2dToggle.AddToClassList("svbm-cell-checkbox");
                is2dToggle.RegisterValueChangedCallback((x) => Bookmarks.Instance[index].settings.is2D = x.newValue);
                bookmarkElement.Add(is2dToggle);

                FloatField fovField = new FloatField() { tooltip = "Set this bookmark's Camera Field of View." };
                fovField.value = Bookmarks.Instance[index].settings.fov;
                fovField.AddToClassList("svbm-cell-float");
                fovField.RegisterValueChangedCallback((x) => Bookmarks.Instance[index].settings.fov = Mathf.Clamp(x.newValue, 4, 120));
                bookmarkElement.Add(fovField);

                EnumField cameraModeMask = new EnumField("", Bookmarks.Instance[index].settings.mode.drawMode) { tooltip = "Set this bookmark's Camera Shading Mode." };
                cameraModeMask.AddToClassList("svbm-cell-mask");
                // Catching cases where SceneView.GetBuiltinCameraMode() will fail on some DrawCameraMode such as Normal and User Defined
                cameraModeMask.RegisterValueChangedCallback((x) => {
                    try
                    {
                        Bookmarks.Instance[index].settings.mode = SceneView.GetBuiltinCameraMode((DrawCameraMode)x.newValue);
                    }
                    catch
                    {
                        cameraModeMask.SetValueWithoutNotify(x.previousValue);
                    }
                    });
                bookmarkElement.Add(cameraModeMask);

                Toggle lightingToggle = new Toggle("") { tooltip = "Enable this bookmark's SceneView Lighting." };
                lightingToggle.value = Bookmarks.Instance[index].settings.sceneLighting;
                lightingToggle.AddToClassList("svbm-cell-checkbox");
                lightingToggle.RegisterValueChangedCallback((x) => Bookmarks.Instance[index].settings.sceneLighting = x.newValue);
                bookmarkElement.Add(lightingToggle);

                MaskField viewStatesMask = new MaskField(SceneViewStatesLabels, (int)Bookmarks.Instance[index].settings.sceneViewState.GetFlags()) { tooltip = "Set this bookmark's SceneView States." };
                viewStatesMask.AddToClassList("svbm-cell-mask");
                viewStatesMask.RegisterValueChangedCallback((x) => Bookmarks.Instance[index].settings.sceneViewState.SetFlags((SceneViewExtensions.SceneViewStateFlags)x.newValue));
                bookmarkElement.Add(viewStatesMask);

                Toggle gridToggle = new Toggle("") { tooltip = "Enable this bookmark's SceneView Grid." };
                gridToggle.value = Bookmarks.Instance[index].settings.showGrid;
                gridToggle.AddToClassList("svbm-cell-checkbox");
                gridToggle.RegisterValueChangedCallback((x) => Bookmarks.Instance[index].settings.showGrid = x.newValue);
                bookmarkElement.Add(gridToggle);

                Toggle gizmosToggle = new Toggle("") { tooltip = "Enable this bookmark's SceneView Gizmos." };
                gizmosToggle.value = Bookmarks.Instance[index].settings.drawGizmos;
                gizmosToggle.AddToClassList("svbm-cell-checkbox");
                gizmosToggle.RegisterValueChangedCallback((x) => Bookmarks.Instance[index].settings.drawGizmos = x.newValue);
                bookmarkElement.Add(gizmosToggle);

                LayerMaskField visibleLayerMask = new LayerMaskField("", Bookmarks.Instance[index].visibleLayers) { tooltip = "Set this bookmark's Visible Layers." };
                visibleLayerMask.AddToClassList("svbm-cell-mask");
                visibleLayerMask.RegisterValueChangedCallback((x) => Bookmarks.Instance[index].visibleLayers = x.newValue);
                bookmarkElement.Add(visibleLayerMask);

                LayerMaskField lockedLayerMask = new LayerMaskField("", Bookmarks.Instance[index].lockedLayers) { tooltip = "Set this bookmark's Locked Layers." };
                lockedLayerMask.AddToClassList("svbm-cell-mask");
                lockedLayerMask.RegisterValueChangedCallback((x) => Bookmarks.Instance[index].lockedLayers = x.newValue);
                bookmarkElement.Add(lockedLayerMask);

                EnumField shortcutMask = new EnumField("", Bookmarks.Instance[index].shortcut) { tooltip = "Set this bookmark's shortcut\nBeware this will override any other function the shortcut is associated with." };
                shortcutMask.AddToClassList("svbm-cell-mask");
                shortcutMask.RegisterValueChangedCallback((x) => Bookmarks.Instance[index].shortcut = (KeyCode)x.newValue);
                bookmarkElement.Add(shortcutMask);

                Button moveUpButton = new Button(() =>
                {
                    if (index <= 0)
                        return;

                    var item = Bookmarks.Instance.viewpoints[index];
                    Bookmarks.Instance.viewpoints.RemoveAt(index);

                    Bookmarks.Instance.viewpoints.Insert(index - 1, item);
                    RefreshUI();
                })
                { name = "Up", text = "Up" };
                moveUpButton.AddToClassList("svbm-cell-button");
                bookmarkElement.Add(moveUpButton);

                Button moveDownButton = new Button(() =>
                {
                    if (index >= Bookmarks.Instance.viewpoints.Count - 1)
                        return;

                    var item = Bookmarks.Instance.viewpoints[index];
                    Bookmarks.Instance.viewpoints.RemoveAt(index);

                    Bookmarks.Instance.viewpoints.Insert(index + 1, item);
                    RefreshUI();
                })
                { name = "Down", text = "Dn" };
                moveDownButton.AddToClassList("svbm-cell-button");
                bookmarkElement.Add(moveDownButton);

                rootVisualElement.Add(bookmarkElement);
            }
        }
    }
}