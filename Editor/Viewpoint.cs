using UnityEngine;
using System;

#if CINEMACHINE
using Cinemachine;
#endif

// TODO : add grid direction

namespace UnityEditor.SceneViewBookmarks
{
    [Serializable]
    public class Viewpoint
    {
        [Flags]
        public enum Overrides : int
        {
            Position = 1,
            Direction = 2,
            FieldOfView = 4,
            IsOrtho = 8,
            Is2D = 16,
            CameraMode = 32,
            Lighting = 64,
            ViewStates = 128,
            Grid = 256,
            Gizmos = 512,
            VisibleLayers = 1024,
            LockedLayers = 2048
        }

        public string name;
        public ViewSettings settings;
        public LayerMask visibleLayers;
        public LayerMask lockedLayers;
        public Overrides overrides = (Overrides) (-1);
        public Action postAction;

        public Viewpoint(string name, ViewSettings settings, LayerMask visibleLayers, LayerMask lockedLayers, Overrides overrides = (Overrides)(-1), Action postAction = null)
        {
            this.name = name;
            this.settings = settings;
            this.visibleLayers = visibleLayers;
            this.lockedLayers = lockedLayers;
            this.overrides = overrides;
            this.postAction = postAction;
        }

        public Viewpoint(string name, ViewSettings settings, LayerMask visibleLayers, LayerMask lockedLayers, Action postAction = null)
        {
            this.name = name;
            this.settings = settings;
            this.visibleLayers = visibleLayers;
            this.lockedLayers = lockedLayers;
            this.postAction = postAction;
        }

        public Viewpoint(string name, SceneView view)
        {
            this.name = name;
            settings = new ViewSettings(view);
            visibleLayers = Tools.visibleLayers;
            lockedLayers = Tools.lockedLayers;
        }

        public void Load(SceneView view)
        {
            view.ApplySettings(settings, overrides);

            if (overrides.Contains(Overrides.VisibleLayers))
                Tools.visibleLayers = visibleLayers;

            if (overrides.Contains(Overrides.VisibleLayers))
                Tools.lockedLayers = lockedLayers;

            postAction?.Invoke();
        }

        public void Save(SceneView view)
        {
            settings = new ViewSettings(view);
            visibleLayers = Tools.visibleLayers;
            lockedLayers = Tools.lockedLayers;
        }
    }
}