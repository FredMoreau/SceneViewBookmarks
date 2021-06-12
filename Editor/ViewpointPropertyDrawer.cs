//using UnityEditor.UIElements;
//using UnityEngine.UIElements;

//namespace UnityEditor.SceneViewBookmarks
//{
//    [CustomPropertyDrawer(typeof(Viewpoint))]
//    public class ViewpointDrawer : PropertyDrawer
//    {
//        public override VisualElement CreatePropertyGUI(SerializedProperty property)
//        {
//            var container = new VisualElement();
//            var vp = EditorHelper.GetTargetObjectOfProperty(property) as Viewpoint;

//            var nameField = new PropertyField(property.FindPropertyRelative("name"));
//            var settingsField = new PropertyField(property.FindPropertyRelative("settings"));

//            var visibleLayersField = new PropertyField(property.FindPropertyRelative("visibleLayers"));
//            var lockedLayersField = new PropertyField(property.FindPropertyRelative("lockedLayers"));
//            var overridesField = new PropertyField(property.FindPropertyRelative("overrides"));
//            var shortcutField = new PropertyField(property.FindPropertyRelative("shortcut"));

//            container.Add(nameField);
//            container.Add(settingsField);

//            container.Add(visibleLayersField);
//            container.Add(lockedLayersField);
//            container.Add(overridesField);
//            container.Add(shortcutField);

//            return container;
//        }
//    }
//}