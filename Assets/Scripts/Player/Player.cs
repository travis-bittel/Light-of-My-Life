using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    private bool canMove;

    // If the player is standing on an object tagged with "Ground", they are considered grounded
    [SerializeField]
    private bool isGrounded;


    public const float jumpVelocity = 7;

    [SerializeField]
    new private Rigidbody2D rigidbody;


    private double horizVelocity;
    private double movementAcceleration;
    [SerializeField]
    private float accelerationRate;
    [SerializeField]
    private float maximumHorizSpeed;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float airborneHorizAccelerationMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        if (rigidbody == null)
        {
            rigidbody = GetComponent<Rigidbody2D>();
        }
        if (accelerationRate == 0)
        {
            accelerationRate = 1;
            Debug.LogWarning("Acceleration Rate was 0, defaulting to 1");
        }
        if (maximumHorizSpeed == 0)
        {
            maximumHorizSpeed = 1;
            Debug.LogWarning("MaxHorizSpeed was 0, defaulting to 1");
        }
        if (jumpForce == 0)
        {
            jumpForce = 200;
            Debug.LogWarning("Jump Force was 0, defaulting to 200");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Update isGrounded every frame
        isGrounded = (GetGroundColliderUnderPlayer() != null);

        HandleMovement();
    }

    private void HandleMovement()
    {
        rigidbody.AddForce(new Vector2((float) movementAcceleration * accelerationRate, 0));
        rigidbody.velocity = new Vector2(Mathf.Clamp(rigidbody.velocity.x, -maximumHorizSpeed, maximumHorizSpeed), rigidbody.velocity.y);
    }

    public void OnMove(InputValue value)
    {
        movementAcceleration = value.Get<float>();
    }

    public void OnJump()
    {
        if (isGrounded)
        {
            rigidbody.AddForce(new Vector2(0, jumpForce));
        }
    }

    // This returns the nearest Collider in case we want to use it for something. For now we're just checking that it is non-null
    // to see if the player is grounded.
    private Collider2D GetGroundColliderUnderPlayer()
    {
        Collider2D[] cols = Physics2D.OverlapCapsuleAll(new Vector2(transform.position.x - 0.25f, transform.position.y),
            new Vector2(1.5f, 1.25f), CapsuleDirection2D.Vertical, 0);

        if (cols != null)
        {
            foreach (Collider2D col in cols)
            {
                if (col.gameObject.CompareTag("Ground"))
                {
                    return col;
                }
            }
        }
        return null;
    }
}
