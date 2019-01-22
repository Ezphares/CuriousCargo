using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SceneSelector))]
public class SceneSelectDrawer : PropertyDrawer
{
    const string pathPropertyString = "_scenePath";
    const string assetPropertyString = "scene";

    static readonly RectOffset padding = EditorStyles.helpBox.padding;
    static readonly float lineHeight = EditorGUIUtility.singleLineHeight;
    static readonly float lineSpacing = EditorGUIUtility.standardVerticalSpacing;
    static readonly float footerHeight = 10.0f;

    private struct SceneBuildInfo
    {
        public int index;
        public string path;
        public GUID gUID;
        public EditorBuildSettingsScene scene;
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int lines = property.FindPropertyRelative(assetPropertyString).objectReferenceValue == null ? 1 : 2;

        return padding.vertical + lineHeight * lines + lineSpacing * (lines - 1) + footerHeight;


        //return EditorGUI.GetPropertyHeight(property.FindPropertyRelative(assetPropertyString), label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty pathProperty = property.FindPropertyRelative(pathPropertyString);
        SerializedProperty assetProperty = property.FindPropertyRelative(assetPropertyString);

        position.height -= footerHeight;
        GUI.Box(EditorGUI.IndentedRect(position), GUIContent.none, EditorStyles.helpBox);
        position = padding.Remove(position);
        position.height = lineHeight;

        label.tooltip = "Selected scene asset";

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
        position.y += lineHeight + lineSpacing;

        SceneBuildInfo info = GetSceneBuildInfo(selected);
        if (!info.gUID.Empty())
        {
            DrawSceneBuildInfoLine(position, info, controlID + 1);
        }

        EditorGUI.EndProperty();
    }

    void DrawSceneBuildInfoLine(Rect position, SceneBuildInfo info, int controlID)
    {
        GUIContent iconContent = new GUIContent();
        GUIContent labelContent = new GUIContent();

        labelContent.text = "Build index: " + info.index.ToString();
        if (info.index == -1)
        {
            iconContent = EditorGUIUtility.IconContent("d_winbtn_mac_close");
            labelContent.text = "Scene not in build";
        }
        else if (info.scene.enabled)
        {
            iconContent = EditorGUIUtility.IconContent("d_winbtn_mac_max");
        }
        else
        {
            iconContent = EditorGUIUtility.IconContent("d_winbtn_mac_min");
        }

        Rect labelRect = position;
        position.width = EditorGUIUtility.labelWidth - lineSpacing;
        Rect iconRect = labelRect;
        iconRect.width = iconContent.image.width + lineSpacing;
        labelRect.width -= iconRect.width;
        labelRect.x += iconRect.width;
        EditorGUI.PrefixLabel(iconRect, controlID, iconContent);
        EditorGUI.PrefixLabel(labelRect, controlID, labelContent);
    }

    SceneBuildInfo GetSceneBuildInfo(SceneAsset asset)
    {
        SceneBuildInfo info = new SceneBuildInfo
        {
            index = -1,
            path = string.Empty,
            gUID = new GUID(string.Empty)
        };

        if (asset == null)
        {
            return info;
        }

        info.path = AssetDatabase.GetAssetPath(asset);
        info.gUID = new GUID(AssetDatabase.AssetPathToGUID(info.path));

        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            if (info.gUID.Equals(EditorBuildSettings.scenes[i].guid))
            {
                info.scene = EditorBuildSettings.scenes[i];
                info.index = i;
                break;
            }
        }

        return info;
    }

}
