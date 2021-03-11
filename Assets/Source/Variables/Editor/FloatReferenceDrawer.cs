using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(FloatReference))]
public class FloatReferenceDrawer : PropertyDrawer
{
    private readonly string[] popupOptions = { "Use Constant", "Use Variable" };
    private GUIStyle popupStyle;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (popupStyle == null)
        {
        }
    }
}
