using System;
using UnityEditor;
using UnityEngine;

namespace FinalClick.Services.Editor
{
    [CustomPropertyDrawer(typeof(ApplicationServiceRegistrationData))]
    public class ApplicationServiceRegistrationDataDrawer : PropertyDrawer
    {
        private const float Padding = 2f;
        private static readonly float RowHeight = EditorGUIUtility.singleLineHeight + Padding;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var data = GetTargetObjectOfProperty(property) as ApplicationServiceRegistrationData;
            if (data == null || !data.IsDataStillValid())
                return RowHeight;

            var count = data.GetRegisterAsTypes().Length;
            return RowHeight * (count + 1); // +1 for the header
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var data = GetTargetObjectOfProperty(property) as ApplicationServiceRegistrationData;

            if (data == null || !data.IsDataStillValid())
            {
                EditorGUI.HelpBox(position, "Invalid or missing service type", MessageType.Error);
                return;
            }

            var types = data.GetRegisterAsTypes();
            var actualType = data.GetServiceType();
        
            var y = position.y;

            // Draw header
            var headerRect = new Rect(position.x, y, position.width, RowHeight);
            DrawRow(headerRect, "Register As", "Concrete Type", true);
            y += RowHeight;

            foreach (var registerAsType in types)
            {
                var rowRect = new Rect(position.x, y, position.width, RowHeight);
                DrawRow(rowRect, registerAsType.FullName, actualType.FullName, false);
                y += RowHeight;
            }
        }

        private void DrawRow(Rect rowRect, string leftText, string rightText, bool isHeader)
        {
            float mid = rowRect.width / 2f;

            var leftRect = new Rect(rowRect.x, rowRect.y, mid - Padding, rowRect.height);
            var rightRect = new Rect(rowRect.x + mid, rowRect.y, mid - Padding, rowRect.height);

            if (isHeader)
            {
                EditorGUI.LabelField(leftRect, leftText, EditorStyles.boldLabel);
                EditorGUI.LabelField(rightRect, rightText, EditorStyles.boldLabel);
            }
            else
            {
                EditorGUI.LabelField(leftRect, leftText, EditorStyles.label);
                EditorGUI.LabelField(rightRect, rightText, EditorStyles.label);
            }
        }

        private object GetTargetObjectOfProperty(SerializedProperty prop)
        {
            if (prop == null) return null;

            string path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            string[] elements = path.Split('.');

            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }

            return obj;
        }

        private object GetValue(object source, string name)
        {
            if (source == null) return null;

            var type = source.GetType();
            var f = type.GetField(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (f == null) return null;
            return f.GetValue(source);
        }

        private object GetValue(object source, string name, int index)
        {
            var enumerable = GetValue(source, name) as System.Collections.IEnumerable;
            if (enumerable == null) return null;

            var enm = enumerable.GetEnumerator();
            for (int i = 0; i <= index; i++)
            {
                if (!enm.MoveNext()) return null;
            }

            return enm.Current;
        }
    }
}