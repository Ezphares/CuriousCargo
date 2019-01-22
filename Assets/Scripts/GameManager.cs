using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static GameManager INSTANCE;

    [Header("DEBUG")]
    public SceneSelector testOverride;

    [Header("Inscribed")]
    public SceneSelector[] levels;
    public FadePanel uIPanel;
    public Text startPrompt;

    [Header("Dynamic")]
    [SerializeField] [ReadOnly] int currentLevel;
    [SerializeField] [ReadOnly] string loadedScene;
    [SerializeField] [ReadOnly] bool levelStarted = false;


    private void Awake()
    {
        INSTANCE = this;

        currentLevel = -1;
        loadedScene = string.Empty;
    }

    private void Start()
    {
        uIPanel.OnFadeIn += NextLevel;
        uIPanel.OnFadeOut += StartLevel;
    }

    private void Update()
    {
        if (startPrompt.gameObject.activeSelf && Input.GetButtonDown("Jump"))
        {
            startPrompt.gameObject.SetActive(false);
            NextLevel();
        }
    }

    private void OnDestroy()
    {
        if (INSTANCE == this)
        {
            INSTANCE = null;
        }
    }

    static public void LevelWon()
    {
        INSTANCE?.TryEndLevel(0.0f, true);
    }

    static public void LevelLost()
    {
        INSTANCE?.TryEndLevel(1.0f, false);
    }

    void TryEndLevel(float delay, bool won)
    {
        if (levelStarted)
        {
            levelStarted = false;
            StartCoroutine(EndLevelAfter(delay, won));
        }
    }

    IEnumerator EndLevelAfter(float delay, bool won)
    {
        yield return new WaitForSeconds(delay);
        EndLevel(won);
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
        if (!string.IsNullOrEmpty(loadedScene) && SceneManager.GetSceneByPath(loadedScene).isLoaded)
        {
            SceneManager.UnloadSceneAsync(loadedScene);
        }

        currentLevel = currentLevel + 1;
        if (currentLevel >= levels.Length)
        {
            Debug.Log("Yay!");
            return;
        }


        string scenePath = string.IsNullOrEmpty(testOverride.scenePath) ? levels[currentLevel].scenePath : testOverride.scenePath;

        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
        loadedScene = scenePath;

    }

    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= SceneLoaded;
        uIPanel.FadeOut(); // Callback calls start level
    }

    void StartLevel()
    {
        levelStarted = true;
        Time.timeScale = 1.0f;
    }

}
