using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Coord))]
public class HexCoordinatesDrawer : PropertyDrawer
{
    public override void OnGUI(
        Rect position, SerializedProperty property, GUIContent label
    )
    {
        Coord coordinates = new Coord(
            property.FindPropertyRelative("x").intValue,
            property.FindPropertyRelative("y").intValue
            );
        position = EditorGUI.PrefixLabel(position, label);
        GUI.Label(position, coordinates.ToString());
    }
}