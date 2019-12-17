using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class TeamManager : NetworkBehaviour
{
    public static TeamManager instance;
    static int playerCount;
    static int blueTeamCount;
    static int redTeamCount;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }

        SceneManager.activeSceneChanged += OnSceneChange;
    }

    void OnSceneChange(Scene previousScene, Scene newScene)
    {
        if (newScene.name == SceneChanger.START_MENU)
        {
            SceneManager.activeSceneChanged -= OnSceneChange;
            Destroy(gameObject);
        }
    }

    [Server]
    public static void SetPlayerTeam(GameObject newPlayer)
    {
        var player = newPlayer.GetComponent<OnlinePlayer>();
        if (redTeamCount > blueTeamCount)
        {
            //op.AssignColor((int)TeamColor.BLUE);
            player.Team = (int)TeamColor.BLUE;
            blueTeamCount++;
            Debug.Log("Assigning Player_" + player.netId + " to Blue Team");
            //RpcAssignPlayerToTeam(id, (int)TeamColor.BLUE);
        }
        else if (blueTeamCount > redTeamCount)
        {
            //op.AssignColor((int)TeamColor.RED);
            player.Team = (int)TeamColor.RED;
            redTeamCount++;
            Debug.Log("Assigning Player_" + player.netId + " to Red Team");
            //RpcAssignPlayerToTeam(id, (int)TeamColor.RED);
        }
        else
        {
            if (Random.Range(1, 3) == 1)
            {
                //op.AssignColor((int)TeamColor.BLUE);
                player.Team = (int)TeamColor.BLUE;
                blueTeamCount++;
                Debug.Log("Assigning Player_" + player.netId + " to Blue Team");
                //RpcAssignPlayerToTeam(id, (int)TeamColor.BLUE);
            }
            else
            {
                //op.AssignColor((int)TeamColor.RED);
                player.Team = (int)TeamColor.RED;
                redTeamCount++;
                Debug.Log("Assigning Player_" + player.netId + " to Red Team");
                //RpcAssignPlayerToTeam(id, (int)TeamColor.RED);
            }
        }
        playerCount++;
    }
}
