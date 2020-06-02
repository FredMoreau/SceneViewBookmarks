using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if CINEMACHINE
using Cinemachine;
#endif

namespace UnityEditor.SceneViewBookmarks
{
    internal static class SceneViewExtensions
    {
        public static void ApplySettings(this SceneView view, ViewSettings settings, bool staysInPlace = false)
        {
            if (!staysInPlace)
            {
                view.pivot = settings.pivot;
                view.size = settings.size;
            }

            view.in2DMode = settings.is2D;
            view.rotation = settings.rotation;
            view.orthographic = settings.ortho;
            view.cameraSettings.fieldOfView = settings.fov;
            view.cameraMode = settings.mode;
            view.drawGizmos = settings.drawGizmos;
            view.sceneLighting = settings.sceneLighting;
            view.sceneViewState = settings.sceneViewState;
            view.showGrid = settings.showGrid;
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
    }
}