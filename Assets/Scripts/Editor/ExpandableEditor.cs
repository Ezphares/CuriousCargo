
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Expandable))]
public class ExpandableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Expandable expandable = target as Expandable;

        DrawDefaultInspector();

        if (GUILayout.Button("Expand Right"))
        {
            Selection.activeGameObject = Instantiate(expandable, expandable.transform.position + Vector3.right * 16.0f, expandable.transform.rotation, expandable.transform.parent).gameObject;
        }


        serializedObject.ApplyModifiedProperties();
    }
}
