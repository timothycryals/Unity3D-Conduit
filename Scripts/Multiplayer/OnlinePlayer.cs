using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.PostProcessing;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public class OnlinePlayer : NetworkBehaviour
{
    public static event Action PlayerDied;
    public static event Action PlayerRespawned;

    public const string PLAYER_TAG = "Player";
    public const string DEATH_PARAM = "Dead";
    public const string RESPAWN_PARAM = "Respawned";

    [SerializeField]
    private Material BlueMaterial;
    [SerializeField]
    private Material RedMaterial;

    [SerializeField]
    private GameObject DeathCam;
    [SerializeField]
    private GameObject PlayerCam;

    private Animator anim;
    private NetworkWeaponManager nwm;

    //[SyncVar(hook = "OnTeamChanged")]
    public int Team;

    public string playerID;

    [SerializeField]
    private Renderer mesh;

    [SerializeField]
    private int _maxHealth = 100;
    [SerializeField]
    private int _maxShield = 100;

    [SerializeField]
    private int _currentHealth;
    public int CurrentHealth
    {
        get { return _currentHealth; }
    }

    [SerializeField]
    private int _currentShield;
    public int CurrentShield
    {
        get { return _currentShield; }
    }

    [SerializeField]
    private int _currentScore;
    public int CurrentScore
    {
        get { return _currentScore; }
    }

    private int _ammoTotal;
    public int AmmoTotal
    {
        get { return _ammoTotal; }
        set { _ammoTotal = value; }
    }

    private int _ammoInMag;
    public int AmmoInMag
    {
        get { return _ammoInMag; }
        set { _ammoInMag = value; }
    }

    private bool _isDead = false;
    public bool IsDead
    {
        get { return _isDead; }
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        nwm = GetComponent<NetworkWeaponManager>();
        DeathCam.SetActive(false);
        SetDefaults(100, 0);
    }

    private void Update()
    {
        Reset();
        if(transform.position.y < -100 && !_isDead)
        {
            if(isLocalPlayer)
                Die();
        }
    }

    public override void OnStartLocalPlayer()
    {
        CmdDebugLocal();
        string myPlayerID = PLAYER_TAG + "_" + netId.ToString();
        MultiplayerGameManager.instance.SpawnPlayer(gameObject);
        if (isLocalPlayer)
        {
            MultiplayerGameManager.instance.playerPostProcessing = PlayerCam.GetComponent<PostProcessingBehaviour>();
        }
        //CmdSetPlayerID(myPlayerID);
        //CmdSetTeam(gameObject);
        //CmdUpdateTeam(gameObject);
        //CmdSpawn(gameObject);
    }

    public override void OnStartClient()
    {
        //base.OnStartClient();
        string myPlayerID = PLAYER_TAG + "_" + netId.ToString();
        MultiplayerGameManager.RegisterPlayer(myPlayerID, this);
        //OnTeamChanged(Team);
    }

    #region Respawn_Functions

    public void Die()
    {
        _isDead = true;
        SetDead(true);
        PlayerCam.SetActive(false);
        DeathCam.SetActive(true);
        anim.SetBool(DEATH_PARAM, true);

        nwm.ResetWeapons();

        if(PlayerDied != null)
            PlayerDied();

        StartCoroutine(Respawn(6f));
    }

    public IEnumerator Respawn(float respawnTime)
    {
        yield return new WaitForSeconds(respawnTime);
        //MultiplayerGameManager.instance.SpawnPlayer(gameObject);
        MultiplayerGameManager.instance.SpawnPlayer(gameObject);
        //CmdSpawn(gameObject);
        anim.SetBool(DEATH_PARAM, false);

        nwm.GiveWeapons();

        anim.SetBool(RESPAWN_PARAM, true);
        if(PlayerRespawned != null)
            PlayerRespawned();

        GiveHealth(100, transform.name);
        DeathCam.SetActive(false);
        PlayerCam.SetActive(true);
        _isDead = false;
        SetDead(false);
        GetComponent<NetworkPlayerController>().isAimed = false;
        yield return new WaitForSeconds(0.05f);
        anim.SetBool(RESPAWN_PARAM, false);
    }

    public void Reset()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Home))
            {
                Die();
            }
        }
    }

    public void SetDead(bool alive)
    {
        if (isServer)
            RpcSetDead(alive);
        else
            CmdSetDead(alive);
    }

    [Command]
    public void CmdSetDead(bool alive)
    {
        RpcSetDead(alive);
    }

    [ClientRpc]
    public void RpcSetDead(bool alive)
    {
        if (!isLocalPlayer)
        {
            _isDead = alive;
        }
    }

    #endregion

    #region Team_Functions

    [Command]
    public void CmdUpdateTeam(GameObject player)
    {
        TeamManager.SetPlayerTeam(player);
    }

    public void OnTeamChanged(int team)
    {
        Team = team;
        mesh.material = Team == 0 ? RedMaterial : BlueMaterial;
    }


    [Command]
    public void CmdDebugLocal()
    {
        Debug.Log("On start player called for Player_" + netId.ToString());
    }

    [Command]
    public void CmdSpawn(GameObject player)
    {
        MultiplayerGameManager.instance.SpawnPlayer(player);
    }

    [Command]
    void CmdSetPlayerID(string newID)
    {
        playerID = newID;
    }

    [Command]
    public void CmdSetTeam(GameObject player)
    {
        MultiplayerGameManager.instance.AssignPlayerToTeam(gameObject);
    }

    public void AssignColor(int color)
    {
        if(color == (int)TeamColor.BLUE)
        {
            Team = (int)TeamColor.BLUE;
            mesh.material = BlueMaterial;
        }
        else
        {
            Team = (int)TeamColor.RED;
            mesh.material = RedMaterial;
        }
    }

    #endregion

    #region Score_Functions

    public void AddScore()
    {
        _currentScore++;
    }

    public void GivePlayerScore(string ID)
    {
        if (isServer)
        {
            RpcGivePlayerScore(ID);
        }
        else
        {
            CmdGivePlayerScore(ID);
        }
    }

    [Command]
    public void CmdGivePlayerScore(string ID)
    {
        RpcGivePlayerScore(ID);
    }

    [ClientRpc]
    public void RpcGivePlayerScore(string ID)
    {
        MultiplayerGameManager.GetPlayer(ID).AddScore();
    }

    //Functions that give score to the player that kills this player

    public void GiveAttackerScore(string attackerID)
    {
        if (isServer)
        {
            RpcGiveAttackerScore(attackerID);
        }
        else
        {
            CmdGiveAttackerScore(attackerID);
        }
    }

    [Command]
    public void CmdGiveAttackerScore(string attackerID)
    {
        RpcGiveAttackerScore(attackerID);
    }

    [ClientRpc]
    public void RpcGiveAttackerScore(string attackerID) 
    {
        MultiplayerGameManager.GetPlayer(attackerID).AddScore();
    }

    #endregion

    #region Health_Functions

    public void ReceiveHealth(int healthAmount)
    {
        _currentHealth += healthAmount;
        if (_currentHealth > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
    }

    public void GiveHealth(int amount, string ID)
    {
        if (isServer)
        {
            RpcGiveHealth(amount, ID);
        }
        else
        {
            CmdGiveHealth(amount, ID);
        }
    }

    [Command]
    public void CmdGiveHealth(int amount, string ID)
    {
        RpcGiveHealth(amount, ID);
    }

    [ClientRpc]
    public void RpcGiveHealth(int amount, string ID)
    {
        MultiplayerGameManager.GetPlayer(ID).ReceiveHealth(amount);
    }

    public void ReceiveShield (int shieldAmount)
    {
        _currentShield += shieldAmount;
        if (_currentShield > _maxShield)
        {
            _currentShield = _maxShield;
        }
    }

    public void GiveShield(int amount, string ID)
    {
        if (isServer)
        {
            RpcGiveShield(amount, ID);
        }
        else
        {
            CmdGiveShield(amount, ID);
        }
    }

    [Command]
    public void CmdGiveShield (int amount, string ID)
    {
        RpcGiveShield(amount, ID);
    }

    [ClientRpc]
    public void RpcGiveShield(int amount, string ID)
    {
        MultiplayerGameManager.GetPlayer(ID).ReceiveShield(amount);
    }

    public void SetDefaults(int health, int shield)
    {
        _currentHealth = health;
        _currentShield = shield;
    }

    public void TakeDamage(int damageAmount, string attackerID)
    {
        if (!_isDead)
        {
            if (_currentShield == 0)
            {
                _currentHealth -= damageAmount;
            }
            else
            {
                _currentShield -= damageAmount;
                if (_currentShield < 0)
                {
                    _currentHealth -= Mathf.Abs(_currentShield);
                    _currentShield = 0;
                }
            }
            if (_currentHealth <= 0)
            {
                _isDead = true;
                _currentHealth = 0;
                _currentShield = 0;

                if (isLocalPlayer)
                {
                    GiveAttackerScore(attackerID);
                    Die();
                }

            }
        }
    }

    public void DamagePlayer(int damage, string receiverID, string attackerID)
    {
        if (isServer)
        {
            RpcDamagePlayer(damage, receiverID, attackerID);
        }
        else
        {
            CmdDamagePlayer(damage, receiverID, attackerID);
        }
    }

    [Command]
    public void CmdDamagePlayer(int damage, string receiverID, string attackerID)
    {
        RpcDamagePlayer(damage, receiverID, attackerID);
    }

    [ClientRpc]
    public void RpcDamagePlayer(int damage, string receiverID, string attackerID)
    {
        MultiplayerGameManager.GetPlayer(receiverID).TakeDamage(damage, attackerID);
    }

    #endregion

    private void OnGUI()
    {
        if(isLocalPlayer)
            GUI.Label(new Rect(new Vector2(10, 10), new Vector2(100, 20)), transform.name + ": " + _currentScore + "/10");
    }
}

