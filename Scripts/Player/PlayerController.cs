using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    private PlayerInventory inventory;

    private Vector3 MoveVector;

    [SerializeField]
    private float WalkSpeed;
    [SerializeField]
    private float SprintSpeed;

    [SerializeField]
    private float JumpVelocity;

    private float MovementSpeed = 8;

    private bool isGrounded = true;

    private bool isTouchingWall = false;

    private bool isSprinting = false;

    Vector3 MovementVector;

    Collider collider;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        MovementVector = Vector3.zero;
        inventory = GetComponent<PlayerInventory>();
        collider = GetComponent<Collider>();
        //DontDestroyOnLoad(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        switch(collision.gameObject.layer)
        {
            case 9:
                isGrounded = true; //If the collision is on the ground layer (layer 9), the player is now grounded.
                break;

            case 12:
                isSprinting = false; //If the collision is on the walls layer (layer 12), the player cannot sprint.
                //isTouchingWall = true;
                //MovementSpeed = 1;
                break;

            default:
                break;
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(ControllerInputs.XBOX_A))
        {
            int layerMask = 1 << LayerMask.NameToLayer("PlayerModel");
            layerMask = ~layerMask;
            RaycastHit hit;
            if (Physics.Raycast(collider.bounds.center, Vector3.down, out hit, collider.bounds.extents.y + 0.1f, layerMask))
            {
                rb.AddForce(transform.up * JumpVelocity, ForceMode.Impulse);
            }
        }
    }

    private void FixedUpdate()
    {
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");
        if (Mathf.Abs(vAxis) > 0 || Mathf.Abs(hAxis) > 0)
        {
            //MovementVector += transform.position + (transform.forward * vAxis) * MovementSpeed * Time.fixedDeltaTime +
            //    (transform.right * hAxis) * WalkSpeed * Time.fixedDeltaTime;

            MovementVector = (transform.forward * vAxis * MovementSpeed) + (transform.right * hAxis * WalkSpeed) ;
            MovementVector.y = rb.velocity.y;

            rb.velocity = MovementVector;
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }

        bool ShiftKey = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Joystick1Button8);
        if (ShiftKey)
        {
            if (!isTouchingWall)
            {
                isSprinting = true;
                MovementSpeed = SprintSpeed;
            }
        }
        else
        {
            isSprinting = false;
            MovementSpeed = WalkSpeed;
        }
    }

    private void Interact()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Joystick1Button2))
        {
            Vector3 point = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0);
            Ray ray = Camera.main.ScreenPointToRay(point);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.distance < 4)
                {
                    GameObject hitObject = hit.transform.gameObject;
                    switch (hitObject.tag)
                    {
                        case "Button":
                            TriggerScript target = hitObject.GetComponent<TriggerScript>();

                            if (target)
                            {
                                target.ActivateTrigger();
                            }
                            break;

                        case "Item":
                            Item item = hitObject.GetComponent<Item>();

                            if (item)
                            {
                                inventory.AddItemToInventory(item);
                            }
                            break;

                        case "Lock":
                            KeyHole keyLock = hitObject.GetComponent<KeyHole>();
                            if (keyLock)
                            {
                                Debug.Log("Keylock found!");
                                if (inventory.CheckForKey(keyLock.LockNumber))
                                {
                                    keyLock.ActivateTrigger();
                                }

                            }
                            break;

                        default:
                            break;
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Jump();
        Interact();
    }
}
