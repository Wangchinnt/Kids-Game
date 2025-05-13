using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;
    [FormerlySerializedAs("_loadingScreen")] [SerializeField] private GameObject loadingScreen;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        if (loadingScreen == null)
        {
            loadingScreen = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "LoadingPanel");
        }
    }
    private void OnEnable()
    {   
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenuScene")
        {
            UIManager.Instance?.ReassignUIObjects();
            AudioManager.Instance.PlayMusic("Main Menu Background Music");
            Debug.Log("Main Menu Scene Loaded");
        }
        if (loadingScreen == null)
        {
            loadingScreen = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.name == "LoadingPanel");
        }
    }
    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(int sceneIndex)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }
    public void LoadSceneWithLoadingScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Show the loading screen
        loadingScreen.SetActive(true);
        // Start loading the scene asynchronously
        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        operation!.allowSceneActivation = false; // Prevent immediate activation

        while (!operation.isDone)
        {
            // Update progress bar (use Mathf.Clamp01 to keep it between 0 and 1)
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            // If the scene is almost loaded
            if (operation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(2f); // Optional delay
                operation.allowSceneActivation = true; // Activate scene
            }

            yield return null;
        }

        // Hide the loading screen after transition
        loadingScreen.SetActive(false);
    }
    
    public void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
