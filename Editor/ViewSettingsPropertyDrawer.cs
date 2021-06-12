//#define USE_UI_ELEMENTS
using System.Collections.Generic;
#if !USE_UI_ELEMENTS
using UnityEngine;
#else
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#endif

namespace UnityEditor.SceneViewBookmarks
{
    [CustomPropertyDrawer(typeof(ViewSettings))]
    public class ViewSettingsDrawer : PropertyDrawer
    {
#if !USE_UI_ELEMENTS
        const int fieldNumber = 11;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * fieldNumber;
        }

        DrawCameraMode newCameraMode, oldCameraMode;
        System.Enum newSceneStateFlags;
        ViewSettings viewSettings;
        bool isInit;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!isInit) // FIXME : this wasn't the best idea
            {
                viewSettings = (ViewSettings)EditorHelper.GetTargetObjectOfProperty(property);
                oldCameraMode = viewSettings.mode.drawMode;
                isInit = true;
            }

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var rects = new Rect[fieldNumber];
            for (int i = 0; i < fieldNumber; i++)
            {
                rects[i] = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * i, position.width, EditorGUIUtility.singleLineHeight);
            }

            EditorGUI.PropertyField(rects[0], property.FindPropertyRelative("pivot"));
            EditorGUI.PropertyField(rects[1], property.FindPropertyRelative("rotation"));
            EditorGUI.PropertyField(rects[2], property.FindPropertyRelative("size"));
            EditorGUI.PropertyField(rects[3], property.FindPropertyRelative("ortho"));
            EditorGUI.PropertyField(rects[4], property.FindPropertyRelative("fov"));
            EditorGUI.PropertyField(rects[5], property.FindPropertyRelative("is2D"));

            EditorGUI.BeginChangeCheck();
            newCameraMode = (DrawCameraMode)EditorGUI.EnumPopup(rects[6], "Mode", viewSettings.mode.drawMode);
            if (EditorGUI.EndChangeCheck())
            {
                try
                {
                    viewSettings.mode = SceneView.GetBuiltinCameraMode(newCameraMode);
                }
                catch
                {

                }
            }
            EditorGUI.PropertyField(rects[7], property.FindPropertyRelative("drawGizmos"));
            EditorGUI.PropertyField(rects[8], property.FindPropertyRelative("sceneLighting"));

            EditorGUI.BeginChangeCheck();
            newSceneStateFlags = EditorGUI.EnumFlagsField(rects[9], "States", viewSettings.sceneViewState.GetFlags());
            if (EditorGUI.EndChangeCheck())
            {
                viewSettings.sceneViewState.SetFlags((SceneViewExtensions.SceneViewStateFlags)newSceneStateFlags);
            }

            EditorGUI.PropertyField(rects[10], property.FindPropertyRelative("showGrid"));

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
#else
        readonly List<string> SceneViewStatesLabels = new List<string>() { "Skybox", "Fog", "Flares", "Animated Materials", "Post Processings", "Particle Systems" };

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            ViewSettings viewSettings = (ViewSettings)EditorHelper.GetTargetObjectOfProperty(property);
            var container = new VisualElement();

            //var pivotField = new PropertyField(property.FindPropertyRelative("pivot"));
            //var rotationField = new PropertyField(property.FindPropertyRelative("rotation"));
            //var sizeField = new PropertyField(property.FindPropertyRelative("size"));
            var orthoField = new PropertyField(property.FindPropertyRelative("ortho"));
            var fovField = new PropertyField(property.FindPropertyRelative("fov"));
            var is2DField = new PropertyField(property.FindPropertyRelative("is2D"));
            //var modeField = new PropertyField(property.FindPropertyRelative("mode")); // draw as Dropdown

            EnumField cameraModeMask = new EnumField("Camera Mode", viewSettings.mode.drawMode) { tooltip = "Set this bookmark's Camera Shading Mode." };
            cameraModeMask.AddToClassList("svbm-cell-mask");
            // Catching cases where SceneView.GetBuiltinCameraMode() will fail on some DrawCameraMode such as Normal and User Defined
            cameraModeMask.RegisterValueChangedCallback((x) =>
            {
                try
                {
                    viewSettings.mode = SceneView.GetBuiltinCameraMode((DrawCameraMode)x.newValue);
                }
                catch
                {
                    cameraModeMask.SetValueWithoutNotify(x.previousValue);
                }
            });

            var drawGizmosField = new PropertyField(property.FindPropertyRelative("drawGizmos"));
            var sceneLightingField = new PropertyField(property.FindPropertyRelative("sceneLighting"));
            var sceneViewStateField = new PropertyField(property.FindPropertyRelative("sceneViewState")); // draw as Flags

            MaskField viewStatesMask = new MaskField(SceneViewStatesLabels, (int)viewSettings.sceneViewState.GetFlags()) { tooltip = "Set this bookmark's SceneView States." };
            viewStatesMask.AddToClassList("svbm-cell-mask");
            viewStatesMask.RegisterValueChangedCallback((x) => viewSettings.sceneViewState.SetFlags((SceneViewExtensions.SceneViewStateFlags)x.newValue));

            var showGridField = new PropertyField(property.FindPropertyRelative("showGrid"));

            //container.Add(pivotField);
            //container.Add(rotationField);
            //container.Add(sizeField);
            container.Add(orthoField);
            container.Add(fovField);
            container.Add(is2DField);
            //container.Add(modeField);
            container.Add(cameraModeMask);
            container.Add(drawGizmosField);
            container.Add(sceneLightingField);
            //container.Add(sceneViewStateField);
            container.Add(viewStatesMask);
            container.Add(showGridField);

            return container;
        }
#endif
    }
}