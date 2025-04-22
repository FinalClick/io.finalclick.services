using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FinalClick.Services.Editor
{
    [CustomPropertyDrawer(typeof(ApplicationServiceRegistrationSavedData))]
    public class ApplicationServiceRegistrationSavedDataDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Reserve a big area — dynamic height could be added later.
            return EditorGUIUtility.singleLineHeight * 15;
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

            var type = Type.GetType(typeNameProp.stringValue);
            if (type == null)
            {
                EditorGUI.LabelField(position, label.text, $"Unknown Type: {typeNameProp.stringValue}");
                return;
            }

            object serviceInstance = JsonUtility.FromJson(jsonProp.stringValue, type) ?? Activator.CreateInstance(type);

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            Rect currentRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(currentRect, "Service Type", type.Name);
            currentRect.y += EditorGUIUtility.singleLineHeight + 4;

            EditorGUI.LabelField(currentRect, "Fields:");
            currentRect.y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.indentLevel++;

            foreach (var field in fields)
            {
                if (!field.IsPublic && !field.IsDefined(typeof(SerializeField), true))
                    continue;

                var fieldType = field.FieldType;
                var fieldLabel = ObjectNames.NicifyVariableName(field.Name);
                var oldValue = field.GetValue(serviceInstance);
                object newValue = oldValue;

                Rect fieldRect = new Rect(currentRect.x, currentRect.y, currentRect.width, EditorGUIUtility.singleLineHeight);

                try
                {
                    if (fieldType == typeof(int))
                        newValue = EditorGUI.IntField(fieldRect, fieldLabel, oldValue != null ? (int)oldValue : 0);

                    else if (fieldType == typeof(float))
                        newValue = EditorGUI.FloatField(fieldRect, fieldLabel, oldValue != null ? (float)oldValue : 0f);

                    else if (fieldType == typeof(bool))
                        newValue = EditorGUI.Toggle(fieldRect, fieldLabel, oldValue != null && (bool)oldValue);

                    else if (fieldType == typeof(string))
                        newValue = EditorGUI.TextField(fieldRect, fieldLabel, oldValue as string ?? "");

                    else if (fieldType == typeof(Vector2))
                        newValue = EditorGUI.Vector2Field(fieldRect, fieldLabel, oldValue != null ? (Vector2)oldValue : Vector2.zero);

                    else if (fieldType == typeof(Vector3))
                        newValue = EditorGUI.Vector3Field(fieldRect, fieldLabel, oldValue != null ? (Vector3)oldValue : Vector3.zero);

                    else if (fieldType == typeof(Vector4))
                        newValue = EditorGUI.Vector4Field(fieldRect, fieldLabel, oldValue != null ? (Vector4)oldValue : Vector4.zero);

                    else if (fieldType.IsEnum)
                        newValue = EditorGUI.EnumPopup(fieldRect, fieldLabel, (Enum)(oldValue ?? Activator.CreateInstance(fieldType)));

                    else
                        EditorGUI.LabelField(fieldRect, fieldLabel, $"(Unsupported: {fieldType.Name})");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error drawing field {field.Name}: {ex.Message}");
                    EditorGUI.LabelField(fieldRect, fieldLabel, "(Error)");
                }

                if (!Equals(newValue, oldValue))
                    field.SetValue(serviceInstance, newValue);

                currentRect.y += EditorGUIUtility.singleLineHeight + 2;
            }

            EditorGUI.indentLevel--;

            // Serialize updated service instance back to JSON
            jsonProp.stringValue = JsonUtility.ToJson(serviceInstance);
        }
    }
}