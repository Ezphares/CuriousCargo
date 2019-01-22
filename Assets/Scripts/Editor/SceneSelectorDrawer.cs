using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SceneSelector))]
public class SceneSelectProperty : PropertyDrawer
{
    const string pathPropertyString = "_scenePath";
    const string assetPropertyString = "scene";

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property.FindPropertyRelative(assetPropertyString), label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty pathProperty = property.FindPropertyRelative(pathPropertyString);
        SerializedProperty assetProperty = property.FindPropertyRelative(assetPropertyString);

        EditorGUI.BeginProperty(position, GUIContent.none, property);

        EditorGUI.BeginChangeCheck();
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        SceneAsset selected = EditorGUI.ObjectField(position, label, assetProperty.objectReferenceValue, typeof(SceneAsset), false) as SceneAsset;
        if (EditorGUI.EndChangeCheck())
        {
            assetProperty.objectReferenceValue = selected;
            if (!selected)
            {
                pathProperty.stringValue = string.Empty;
            }
        }

        EditorGUI.EndProperty();
    }
}
