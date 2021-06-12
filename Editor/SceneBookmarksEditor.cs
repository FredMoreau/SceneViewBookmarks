//using UnityEditor;
//using UnityEngine;
//using UnityEditor.UIElements;
//using UnityEngine.UIElements;
//using System;
//using System.Collections.Generic;

//namespace UnityEditor.SceneViewBookmarks
//{
//    [CustomEditor(typeof(SceneBookmarks))]
//    public class SceneBookmarksEditor : Editor
//    {
//        SerializedProperty viewpoints;

//        private void OnEnable()
//        {
//            viewpoints = serializedObject.FindProperty("viewpoints");
//        }

//        public override VisualElement CreateInspectorGUI()
//        {
//            VisualElement customInspector = new VisualElement();

//            const int itemCount = 5;
//            var items = new List<string>(itemCount);
//            for (int i = 1; i <= itemCount; i++)
//                items.Add(i.ToString());

//            Func<VisualElement> makeItem = () => new Label();
//            Action<VisualElement, int> bindItem = (e, i) => (e as Label).text = items[i];
//            const int itemHeight = 16;
//            var listView = new ListView(items, itemHeight, makeItem, bindItem);
//            listView.selectionType = SelectionType.Multiple;
//            listView.onItemsChosen += obj => Debug.Log(obj);
//            listView.onSelectionChange += objects => Debug.Log(objects);
//            listView.style.flexGrow = 1.0f;
//            listView.reorderable = true;
//            customInspector.Add(listView);

//            var vp = new PropertyField(viewpoints);
//            //customInspector.Add(vp);

//            return customInspector;
//        }
//    }
//}