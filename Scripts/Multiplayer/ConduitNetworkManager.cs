using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ConduitNetworkManager : NetworkManager
{
    private void Start()
    {
        SceneManager.activeSceneChanged += OnSceneChange;
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
        if (conn.playerControllers.Count > 0)
        {
            GameObject player = conn.playerControllers[0].gameObject;
            OnlinePlayer op = player.GetComponent<OnlinePlayer>();
            //TeamManager.SetPlayerTeam(player);
            //MultiplayerGameManager.instance.SpawnPlayer(player);
            
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        //base.OnClientConnect(conn);
        if (string.IsNullOrEmpty(this.onlineScene) || this.onlineScene == this.offlineScene)
        {
            ClientScene.Ready(conn);
            if (this.autoCreatePlayer)
            {
                ClientScene.AddPlayer(conn, 0);
            }
        }
    }

    void OnSceneChange(Scene previousScene, Scene newScene)
    {
        if (newScene.name == SceneChanger.START_MENU)
        {
            SceneManager.activeSceneChanged -= OnSceneChange;
            NetworkServer.Shutdown();
            Destroy(gameObject);
        }
    }

}
