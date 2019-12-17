using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TeamColor { RED, BLUE, NEUTRAL }

public class SpawnPoint : MonoBehaviour
{
    public TeamColor Team;

    [SerializeField]
    private Material NeutralMaterial;
    [SerializeField]
    private Material BlueMaterial;
    [SerializeField]
    private Material RedMaterial;

    [SerializeField]
    private Renderer[] Renderers;
    
    private Material Material;

    private bool _isBlocked;
    public bool isBlocked
    {
        get { return _isBlocked; }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == OnlinePlayer.PLAYER_TAG)
        {
            _isBlocked = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == OnlinePlayer.PLAYER_TAG)
        {
            _isBlocked = false;
        }
    }

    private void OnValidate()
    {
        if (Application.isEditor)
        {
            switch (Team)
            {
                case TeamColor.NEUTRAL:
                    Material = NeutralMaterial;
                    break;
                case TeamColor.BLUE:
                    Material = BlueMaterial;
                    break;
                case TeamColor.RED:
                    Material = RedMaterial;
                    break;
            }
            foreach (Renderer r in Renderers)
            {
                r.material = Material;
            }
        }
    }
}
