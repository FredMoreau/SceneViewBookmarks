using UnityEngine;
using System;

#if CINEMACHINE
using Cinemachine;
#endif

namespace UnityEditor.SceneViewBookmarks
{
    [Serializable]
    public struct ViewSettings
    {
        public Vector3 pivot;
        public Quaternion rotation;
        public float size;
        public bool ortho;
        public float fov;
        public bool is2D;
        public SceneView.CameraMode mode;
        public bool drawGizmos;
        public bool sceneLighting;
        public SceneView.SceneViewState sceneViewState;
        public bool showGrid;

        public ViewSettings(SceneView sceneView)
        {
            this.pivot = sceneView.pivot;
            this.rotation = sceneView.rotation;
            this.size = sceneView.size;
            this.ortho = sceneView.orthographic;
            this.fov = sceneView.cameraSettings.fieldOfView;
            this.is2D = sceneView.in2DMode;
            this.mode = sceneView.cameraMode;
            this.drawGizmos = sceneView.drawGizmos;
            this.sceneLighting = sceneView.sceneLighting;
            this.sceneViewState = new SceneView.SceneViewState(sceneView.sceneViewState);
            this.showGrid = sceneView.showGrid;
        }

        public ViewSettings(Vector3 pivot, Quaternion rotation, float size, bool ortho, float fieldOfView, bool is2D, SceneView.CameraMode cameraMode, bool drawGizmos, bool sceneLighting, SceneView.SceneViewState sceneViewState, bool showGrid)
        {
            this.pivot = pivot;
            this.rotation = rotation;
            this.size = size;
            this.ortho = ortho;
            this.fov = fieldOfView;
            this.is2D = is2D;
            this.mode = cameraMode;
            this.drawGizmos = drawGizmos;
            this.sceneLighting = sceneLighting;
            this.sceneViewState = new SceneView.SceneViewState(sceneViewState);
            this.showGrid = showGrid;
        }
    }
}