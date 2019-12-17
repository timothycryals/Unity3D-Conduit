using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.PostProcessing;

public class MultiplayerGameManager : NetworkBehaviour
{
    public static MultiplayerGameManager instance;

    private const string PLAYER_NAME_PREFIX = "Player_";

    public static bool ControllerEnabled = false;

    private static Dictionary<string, OnlinePlayer> _players = new Dictionary<string, OnlinePlayer>();

    public PostProcessingBehaviour playerPostProcessing;

    [SerializeField]
    private PostProcessingProfile _shipProfile;
    public PostProcessingProfile shipProfile { get { return _shipProfile; } }

    [SerializeField]
    private PostProcessingProfile _islandProfile;
    public PostProcessingProfile islandProfile { get { return _islandProfile; } }

    private int blueTeamCount = 0;
    private int redTeamCount = 0;

    [SerializeField]
    private SpawnPoint[] blueSpawnPoints;
    [SerializeField]
    private SpawnPoint[] redSpawnPoints;

    [SerializeField]
    private SpawnPoint[] spawnPoints;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            Physics.gravity = new Vector3(0, -15f, 0);
            InstantiatePostProcessingProfiles();
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void InstantiatePostProcessingProfiles()
    {
        _shipProfile.vignette.enabled = true;
        _islandProfile.vignette.enabled = true;
    }

    private void ResetPostProcessingProfiles()
    {
        _shipProfile.vignette.enabled = false;
        _islandProfile.vignette.enabled = false;
    }

    public static void RegisterPlayer(string netID, OnlinePlayer player)
    {
        string playerID = netID;
        _players.Add(playerID, player);
        player.transform.name = playerID;
    }

    public static void DeRegisterPlayer(string playerID)
    {
        _players.Remove(playerID);
    }

    public static OnlinePlayer GetPlayer(string playerID)
    {
        OnlinePlayer player;
        _players.TryGetValue(playerID, out player);
        return player;
    }

    [Server]
    public void AssignPlayerToTeam(GameObject player)
    {
        OnlinePlayer op = player.GetComponent<OnlinePlayer>();
        string id = PLAYER_NAME_PREFIX + op.netId;
        if (redTeamCount > blueTeamCount)
        {
            //op.AssignColor((int)TeamColor.BLUE);
            op.Team = (int)TeamColor.BLUE;
            blueTeamCount += 1;
            Debug.Log("Assigning Player_" + op.netId + " to Blue Team");
            //RpcAssignPlayerToTeam(id, (int)TeamColor.BLUE);
        }
        else if (blueTeamCount > redTeamCount)
        {
            //op.AssignColor((int)TeamColor.RED);
            op.Team = (int)TeamColor.RED;
            redTeamCount += 1;
            Debug.Log("Assigning Player_" + op.netId + " to Red Team");
            //RpcAssignPlayerToTeam(id, (int)TeamColor.RED);
        }
        else
        {
            if (UnityEngine.Random.Range(1, 3) == 1)
            {
                //op.AssignColor((int)TeamColor.BLUE);
                op.Team = (int)TeamColor.BLUE;
                blueTeamCount += 1;
                Debug.Log("Assigning Player_" + op.netId + " to Blue Team");
                //RpcAssignPlayerToTeam(id, (int)TeamColor.BLUE);
            }
            else
            {
                //op.AssignColor((int)TeamColor.RED);
                op.Team = (int)TeamColor.RED;
                redTeamCount += 1;
                Debug.Log("Assigning Player_" + op.netId + " to Red Team");
                //RpcAssignPlayerToTeam(id, (int)TeamColor.RED);
            }
        }
    }

    [ClientRpc]
    public void RpcAssignPlayerToTeam(string ID, int teamNumber)
    {
        if (!isServer)
        {
            CmdDebugString(GetPlayer(ID).name);
            GetPlayer(ID).AssignColor(teamNumber);
        }
    }

    [Command]
    public void CmdDebugString(string text)
    {
        Debug.Log(text);
    }
    
    //[Server]
    public void SpawnPlayer(GameObject player)
    {
        OnlinePlayer op = player.GetComponent<OnlinePlayer>();
        string id = PLAYER_NAME_PREFIX + op.netId;

        int point = UnityEngine.Random.Range(0, spawnPoints.Length);
        //player.GetComponent<NetworkCameraController>().ResetOrientationOnSpawn();
        player.transform.position = spawnPoints[point].transform.position + new Vector3(0.2f, 0, 0);
        player.transform.forward = spawnPoints[point].transform.forward;
        
        //RpcSpawnPlayer(id, spawnPoints[point].transform.position, spawnPoints[point].transform.rotation);

        //if (op.Team == (int)TeamColor.BLUE)
        //{
        //    foreach(SpawnPoint sp in instance.blueSpawnPoints)
        //    {
        //        if (!sp.isBlocked)
        //        {
        //            player.transform.position = sp.transform.position + new Vector3(0.2f, 0, 0);
        //            player.transform.forward = sp.transform.forward;
        //            Debug.Log("Spawning Player_" + op.netId + " on " + sp.name);
        //            RpcSpawnPlayer(id, sp.transform.position, sp.transform.rotation);
        //            return;
        //        }
        //    }
        //}
        //else
        //{
        //    foreach (SpawnPoint sp in instance.redSpawnPoints)
        //    {
        //        if (!sp.isBlocked)
        //        {
        //            player.transform.position = sp.transform.position + new Vector3(0.2f, 0, 0);
        //            player.transform.forward = sp.transform.forward;
        //            Debug.Log("Spawning Player_" + op.netId + " on " + sp.name);
        //            RpcSpawnPlayer(id, sp.transform.position, sp.transform.rotation);
        //            return;
        //        }
        //    }
        //}
    }

    [ClientRpc]
    public void RpcSpawnPlayer(string ID, Vector3 position, Quaternion rotation)
    {
        if (!isServer)
        {
            OnlinePlayer op = GetPlayer(ID);
            op.transform.position = position;
            op.transform.rotation = rotation;

        }
    }
    
    [Server]
    public void RemovePlayerFromTeam(GameObject player)
    {
        OnlinePlayer _player = player.GetComponent<OnlinePlayer>();
        if (_player.Team == (int)TeamColor.BLUE)
        {
            blueTeamCount -= 1;
        }
        else
        {
            redTeamCount -= 1;
        }
    }

    private void OnDestroy()
    {
        ResetPostProcessingProfiles();
    }
}
