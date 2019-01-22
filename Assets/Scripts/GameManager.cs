using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    static GameManager INSTANCE;

    [Header("DEBUG")]
    public SceneSelector testOverride;

    [Header("Inscribed")]
    public SceneSelector[] levels;
    public FadePanel uIPanel;

    [Header("Dynamic")]
    [SerializeField] int currentLevel;
    [SerializeField] int loadedScene;


    private void Awake()
    {
        INSTANCE = this;

        currentLevel = -1;
        loadedScene = 0;
    }

    private void Start()
    {
        uIPanel.OnFadeIn += NextLevel;
        uIPanel.OnFadeOut += StartLevel;
        NextLevel();
    }

    private void OnDestroy()
    {
        if (INSTANCE == this)
        {
            INSTANCE = null;
        }
    }

    static public void LevelLost()
    {
        INSTANCE?.StartCoroutine(INSTANCE.EndLevelAfter(1.0f, false));
    }

    IEnumerator EndLevelAfter(float delay, bool won)
    {
        yield return new WaitForSeconds(delay);
        EndLevel(false);
    }

    void EndLevel(bool won)
    {
        if (!won)
        {
            currentLevel -= 1;
        }

        Time.timeScale = 0.0f;
        uIPanel.FadeIn(); // Callback calls nextlevel
    }

    void NextLevel()
    {
        if (SceneManager.GetSceneByPath(levels[loadedScene].scenePath).isLoaded)
        {
            SceneManager.UnloadSceneAsync(levels[loadedScene].scenePath);
        }

        currentLevel = currentLevel + 1;
        if (currentLevel >= levels.Length)
        {
            Debug.Log("Yay!");
            return;
        }

        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.LoadSceneAsync(levels[currentLevel].scenePath, LoadSceneMode.Additive);
        loadedScene = currentLevel;

    }

    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= SceneLoaded;
        uIPanel.FadeOut(); // Callback calls start level
    }

    void StartLevel()
    {
        Time.timeScale = 1.0f;
    }

}
