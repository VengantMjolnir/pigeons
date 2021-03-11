using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(CompactAttribute))]
public class CompactDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUIUtility.LookLikeControls();
        position.xMin += 4;
        position.xMax -= 4;
        if (property.propertyType == SerializedPropertyType.Vector3)
        {
            property.vector3Value = EditorGUI.Vector3Field(position, label.text, property.vector3Value);
        }
        else if (property.propertyType == SerializedPropertyType.Vector2)
        {
            property.vector2Value = EditorGUI.Vector2Field(position, label.text, property.vector2Value);
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use [Compact] with Vector3 or Vector2");
        }
    }
}
