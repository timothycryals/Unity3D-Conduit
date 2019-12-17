using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CurrentScene { LOBBY, GATE_2, TOMB}

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;

    private GameObject player;

    private CurrentScene scene;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance.PlayerCreated)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = transform.position;
            player.transform.rotation = transform.rotation;
        }
        else
        {
            player = Instantiate(playerPrefab);
            player.transform.position = transform.position;
            player.transform.rotation = transform.rotation;
            GameManager.Instance.PlayerCreated = true;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
