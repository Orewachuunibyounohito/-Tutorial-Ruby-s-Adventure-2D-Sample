using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer( typeof( ReadOnlyInspectorAttribute ) )]
public class ReadOnlyInspectorDrawer : PropertyDrawer
{
    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ){
        GUI.enabled = false;
        // EditorGUI.BeginProperty( position, label, property );
        EditorGUI.PropertyField( position, property, label );
        GUI.enabled = true;
        // EditorGUI.EndProperty();
    }
}
