using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private Animator anim;
    [SerializeField]
    private AnimatorIK aIK;

    public GameObject WeaponHolder;

    public GameObject PrimaryWeapon;
    public GameObject SecondaryWeapon;

    private Weapon PrimaryWeaponScript;
    private Weapon SecondaryWeaponScript;

    public WeaponType CurrentWeapon;

    private bool swappingWeapons;

    private Transform[] BonesToRotate;
    private float AngleToRotate;

    private Dictionary<string, List<Item>> ItemInventory;
    private List<Key> keys;

    // Start is called before the first frame update
    void Start()
    {
        aIK = GetComponent<AnimatorIK>();
        anim = GetComponent<Animator>();
        CurrentWeapon = WeaponType.PRIMARY;
        ItemInventory = new Dictionary<string, List<Item>>();
        keys = new List<Key>();
        swappingWeapons = false;
        if(PrimaryWeapon)
            PrimaryWeaponScript = PrimaryWeapon.GetComponent<Weapon>();

        if(SecondaryWeapon)
            SecondaryWeaponScript = SecondaryWeapon.GetComponent<Weapon>();

    }

    

    // Update is called once per frame
    void Update()
    {
        if (swappingWeapons)
            return;

        if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0 || Input.GetKeyDown(KeyCode.Joystick1Button3))
        {
            StartCoroutine(WeaponSwap());
            //SwapWeapons();
        }
    }

    public void SwapWeapons()
    {
        switch (CurrentWeapon)
        {
            case WeaponType.PRIMARY:
                SecondaryWeapon.SetActive(true);
                PrimaryWeapon.SetActive(false);
                CurrentWeapon = WeaponType.SECONDARY;
                break;

            case WeaponType.SECONDARY:
                SecondaryWeapon.SetActive(false);
                PrimaryWeapon.SetActive(true);
                CurrentWeapon = WeaponType.PRIMARY;
                break;
        }
    }


    public bool CheckForKey(int KeyNumber)
    {
        foreach (Key key in keys)
        {
            if (key.LockNumber == KeyNumber)
            {
                Debug.Log("Match!");
                keys.Remove(key);
                return true;
            }
        }
        Debug.Log("No Match!");
        return false;
    }

    public void AddItemToInventory(Item item)
    {
        if (item is Key)
        {
            keys.Add((Key)item);
            item.gameObject.SetActive(false);
        }
        else
        {
            item.gameObject.SetActive(false);
            Debug.Log("Picked up " + item.name);
            if (ItemInventory.ContainsKey(item.name))
            {
                List<Item> newList;
                ItemInventory.TryGetValue(item.name, out newList);
                newList.Add(item);
                ItemInventory.Remove(item.name);
                ItemInventory.Add(item.name, newList);
            }
            else
            {
                List<Item> newList = new List<Item>();
                newList.Add(item);
                ItemInventory.Add(item.name, newList);
            }
        }
        
    }

    

    private IEnumerator WeaponSwap()
    {
        swappingWeapons = true;
        anim.SetLayerWeight(2, 1f);
        switch (CurrentWeapon)
        {
            case WeaponType.PRIMARY:
                PrimaryWeaponScript.enabled = false;
                anim.SetTrigger("switchingWeapons");
                yield return new WaitForSeconds(0.6f);
                PrimaryWeapon.SetActive(false);
                SecondaryWeapon.SetActive(true);
                aIK.LeftHandGrip = SecondaryWeaponScript.LeftHandGrip;
                aIK.LeftElbowLocation = SecondaryWeaponScript.LeftElbowGrip;
                yield return new WaitForSeconds(0.6f);
                SecondaryWeaponScript.enabled = true;
                CurrentWeapon = WeaponType.SECONDARY;
                while (!anim.GetCurrentAnimatorStateInfo(1).IsName("Movement"))
                {
                    yield return null;
                }
                
                swappingWeapons = false;
                
                break;

            case WeaponType.SECONDARY:
                SecondaryWeaponScript.enabled = false;
                anim.SetTrigger("switchingWeapons");
                yield return new WaitForSeconds(0.6f);
                SecondaryWeapon.SetActive(false);
                PrimaryWeapon.SetActive(true);
                aIK.LeftHandGrip = PrimaryWeaponScript.LeftHandGrip;
                aIK.LeftElbowLocation = PrimaryWeaponScript.LeftElbowGrip;
                yield return new WaitForSeconds(0.6f);
                PrimaryWeaponScript.enabled = true;
                CurrentWeapon = WeaponType.PRIMARY;
                while (!anim.GetCurrentAnimatorStateInfo(1).IsName("Movement"))
                {
                    yield return null;
                }
                swappingWeapons = false;
                
                break;
        }
        anim.SetLayerWeight(2, 0f);

        
    }
}
