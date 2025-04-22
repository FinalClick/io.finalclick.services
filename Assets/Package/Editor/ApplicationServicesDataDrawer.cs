using UnityEditor;
using UnityEngine;

namespace FinalClick.Services.Editor
{
    [CustomPropertyDrawer(typeof(ApplicationServiceRegistrationSavedData))]
    public class ApplicationServiceRegistrationSavedDataDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 8; // enough space for type + label + large JSON area
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var jsonProp = property.FindPropertyRelative("_serviceAsJson");
            var typeNameProp = property.FindPropertyRelative("_serviceTypeName");

            if (jsonProp == null || typeNameProp == null)
            {
                EditorGUI.LabelField(position, label.text, "Invalid data");
                return;
            }

            Rect typeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            Rect jsonRect = new Rect(position.x, typeRect.y + (EditorGUIUtility.singleLineHeight*2), position.width, EditorGUIUtility.singleLineHeight * 5);

            EditorGUI.LabelField(typeRect, "Service Type Name", typeNameProp.stringValue);
            EditorGUI.LabelField(new Rect(jsonRect.x, jsonRect.y - EditorGUIUtility.singleLineHeight, jsonRect.width, EditorGUIUtility.singleLineHeight), "Service JSON:");
            jsonProp.stringValue = EditorGUI.TextArea(jsonRect, jsonProp.stringValue);
        }
    }
}