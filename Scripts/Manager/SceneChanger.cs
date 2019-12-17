using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RotaryHeart.Lib.SerializableDictionary;

public enum SceneName { StartMenu, Lobby, FloatingWorld, Pyramid, Tomb, Zombies, Island, Multiplayer, Settings}

[Serializable]
public class SceneDictionary : SerializableDictionaryBase<SceneName, string> { }

public class SceneChanger : MonoBehaviour
{
    public const string FLOATING_WORLD = "floating world";
    public const string ZOMBIES = "Jeremiah's scene 1";
    public const string PYRAMID = "Pyramid";
    public const string TOMB = "Tomb";
    public const string LOBBY = "lobby2";
    public const string START_MENU = "MainMenu";
    public const string ISLAND = "Island";
    public const string MULTIPLAYER = "FFA_FloatingWorlds";
    public const string SETTINGS = "Settings";

    public static bool isLoadingScene;

    private static SceneDictionary Scenes = new SceneDictionary() {
            { SceneName.StartMenu, START_MENU},
            { SceneName.Lobby, LOBBY },
            { SceneName.FloatingWorld, FLOATING_WORLD },
            { SceneName.Pyramid, PYRAMID },
            { SceneName.Tomb, TOMB },
            { SceneName.Zombies, ZOMBIES},
            { SceneName.Island, ISLAND},
            { SceneName.Multiplayer, MULTIPLAYER},
            { SceneName.Settings, SETTINGS}

        };

    [SerializeField]
    private SceneName SceneToLoad;

    public static SceneName currentScene;

    [SerializeField]
    private bool isPortal;

    [SerializeField]
    private bool isPlayer;

    public void Start()
    {
        GetSceneFromString(SceneManager.GetActiveScene().name);
        if (currentScene != SceneName.StartMenu && currentScene != SceneName.Settings && currentScene != SceneName.Multiplayer)
        {
            if (GameManager.Instance.CheckIfSceneCompleted(SceneToLoad))
            {
                Debug.Log("Disabling object");
                gameObject.SetActive(false);
            }
        }
        
    }

    public void Update()
    {
        if (!isPlayer) return;

        if(Input.GetKeyDown(KeyCode.Home))
        {
            PlayerDied();
        }
    }

    public void GetSceneFromString(string name)
    {
        switch (name)
        {
            case FLOATING_WORLD:
                currentScene = SceneName.FloatingWorld;
                break;
            case ZOMBIES:
                currentScene = SceneName.Zombies;
                break;
            case PYRAMID:
                currentScene = SceneName.Pyramid;
                break;
            case TOMB:
                currentScene = SceneName.Tomb;
                break;
            case ISLAND:
                currentScene = SceneName.Island;
                break;
        }
    }

    public void NextLevel()
    {
        string sceneName;
        Scenes.TryGetValue(SceneToLoad, out sceneName);
        if (sceneName != null && sceneName != "")
        {
            if (currentScene != SceneName.StartMenu)
                GameManager.Instance.MarkSceneAsCompleted(SceneToLoad, true);
            GameManager.Instance.EnableLoadingScreen();
            GameManager.PlayerInfo.DeactivateBehaviours();
            StartCoroutine(LoadSceneAsynchronously(sceneName));
        }
        else
            Debug.Log("No scene by that name");
    }

    public void quitGame()
    {
        Application.Quit();
        Debug.Log("you quit");
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPlayer) return;
        if(other.tag == "Player" && !isLoadingScene){
            if (isPortal)
                NextLevel();
            else
                NextArea();
        }
    }

    public void NextArea()
    {
        string sceneName;
        Scenes.TryGetValue(SceneToLoad, out sceneName);
        if (sceneName != null && sceneName != "")
        {
            if(currentScene != SceneName.StartMenu && currentScene != SceneName.Settings && currentScene != SceneName.Multiplayer)
                GameManager.Instance.MarkSceneAsCompleted(SceneToLoad, true);

            isLoadingScene = true;
            SceneManager.LoadScene(sceneName);
        }
        else
            Debug.Log("No scene by that name");
    }

    public IEnumerator LoadSceneAsynchronously(string sceneName)
    {
        AsyncOperation operation = null;
        yield return null;
        try
        {
            Debug.Log("Changing scenes");
            Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;
            operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;
        }
        catch (Exception e)
        {
            Application.backgroundLoadingPriority = ThreadPriority.Normal;
            Debug.Log("No scene by that name");
            GameManager.Instance.DisableLoadingScreen();
            GameManager.PlayerInfo.ActivateBehaviours();
            isLoadingScene = false;
            StopAllCoroutines();
        }

        while (!operation.isDone)
        {
            yield return null;
            if(operation.progress >= 0.9f)
            {
                break;
            }
        }

        for(int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(1f);
        }

        GameManager.Instance.StartCoroutine(GameManager.Instance.FadeOut());
        yield return new WaitForSeconds(0.1f);
        while (GameManager.isFadingOut)
        {
            yield return null;
        }
        GameManager.Instance.DisableLoadingScreen();
        isLoadingScene = false;
        Application.backgroundLoadingPriority = ThreadPriority.Normal;
        operation.allowSceneActivation = true;
        
    }
    
    public void PlayerDied()
    {
        if (!isLoadingScene)
        {
            isLoadingScene = true;
            GameManager.Instance.MarkSceneAsCompleted(currentScene, false);

            string sceneName;
            Scenes.TryGetValue(SceneToLoad, out sceneName);
            if (sceneName != null && sceneName != "")
            {
                GameManager.Instance.EnableLoadingScreen();
                GameManager.PlayerInfo.DeactivateBehaviours();
                StartCoroutine(LoadSceneAsynchronously(sceneName));
            }
            else
                Debug.Log("No scene by that name");
        }
    }
}
