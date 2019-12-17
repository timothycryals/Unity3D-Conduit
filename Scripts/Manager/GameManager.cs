using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public VideoPlayer loadingScreen;
    [SerializeField]
    private Image WhiteFade;

    public static PlayerSetup PlayerInfo;

    private List<SceneName> CompletedScenes;

    public static bool isFadingOut = false;
    public static bool isFadingIn = false;

    private bool gamePaused;
    public bool GamePaused
    {
        get
        {
            return gamePaused;
        }
        set
        {
            gamePaused = value;
        }
    }

    private bool playerCreated;
    public bool PlayerCreated
    {
        get
        {
            return playerCreated;
        }
        set
        {
            playerCreated = value;
        }
    }

    public static bool ControllerEnabled = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }

        playerCreated = false;
        gamePaused = false;

        DontDestroyOnLoad(gameObject);

        CompletedScenes = new List<SceneName>();

        loadingScreen = GetComponent<VideoPlayer>();
        loadingScreen.Prepare();
        SceneManager.activeSceneChanged += OnSceneChanged;
        PlayerSetup.playerCharacterChanged += OnPlayerChanged;
    }

    public void EnableLoadingScreen()
    {
        loadingScreen.Play();
    }

    public void DisableLoadingScreen()
    {
        loadingScreen.Stop();
    }

    public void OnSceneChanged(Scene previousScene, Scene newScene)
    {
        SceneChanger.isLoadingScene = false;
        if (newScene.name == SceneChanger.START_MENU)
        {
            Instance = null;
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(FadeIn());
        }
    }

    public void OnPlayerChanged(PlayerSetup pSetup)
    {
        loadingScreen.targetCamera = pSetup.PlayerCamera;
        StartCoroutine(PrepareLoadingScreen());
    }

    public void MarkSceneAsCompleted(SceneName name, bool isActive)
    {
        if (isActive)
        {
            if (name == SceneName.Lobby) return;
            if (CompletedScenes.Contains(name))
            {
                return;
            }
            else
            {
                CompletedScenes.Add(name);
                SceneChanger.currentScene = name;
            }
        }
        else
        {
            if (name == SceneName.Lobby) return;
            if (!CompletedScenes.Contains(name))
            {
                Debug.Log("Name not in list: " + name.ToString());
                foreach(SceneName sn in CompletedScenes)
                {
                    Debug.Log(sn.ToString());
                }
                return;
            }
            else
            {
                Debug.Log("Removing Scene");
                CompletedScenes.Remove(name);
                CompletedScenes.TrimExcess();
            }
        }
    }

    public bool CheckIfSceneCompleted(SceneName name)
    {
        if(CompletedScenes.Contains(name))
        {
            return true;
        }
        return false;
    }

    public IEnumerator FadeOut()
    {
        if (!isFadingOut)
        {
            isFadingOut = true;
            while (WhiteFade.color.a < 1)
            {
                WhiteFade.color = new Color(WhiteFade.color.r, WhiteFade.color.g, WhiteFade.color.b, WhiteFade.color.a + 1f / 255f);
                yield return new WaitForSeconds(2f / 255f);
            }
            isFadingOut = false;
        }
    }

    public IEnumerator FadeIn()
    {
        if (!isFadingIn)
        {
            isFadingIn = true;
            while (WhiteFade.color.a > 0)
            {
                WhiteFade.color = new Color(WhiteFade.color.r, WhiteFade.color.g, WhiteFade.color.b, WhiteFade.color.a - 1f / 255f);
                yield return new WaitForSeconds(2f / 255f);
            }
            isFadingIn = false;
            PlayerInfo.ActivateBehaviours();
        }
    }

    public IEnumerator PrepareLoadingScreen()
    {
        loadingScreen.Prepare();
        while (!loadingScreen.isPrepared)
        {
            yield return null;
        }
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
        PlayerSetup.playerCharacterChanged -= OnPlayerChanged;
    }
}
