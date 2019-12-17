using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerStats : MonoBehaviour
{
    [SerializeField]
    private float _currentHealth;
    [SerializeField]
    private float _maxHealth;
    [SerializeField]
    private float _currentShield;
    [SerializeField]
    private float _maxShield;
    [SerializeField]
    private int _ammoPerMag;
    [SerializeField]
    private int _ammoTotal;

    SceneChanger sceneChanger;


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

    public float CurrentShield
    {
        get { return _currentShield; }
        set { _currentShield = value; }
    }

    public float MaxShield
    {
        get { return _maxShield; }
        set { _maxShield = value; }
    }

    public int AmmoPerMag
    {
        get { return _ammoPerMag; }
        set { _ammoPerMag = value; }
    }

    public int AmmoTotal
    {
        get { return _ammoTotal; }
        set { _ammoTotal = value; }
    }

    private bool isDead = false;

    void Start()
    {
        sceneChanger = GetComponent<SceneChanger>();

    }

    void Update()
    {

    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;
        CurrentHealth -= damageAmount;
        
        if (CurrentHealth <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;
        sceneChanger.PlayerDied();
    }
}
