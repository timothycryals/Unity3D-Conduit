using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NetworkPlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Animator anim;

    private Vector3 MoveVector;

    private OnlinePlayer op;

    [SerializeField]
    private float WalkSpeed;
    [SerializeField]
    private float SprintSpeed;
    [SerializeField]
    private float AimedWalkSpeed;

    [SerializeField]
    private float JumpVelocity;

    private float MovementSpeed = 8;

    private bool isGrounded = true;

    private bool isTouchingWall = false;

    Collider collider;

    [HideInInspector]
    public bool isSprinting = false;

    [HideInInspector]
    public bool isAimed = false;

    float hAxis;
    float vAxis;

    Vector3 MovementVector;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        MovementVector = Vector3.zero;
        collider = GetComponent<Collider>();
        op = GetComponent<OnlinePlayer>();
        //DontDestroyOnLoad(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {

        switch (collision.gameObject.layer)
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
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0))
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
        if (NetworkUIManager.isMenuOpen) return;
        if (op.IsDead) return;
        Move();
        Sprint();

        if (isSprinting)
        {
            anim.SetFloat("Speed", 2f);
        }
        else
        {
            anim.SetFloat("Speed", vAxis);
        }
    }

    private void Move()
    {
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");
        if (Mathf.Abs(vAxis) > 0 || Mathf.Abs(hAxis) > 0)
        {
            if (isAimed)
            {
                //MovementVector += transform.position + (transform.forward * vAxis) * AimedWalkSpeed * Time.fixedDeltaTime +
                //(transform.right * hAxis) * AimedWalkSpeed * Time.fixedDeltaTime;

                MovementVector = (transform.forward * vAxis * AimedWalkSpeed) + (transform.right * hAxis * AimedWalkSpeed);
                
            }
            else
            {
                //MovementVector += transform.position + (transform.forward * vAxis) * MovementSpeed * Time.fixedDeltaTime +
                //(transform.right * hAxis) * WalkSpeed * Time.fixedDeltaTime;

                MovementVector = (transform.forward * vAxis * MovementSpeed) + (transform.right * hAxis * WalkSpeed);
            }
            //MovementVector.y = rb.velocity.y;
            rb.AddForce(MovementVector, ForceMode.VelocityChange);
        }
    }

    private void Sprint()
    {
        bool ShiftKey = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Joystick1Button8);
        if (ShiftKey)
        {
            if (!isTouchingWall && !isAimed)
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

    // Update is called once per frame
    void Update()
    {

        if (NetworkUIManager.isMenuOpen) return;
        if (op.IsDead) return;
        Jump();
    }
}
