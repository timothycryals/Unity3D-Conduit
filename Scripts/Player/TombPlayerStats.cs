using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TombPlayerStats : MonoBehaviour
{
    public GameObject fire;
    [SerializeField]
    private float _currentHealth;
    [SerializeField]
    private float _maxHealth;

    [SerializeField]
    private int _enemiesKilled;

    [SerializeField]
    private int _enemiesToKill;

    public Vector3 moveDirection;

    Scene currentScene;
    Rigidbody m_Rigidbody;
    GameObject reset;
    GameObject player;

    public AudioClip audioHurt;
    public AudioClip audioHurt2;
    public AudioClip audioHurt3;
    public AudioClip audioKill;
    public AudioClip audioKill2;
    public AudioClip audioKill3;
    public AudioClip audioDeath;
    
    int rand;
    int hurt;
    int prevNum;
    int prevNum2;
    public bool takingDamage;
    public bool isDead;

    private SceneChanger sceneChanger;



    public float CurrentHealth
    {
        get { return _currentHealth; }
        set { _currentHealth = value; }
    }

    public float MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }

    public int EnemiesToKill
    {
        get { return _enemiesToKill; }
        set { _enemiesToKill = value; }
    }


    public int EnemiesKilled
    {
        get { return _enemiesKilled; }
        set { _enemiesKilled = value; }
    }

    public int RandomNumber(int min, int max)
    {
        System.Random random = new System.Random();
        return random.Next(min, max);
    }

    public void setEnemiesToKill()
    {
        currentScene = SceneManager.GetActiveScene();
        if(currentScene.name == "Tomb")
        {
            _enemiesToKill = 11;
        }
        if (currentScene.name == "Pyramid")
        {
            _enemiesToKill = 6;
        }
    }
    public void increaseKillCount()
    {
        AudioSource audio = GetComponent<AudioSource>();

        rand = RandomNumber(2, 9);
        while (rand == prevNum)
        {
            rand = RandomNumber(2, 9);
        }
        prevNum2 = rand;
        
        if(rand == 3)
        {
            audio.PlayOneShot(audioKill);

        }
        else if (rand == 5)
        {
            audio.PlayOneShot(audioKill2);

        }

        else if (rand == 7)
        {
            audio.PlayOneShot(audioKill3);

        }

        _enemiesKilled += 1;
        Debug.Log(_enemiesKilled);
    }

    public int getKillCount()
    {
        return _enemiesKilled;
    }

    


    void Start()
    {
        setEnemiesToKill();
        _enemiesKilled = 0;
        isDead = false;
        takingDamage = false;
        m_Rigidbody = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player");
        sceneChanger = GetComponent<SceneChanger>();

    }

    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "TrapDamage")
        {
            reset = GameObject.FindWithTag("Reset");
            moveDirection = new Vector3(reset.transform.position.x, reset.transform.position.y, reset.transform.position.z);
            player.transform.position = moveDirection;

        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (!takingDamage && !isDead)
        {
            takingDamage = true;
            AudioSource audio = GetComponent<AudioSource>();

            hurt = RandomNumber(1, 4);
            while (hurt == prevNum)
            {
                hurt = RandomNumber(1, 4);
            }
            prevNum = hurt;

            if (hurt == 1)
            {
                audio.PlayOneShot(audioHurt);
                
            }
            else if (hurt == 2)
            {
                audio.PlayOneShot(audioHurt2);
                
            }

            else if (hurt == 3)
            {
                audio.PlayOneShot(audioHurt3);
              
            }


            CurrentHealth -= damageAmount;
            StartCoroutine(Wait());
            if (CurrentHealth <= 0)
            {
                isDead = true;
                audio.PlayOneShot(audioDeath);
                Die();

            }
               

        }
        
    }
    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
        takingDamage = false;
    }

    private void Die()
    {

        m_Rigidbody.constraints = RigidbodyConstraints.None;
        m_Rigidbody.AddForce(transform.forward * 10f);
        sceneChanger.PlayerDied();
        //SceneManager.LoadScene("lobby");
    }

}
