using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    #region Singleton Code
    private static Player _instance;

    public static Player Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("Attempted to Instantiate multiple Players in one scene!");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void OnDestroy()
    {
        if (this == _instance) { _instance = null; }
    }
    #endregion

    [SerializeField]
    private bool canMove;

    // If the player is standing on an object tagged with "Ground", they are considered grounded
    [SerializeField]
    private bool isGrounded;


    public const float jumpVelocity = 7;

    public Rigidbody2D rb;


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

    public bool allowExceedingMaxHorizVelocity;

    public Launchpad launchpadWithAuthority; // Player can only be affected by one launch pad at a time

    // We can add to this list to reduce or increase acceleration further and apply them all when calculating
    public List<float> additionalHorizAccelerationModifers;

    public int abilityLevel; // 1 = Stronger Light, 2 = Dash, 3 = Light Shot

    // Delegate the responsibility for handling lantern stuff to LightHandler if it is present
    public Lantern LanternWithinRange
    {
        get { return lanternWithinRange; }
        set
        {
            if (LightHandler.Instance != null)
            {
                LightHandler.Instance.lanternWithinRange = value;
            }
            lanternWithinRange = value;
        }
    }
    private Lantern lanternWithinRange; // The lantern the player is currently able to interact with

    public Gate gateWithinRange;

    // Start is called before the first frame update
    void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
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
        if (canMove)
        {
            float xForceToAdd = (float) movementAcceleration * accelerationRate;
            if (!isGrounded)
            {
                xForceToAdd *= airborneHorizAccelerationMultiplier;
            }
            foreach (float f in additionalHorizAccelerationModifers)
            {
                xForceToAdd *= f;
            }
            rb.AddForce(new Vector2(xForceToAdd * Time.deltaTime, 0));
            //Debug.Log(new Vector2(xForceToAdd, 0));

            if (!allowExceedingMaxHorizVelocity)
            {
                rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maximumHorizSpeed, maximumHorizSpeed), rb.velocity.y);
            }
        }
    }

    public void OnMove(InputValue value)
    {
        movementAcceleration = value.Get<float>();
    }

    public void OnJump()
    {
        if (isGrounded)
        {
            rb.AddForce(new Vector2(0, jumpForce));
        }
    }

    // This returns the nearest Collider in case we want to use it for something. For now we're just checking that it is non-null
    // to see if the player is grounded.
    private Collider2D GetGroundColliderUnderPlayer()
    {
        Collider2D[] cols = Physics2D.OverlapCapsuleAll(new Vector2(transform.position.x, transform.position.y),
            new Vector2(1.25f, 1.25f), CapsuleDirection2D.Vertical, 0);

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

    // This 
    public void OnInterract()
    {
        if (lanternWithinRange != null)
        {
            if (abilityLevel >= 1)
            {
                lanternWithinRange.SetLitState(true);
            }
            else
            {

            }
        }
        if (gateWithinRange != null && gateWithinRange.isUnlocked)
        {
            if (gateWithinRange.nextScene == "")
            {
                Debug.LogError("Gate had no attached Scene to move to!");
            } else
            {
                SceneManager.LoadSceneAsync(gateWithinRange.nextScene);
            }
        }
    }
}
