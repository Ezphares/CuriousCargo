using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class SceneSelector : ISerializationCallbackReceiver
{
#if UNITY_EDITOR
    public SceneAsset scene;
#endif

    [SerializeField] [ReadOnly] private string _scenePath = string.Empty;

    public string scenePath
    {
        get
        {
#if UNITY_EDITOR
            return GetPathFromScene();
#else
            return _scenePath;
#endif
        }
        set
        {
            _scenePath = value;
#if UNITY_EDITOR
            scene = GetSceneFromPath();
#endif
        }
    }

    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR 
        BeforeSerialize();
#endif
    }

    public void OnAfterDeserialize()
    {
#if UNITY_EDITOR
        EditorApplication.update += AfterDeserialize;
#endif
    }

    public static implicit operator string(SceneSelector sceneSelector)
    {

#if UNITY_EDITOR
        return sceneSelector.scene.name;
#else
        return sceneSelector.scenePath;
#endif
    }

#if UNITY_EDITOR
    string GetPathFromScene()
    {
        return scene ? AssetDatabase.GetAssetPath(scene) : string.Empty;
    }

    SceneAsset GetSceneFromPath()
    {
        return AssetDatabase.LoadAssetAtPath<SceneAsset>(_scenePath);
    }

    void BeforeSerialize()
    {
        // Recover scene from path
        if (!scene && !string.IsNullOrEmpty(_scenePath))
        {
            scene = GetSceneFromPath();

            if (!scene)
            {
                _scenePath = string.Empty;
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        }
        else
        {
            _scenePath = GetPathFromScene();
        }
    }

    void AfterDeserialize()
    {
        EditorApplication.update -= AfterDeserialize;

        if (scene)
        {
            return;
        }

        if (!string.IsNullOrEmpty(_scenePath))
        {
            scene = GetSceneFromPath();

            if (!scene)
            {
                _scenePath = string.Empty;
            }

            if (!EditorApplication.isPlaying)
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
            }
        }
    }
#endif
}
