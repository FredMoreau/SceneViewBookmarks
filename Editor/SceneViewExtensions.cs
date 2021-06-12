using UnityEngine;
using System.Reflection;

#if CINEMACHINE
using Cinemachine;
#endif

namespace UnityEditor.SceneViewBookmarks
{
    internal static class SceneViewExtensions
    {
        public static void ApplySettings(this SceneView view, ViewSettings settings, Viewpoint.Overrides overrides)
        {
            if (overrides.Contains(Viewpoint.Overrides.Is2D))
                view.in2DMode = settings.is2D;

            if (overrides.Contains(Viewpoint.Overrides.IsOrtho))
                view.orthographic = settings.ortho;

            if (overrides.Contains(Viewpoint.Overrides.Position))
            {
                view.pivot = settings.pivot;
                view.size = settings.size;
            }

            if (overrides.Contains(Viewpoint.Overrides.Direction))
                view.rotation = settings.rotation;

            if (overrides.Contains(Viewpoint.Overrides.FieldOfView))
                view.cameraSettings.fieldOfView = settings.fov;

            if (overrides.Contains(Viewpoint.Overrides.CameraMode))
                view.cameraMode = settings.mode;

            if (overrides.Contains(Viewpoint.Overrides.Gizmos))
                view.drawGizmos = settings.drawGizmos;

            if (overrides.Contains(Viewpoint.Overrides.Lighting))
                view.sceneLighting = settings.sceneLighting;

            if (overrides.Contains(Viewpoint.Overrides.ViewStates))
                view.sceneViewState.Copy(settings.sceneViewState);

            if (overrides.Contains(Viewpoint.Overrides.Grid))
                view.showGrid = settings.showGrid;

            SceneView.RepaintAll();
        }

        public static void FrameAll(this SceneView view, bool includeTransforms = false, bool instant = true)
        {
            Bounds b = new Bounds();

            foreach (Renderer r in Object.FindObjectsOfType<Renderer>())
                b.Encapsulate(r.bounds);

            if (includeTransforms)
            {
                foreach (Transform t in Object.FindObjectsOfType<Transform>())
                    b.Encapsulate(t.position);
            }

            view.Frame(b, instant);
        }

        public static void FrameLayers(this SceneView view, LayerMask layers, bool includeTransforms = false, bool instant = true)
        {
            Bounds b = new Bounds();

            foreach (Renderer r in Object.FindObjectsOfType<Renderer>())
            {
                if (((1 << r.gameObject.layer) & layers) > 0)
                    b.Encapsulate(r.bounds);
            }

            foreach (Canvas c in Object.FindObjectsOfType<Canvas>())
            {
                if (((1 << c.gameObject.layer) & layers) > 0)
                {
                    RectTransform r = (RectTransform)c.transform;
                    b.Encapsulate(new Bounds(r.position, r.sizeDelta * r.localScale));
                }
            }

            if (includeTransforms)
            {
                foreach (Transform t in Object.FindObjectsOfType<Transform>())
                {
                    if (((1 << t.gameObject.layer) & layers) > 0)
                        b.Encapsulate(t.position);
                }
            }

            view.Frame(b, instant);
        }

        public static void SetSearchFilter(this SceneView view, string filter, int filterMode)
        {
            MethodInfo setSearchType = typeof(SearchableEditorWindow).GetMethod("SetSearchFilter", BindingFlags.NonPublic | BindingFlags.Instance);
            object[] parameters = new object[] { filter, filterMode, true, false };

            setSearchType.Invoke(view, parameters);
        }

        [System.Flags]
        public enum SceneViewStateFlags : int
        {
            ShowSkybox = 1,
            ShowFog = 2,
            ShowFlares = 4,
            ShowMaterialUpdate = 8,
            ShowImageEffects = 16,
            ShowParticleSystems = 32
        }

        public static SceneViewStateFlags GetFlags(this SceneView.SceneViewState state)
        {
            SceneViewStateFlags flags = 0;

            flags |= state.showSkybox ? SceneViewStateFlags.ShowSkybox : 0;
            flags |= state.showFog ? SceneViewStateFlags.ShowFog : 0;
            flags |= state.showFlares ? SceneViewStateFlags.ShowFlares : 0;
            flags |= state.alwaysRefresh ? SceneViewStateFlags.ShowMaterialUpdate : 0;
            flags |= state.showImageEffects ? SceneViewStateFlags.ShowImageEffects : 0;
            flags |= state.showParticleSystems ? SceneViewStateFlags.ShowParticleSystems : 0;

            return flags;
        }

        public static void SetFlags(this SceneView.SceneViewState state, SceneViewStateFlags flags)
        {
            state.showSkybox = (flags & SceneViewStateFlags.ShowSkybox) == SceneViewStateFlags.ShowSkybox;
            state.showFog = (flags & SceneViewStateFlags.ShowFog) == SceneViewStateFlags.ShowFog;
            state.showFlares = (flags & SceneViewStateFlags.ShowFlares) == SceneViewStateFlags.ShowFlares;
            state.alwaysRefresh = (flags & SceneViewStateFlags.ShowMaterialUpdate) == SceneViewStateFlags.ShowMaterialUpdate;
            state.showImageEffects = (flags & SceneViewStateFlags.ShowImageEffects) == SceneViewStateFlags.ShowImageEffects;
            state.showParticleSystems = (flags & SceneViewStateFlags.ShowParticleSystems) == SceneViewStateFlags.ShowParticleSystems;
        }

        public static void Copy (this SceneView.SceneViewState state, SceneView.SceneViewState source)
        {
            state.showSkybox = source.showSkybox;
            state.showFog = source.showFog;
            state.showFlares = source.showFlares;
            state.alwaysRefresh = source.alwaysRefresh;
            state.showImageEffects = source.showImageEffects;
            state.showParticleSystems = source.showParticleSystems;
        }

        public static bool Contains(this Viewpoint.Overrides overrides, Viewpoint.Overrides other)
        {
            return (overrides & other) == other;
        }
    }
}