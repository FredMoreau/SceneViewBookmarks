using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if CINEMACHINE
using Cinemachine;
#endif

namespace UnityEditor.SceneViewBookmarks
{
    [System.Serializable]
    public class Viewpoint
    {
        public string name;
        public string notes;
        public ViewSettings settings;
        public LayerMask visibleLayers;
        public LayerMask lockedLayers;
        public bool staysInPlace = false;
        public Action postAction;

        public Viewpoint(string name, ViewSettings settings, LayerMask visibleLayers, LayerMask lockedLayers, bool staysInPlace = false, Action postAction = null)
        {
            this.name = name;
            this.settings = settings;
            this.visibleLayers = visibleLayers;
            this.lockedLayers = lockedLayers;
            this.staysInPlace = staysInPlace;
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
            view.ApplySettings(settings, staysInPlace);
            Tools.visibleLayers = visibleLayers;
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