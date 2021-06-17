#if UNITY_2021_2_OR_NEWER
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Toolbars;

namespace UnityEditor.SceneViewBookmarks
{
    [Overlay(typeof(SceneView), "SelectionHistory", "Selection History", true)]
    class SelectionHistoryOverlay : Overlay
    {
        List<GameObject> history;
        VisualElement container;

        public override void OnCreated()
        {
            history = new List<GameObject>();
            container = new VisualElement();
            Selection.selectionChanged += () =>
            {
                var selection = Selection.activeGameObject;
                if (selection == null || history.Contains(selection))
                    return;
                history.Add(selection);
                var button = new Button(() => Selection.activeGameObject = selection) { text = selection.name };
                button.RegisterCallback((ContextClickEvent evt) =>
                {
                    container.Remove(button);
                    history.Remove(selection);
                });
                container.Add(button);
            };
            SceneManagement.EditorSceneManager.sceneClosed += (x) =>
            {
                history.Clear();
                container.Clear();
            };
        }

        public override VisualElement CreatePanelContent()
        {
            VisualElement root = new VisualElement();

            root.Add(container);

            return root;
        }
    }
}
#endif